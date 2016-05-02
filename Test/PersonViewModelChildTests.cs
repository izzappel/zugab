using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using ZuegerAdressbook.Service;
using ZuegerAdressbook.ViewModels;

namespace Test
{
    [TestClass]
    public class PersonViewModelChildTests
    {
        private Mock<IMessageDialogService> _messageDialogServiceMock;

        private TestDoucmentStoreFactory _documentStoreFactory;

        private PersonViewModel _viewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _documentStoreFactory = new TestDoucmentStoreFactory();

            _messageDialogServiceMock = new Mock<IMessageDialogService>();
        }

        private void InitializeViewModel()
        {
            _viewModel = new PersonViewModel(_messageDialogServiceMock.Object, _documentStoreFactory, null, null);
        }

        [TestMethod]
        public void GivenSevenYearsOldThenIsChild()
        {
            InitializeViewModel();
            _viewModel.Birthdate = DateTime.Now.AddYears(-7);

           Assert.IsTrue(_viewModel.IsChild.Value);
        }

        [TestMethod]
        public void GivenSixteenAndOneDayYearsOldThenIsChild()
        {
            // wurde gestern 16
            InitializeViewModel();
            _viewModel.Birthdate = DateTime.Now.AddYears(-16).AddDays(-1);

            Assert.IsTrue(_viewModel.IsChild.Value);
        }

        [TestMethod]
        public void GivenSixteenYearsOldThenIsChild()
        {
            InitializeViewModel();
            _viewModel.Birthdate = DateTime.Now.AddYears(-16);

            Assert.IsTrue(_viewModel.IsChild.Value);
        }

        [TestMethod]
        public void GivenSixteenSubtractOneDayYearsOldThenIsAdult()
        {
            // wird am nächsten tag 16 (ist noch 15)
            InitializeViewModel();
            _viewModel.Birthdate = DateTime.Now.AddYears(-16).AddDays(1);

            Assert.IsTrue(_viewModel.IsChild.Value);
        }

        [TestMethod]
        public void GivenSeventeenSubtractOneDayYearsOldThenIsChild()
        {
            // wird am nächsten tag 17 (ist noch 16)
            InitializeViewModel();
            _viewModel.Birthdate = DateTime.Now.AddYears(-17).AddDays(1);

            Assert.IsTrue(_viewModel.IsChild.Value);
        }

        [TestMethod]
        public void GivenSeventeenYearsOldThenIsChild()
        {
            InitializeViewModel();
            _viewModel.Birthdate = DateTime.Now.AddYears(-17);

            Assert.IsFalse(_viewModel.IsChild.Value);
        }

        [TestMethod]
        public void GivenTwentyYearsOldThenIsAdult()
        {
            InitializeViewModel();
            _viewModel.Birthdate = DateTime.Now.AddYears(-20);

            Assert.IsFalse(_viewModel.IsChild.Value);
        }

        [TestMethod]
        public void GivenUnknownBirthdateThenIsUnknown()
        {
            InitializeViewModel();

            Assert.IsNull(_viewModel.IsChild);
        }
    }
}
