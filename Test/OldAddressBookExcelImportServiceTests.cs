using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ZuegerAdressbook.Model;
using ZuegerAdressbook.Service;

namespace Test
{
    [TestClass]
    public class OldAddressBookExcelImportServiceTests
    {
        private TestDoucmentStoreFactory _documentStoreFactory;

        private IExcelImportService _excelImportService;

        [TestInitialize]
        public void TestInitialize()
        {
            _documentStoreFactory = new TestDoucmentStoreFactory();
        }

        private void InitiliazeService()
        {
            _excelImportService = new ExcelImportService(_documentStoreFactory);
        }

        [TestMethod]
        [DeploymentItem("TestData/OldAddressbookExcel/TestMethod.xlsx", "TestData")]
        public void ImportPersons()
        {
            InitiliazeService();

            var numerOfPersons = _excelImportService.Import("TestData/TestMethod.xlsx");

            Assert.AreEqual(2, numerOfPersons);
            using (var session = _documentStoreFactory.CreateDocumentStore().OpenSession())
            {
                var persons = session.Query<Person>().ToList();
                Assert.AreEqual(2, persons.Count);

                var sandra = persons.First(t => t.Gender == Gender.Female);
                var sandro = persons.First(t => t.Gender == Gender.Male);

                Assert.AreEqual("Meier", sandra.Lastname);
                Assert.AreEqual("Sandra", sandra.Firstname);
                Assert.AreEqual("Sonnenstrasse 33", sandra.Street1);
                Assert.AreEqual("Sonnhausen", sandra.City);
                Assert.AreEqual("8541", sandra.Plz);
                Assert.AreEqual("023 658 96 15", sandra.PhoneNumber);
                Assert.AreEqual("036 745 85 23", sandra.MobileNumber);
                Assert.AreEqual("045 858 36 96", sandra.BusinessPhoneNumber);
                Assert.AreEqual("sandra.meier@gmail.com", sandra.EmailAddress);
                Assert.AreEqual("15.02.1987", sandra.Birthdate.GetValueOrDefault().ToString("dd.MM.yyyy"));
                Assert.IsFalse(sandra.HasEnkelKarte);
                Assert.IsFalse(sandra.HasJuniorKarte);
                Assert.IsFalse(sandra.HasGeneralAbo);
                Assert.IsTrue(sandra.HasHalbtax);

                Assert.AreEqual("Meier", sandro.Lastname);
                Assert.AreEqual("Sandro", sandro.Firstname);
                Assert.AreEqual("Blumenstrasse 3A", sandro.Street1);
                Assert.AreEqual("Blumenort", sandro.City);
                Assert.AreEqual("4125", sandro.Plz);
                Assert.AreEqual("058 745 89 33", sandro.PhoneNumber);
                Assert.AreEqual("045 986 35 74", sandro.MobileNumber);
                Assert.AreEqual("047 856 93 85", sandro.BusinessPhoneNumber);
                Assert.AreEqual("sandro.meier@bluewin.ch", sandro.EmailAddress);
                Assert.AreEqual("25.06.1997", sandro.Birthdate.GetValueOrDefault().ToString("dd.MM.yyyy"));
                Assert.IsFalse(sandro.HasEnkelKarte);
                Assert.IsTrue(sandro.HasJuniorKarte);
                Assert.IsFalse(sandro.HasGeneralAbo);
                Assert.IsFalse(sandro.HasHalbtax);
            }
        }
    }
}
