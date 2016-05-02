using System;
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
    public class PersonViewModelDocumentTests
    {
        private List<Person> _persons;

        private List<Document> _documents;

        private Mock<IMessageDialogService> _messageDialogServiceMock;

        private TestDoucmentStoreFactory _documentStoreFactory;

        private PersonViewModel _viewModel;

        private Mock<IChangeListener> _parentMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _persons = new List<Person>();
            _documents = new List<Document>();

            _parentMock = new Mock<IChangeListener>();
            _documentStoreFactory = new TestDoucmentStoreFactory();

            _messageDialogServiceMock = new Mock<IMessageDialogService>();

            IocKernel.Initialize(new IocConfiguration());
        }

        private void InitializeViewModel(Person person = null)
        {
            _viewModel = new PersonViewModel(_messageDialogServiceMock.Object, _documentStoreFactory, person, _parentMock.Object);
        }

        [TestMethod]
        public void GivenPersonWithNoDocumentsWhenAddingDocumentThenAddsDocument()
        {
            _persons.Add(new Person { Id = "people/1" });
            AddPersons();
            InitializeViewModel(_persons.First());
            _messageDialogServiceMock.Setup(t => t.OpenFileDialog()).Returns(@"C:\\temp\\myFile.txt");

            _viewModel.AddDocumentCommand.Execute(null);

            Assert.AreEqual(1, _viewModel.Documents.Count);
        }

        [TestMethod]
        public void GivenPersonWithAddedDocumentsWhenSavePersonThenAddsDocument()
        {
            _persons.Add(new Person { Id = "people/1" });
            AddPersons();
            InitializeViewModel(_persons.First());
            _messageDialogServiceMock.Setup(t => t.OpenFileDialog()).Returns(@"C:\temp\myFile.txt");
            _viewModel.AddDocumentCommand.Execute(null);

            _viewModel.SaveDocuments();

            using (var session = _documentStoreFactory.CreateDocumentStore().OpenSession())
            {
                var documents = session.Query<Document>().Where(t => t.PersonId == _viewModel.Id).ToList();

                Assert.AreEqual(1, documents.Count);
                Assert.AreEqual(@"C:\temp\myFile.txt", documents.First().FileName);
            }
        }

        [TestMethod]
        public void GivenNewPersonWhenAddAndSaveDocumentThenLinksDocumentToPerson()
        {
            InitializeViewModel();
            _messageDialogServiceMock.Setup(t => t.OpenFileDialog()).Returns(@"C:\temp\myFile.txt");
            _viewModel.AddDocumentCommand.Execute(null);

            // act
            var entity = _viewModel.AcceptChanges();

            using (var session = _documentStoreFactory.CreateDocumentStore().OpenSession())
            {
                session.Store(entity);
                session.SaveChanges();
            }

            _viewModel.Id = entity.Id;

            _viewModel.SaveDocuments();

            // assert
            Assert.AreEqual(_viewModel.Id, _viewModel.Documents.First().PersonId);
            using (var session = _documentStoreFactory.CreateDocumentStore().OpenSession())
            {
                var documents = session.Query<Document>().Where(t => t.PersonId == _viewModel.Id).ToList();

                Assert.AreEqual(1, documents.Count);
                Assert.AreEqual(@"C:\temp\myFile.txt", documents.First().FileName);
            }
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

        private void AddDocuments()
        {
            using (var session = _documentStoreFactory.CreateDocumentStore().OpenSession())
            {
                foreach (var document in _documents)
                {
                    session.Store(document);
                }

                session.SaveChanges();
            }
        }
    }
}
