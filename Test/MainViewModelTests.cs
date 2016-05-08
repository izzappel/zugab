using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using ZuegerAdressbook;
using ZuegerAdressbook.Model;
using ZuegerAdressbook.Service;
using ZuegerAdressbook.ViewModels;

namespace Test
{
    [TestClass]
    public class MainViewModelTests
    {
        private List<Person> _persons;

        private Mock<IMessageDialogService> _messageDialogServiceMock;

        private Mock<IDispatcher> _dispatcherMock;

        private MainViewModel _viewModel;

        private TestDoucmentStoreFactory _documentStoreFactory;

        private Mock<IExcelImportService> _excelImportServiceMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _persons = new List<Person>();

            _documentStoreFactory = new TestDoucmentStoreFactory();
            _dispatcherMock = new Mock<IDispatcher>();

            _excelImportServiceMock = new Mock<IExcelImportService>();

            _messageDialogServiceMock = new Mock<IMessageDialogService>();

            IocKernel.Initialize(new IocConfiguration());
        }

        private void InitializeViewModel()
        {
            _viewModel = new MainViewModel(_documentStoreFactory, _dispatcherMock.Object, _messageDialogServiceMock.Object, _excelImportServiceMock.Object);
        }

        [TestMethod]
        public void GivenNoPersonsWhenAddNewPersonThenNewPersonsInitialized()
        {
            InitializeViewModel();
            _viewModel.NewCommand.Execute(null);

            Assert.IsNotNull(_viewModel.SelectedDetailedPerson);
            Assert.IsNull(_viewModel.SelectedDetailedPerson.Id);
        }

        [TestMethod]
        public void GivenPersonsWhenAddNewPersonThenNewPersonsInitialized()
        {
            _persons.Add(new Person { Id = "person/1" });
            AddPersons();
            InitializeViewModel();
            _viewModel.NewCommand.Execute(null);

            Assert.IsNotNull(_viewModel.SelectedDetailedPerson);
            Assert.IsNull(_viewModel.SelectedDetailedPerson.Id);
        }

        [TestMethod]
        public void GivenChangedPersonWhenAddNewPersonThenAskForRevertChanges()
        {
            _persons.Add(new Person { Id = "person/1" });
            AddPersons();
            InitializeViewModel();
            _viewModel.SelectedDetailedPerson.Firstname = "Sandra";

            _viewModel.NewCommand.Execute(null);

            _messageDialogServiceMock.Verify(t => t.OpenConfirmationDialog(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void GivenRejectedRevertChangesQuestionWhenAddNewPersonThenDoNotAddNewPerson()
        {
            _persons.Add(new Person { Id = "person/1" });
            AddPersons();
            InitializeViewModel();
            _messageDialogServiceMock.Setup(t => t.OpenConfirmationDialog(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            _viewModel.SelectedDetailedPerson.Firstname = "Sandra";

            _viewModel.NewCommand.Execute(null);
            
            Assert.AreEqual("person/1", _viewModel.SelectedDetailedPerson.Id);
            Assert.AreEqual("Sandra", _viewModel.SelectedDetailedPerson.Firstname);
        }

        [TestMethod]
        public void GivenAcceptedRevertChangesQuestionWhenAddNewPersonThenAddNewPerson()
        {
            _persons.Add(new Person { Id = "person/1" });
            AddPersons();
            InitializeViewModel();
            _messageDialogServiceMock.Setup(t => t.OpenConfirmationDialog(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            _viewModel.SelectedDetailedPerson.Firstname = "Sandra";

            _viewModel.NewCommand.Execute(null);
            
            Assert.IsNull(_viewModel.SelectedDetailedPerson.Id);
            Assert.IsNull(_viewModel.SelectedDetailedPerson.Firstname);
        }

        [TestMethod]
        public void GivenAcceptedRevertChangesQuestionWhenAddNewPersonThenRevertChanges()
        {
            _persons.Add(new Person { Id = "person/1" });
            AddPersons();
            InitializeViewModel();
            _messageDialogServiceMock.Setup(t => t.OpenConfirmationDialog(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            _viewModel.SelectedDetailedPerson.Firstname = "Sandra";

            _viewModel.NewCommand.Execute(null);
            
            Assert.AreEqual("person/1", _viewModel.Persons.First().Id);
            Assert.IsNull(_viewModel.Persons.First().Firstname);
        }

        [TestMethod]
        public void GivenChangedPersonWhenSavePersonThenSaveChanges()
        {
            _persons.Add(new Person { Id = "person/1" });
            AddPersons();
            InitializeViewModel();
            _viewModel.SelectedDetailedPerson.Firstname = "Sandra";

            _viewModel.SaveCommand.Execute(null);

            using (var session = _documentStoreFactory.CreateDocumentStore().OpenSession())
            {
                var savedPerson = session.Load<Person>("person/1");
                Assert.IsNotNull(savedPerson);
                Assert.AreEqual("Sandra", savedPerson.Firstname);
            }
        }

        [TestMethod]
        public void GivenChangedPersonWhenSavePersonThenPersonHasNoChanges()
        {
            _persons.Add(new Person { Id = "person/1" });
            AddPersons();
            InitializeViewModel();
            _viewModel.SelectedDetailedPerson.Firstname = "Sandra";

            _viewModel.SaveCommand.Execute(null);

            Assert.AreEqual("person/1", _viewModel.SelectedDetailedPerson.Id);
            Assert.IsFalse(_viewModel.SelectedDetailedPerson.HasChanges);
        }

        [TestMethod]
        public void GivenChangedPersonWhenSelectOtherPersonThenAskForRevertChanges()
        {
            _persons.Add(new Person { Id = "person/1" });
            _persons.Add(new Person { Id = "person/2" });
            AddPersons();
            InitializeViewModel();
            _viewModel.SelectedDetailedPerson.Firstname = "Sandra";

            _viewModel.SelectedListPerson = _viewModel.Persons.Skip(1).First();

            _messageDialogServiceMock.Verify(t => t.OpenConfirmationDialog(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void GivenAcceptedRevertChangesQuestionWhenSelectOtherPersonThenChangeSelectedPerson()
        {
            _persons.Add(new Person { Id = "person/1" });
            _persons.Add(new Person { Id = "person/2" });
            AddPersons();
            InitializeViewModel();
            _messageDialogServiceMock.Setup(t => t.OpenConfirmationDialog(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            _viewModel.SelectedDetailedPerson.Firstname = "Sandra";

            _viewModel.SelectedListPerson = _viewModel.Persons.Skip(1).First();

            Assert.AreEqual("person/2", _viewModel.SelectedDetailedPerson.Id);
            Assert.IsNull(_viewModel.SelectedDetailedPerson.Firstname);
        }

        [TestMethod]
        public void GivenAcceptedRevertChangesQuestionWhenSelectOtherPersonThenRevertChanges()
        {
            _persons.Add(new Person { Id = "person/1" });
            _persons.Add(new Person { Id = "person/2" });
            AddPersons();
            InitializeViewModel();
            _messageDialogServiceMock.Setup(t => t.OpenConfirmationDialog(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            _viewModel.SelectedDetailedPerson.Firstname = "Sandra";

            _viewModel.SelectedListPerson = _viewModel.Persons.Skip(1).First();

            Assert.AreEqual("person/1", _viewModel.Persons.First().Id);
            Assert.IsNull(_viewModel.Persons.First().Firstname);
            Assert.IsFalse(_viewModel.Persons.First().HasChanges);
        }

        [TestMethod]
        public void GivenRejectedRevertChangesQuestionWhenSelectOtherPersonThenDoNotChangeSelectedPerson()
        {
            _persons.Add(new Person { Id = "person/1" });
            _persons.Add(new Person { Id = "person/2" });
            AddPersons();
            InitializeViewModel();
            _messageDialogServiceMock.Setup(t => t.OpenConfirmationDialog(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            _viewModel.SelectedDetailedPerson.Firstname = "Sandra";

            _viewModel.SelectedListPerson = _viewModel.Persons.Skip(1).First();

            Assert.AreEqual("person/1", _viewModel.SelectedDetailedPerson.Id);
            Assert.AreEqual("Sandra", _viewModel.SelectedDetailedPerson.Firstname);
        }

        [TestMethod]
        public void GivenChangedPersonWhenRevertChangesThenAskForRevertChanges()
        {
            _persons.Add(new Person { Id = "person/1" });
            AddPersons();
            InitializeViewModel();
            _viewModel.SelectedDetailedPerson.Firstname = "Sandra";

            _viewModel.RevertCommand.Execute(null);

            _messageDialogServiceMock.Verify(t => t.OpenConfirmationDialog(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void GivenAcceptedRevertChangesWhenRevertChangesThenRevertChanges()
        {
            _persons.Add(new Person { Id = "person/1" });
            AddPersons();
            InitializeViewModel();
            _messageDialogServiceMock.Setup(t => t.OpenConfirmationDialog(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            _viewModel.SelectedDetailedPerson.Firstname = "Sandra";

            _viewModel.RevertCommand.Execute(null);

            Assert.AreEqual("person/1", _viewModel.SelectedDetailedPerson.Id);
            Assert.IsNull(_viewModel.SelectedDetailedPerson.Firstname);
        }

        [TestMethod]
        public void GivenAcceptedRevertChangesWhenRevertChangesThenHasNoChanges()
        {
            _persons.Add(new Person { Id = "person/1" });
            AddPersons();
            InitializeViewModel();
            _messageDialogServiceMock.Setup(t => t.OpenConfirmationDialog(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            _viewModel.SelectedDetailedPerson.Firstname = "Sandra";

            _viewModel.RevertCommand.Execute(null);

            Assert.AreEqual("person/1", _viewModel.SelectedDetailedPerson.Id);
            Assert.IsFalse(_viewModel.SelectedDetailedPerson.HasChanges);
        }

        [TestMethod]
        public void GivenRejectedRevertChangesWhenRevertChangesThenDoNotRevertChanges()
        {
            _persons.Add(new Person { Id = "person/1" });
            AddPersons();
            InitializeViewModel();
            _messageDialogServiceMock.Setup(t => t.OpenConfirmationDialog(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            _viewModel.SelectedDetailedPerson.Firstname = "Sandra";

            _viewModel.RevertCommand.Execute(null);

            Assert.AreEqual("person/1", _viewModel.SelectedDetailedPerson.Id);
            Assert.AreEqual("Sandra", _viewModel.SelectedDetailedPerson.Firstname);
        }

        private void AddPersons()
        {
            using (var session = _documentStoreFactory.CreateDocumentStore().OpenSession())
            {
                foreach (var person in _persons)
                {
                    session.Store(person);
                }

                session.SaveChanges();
            }
        }
    }
}
