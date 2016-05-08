using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NLog;

using OfficeOpenXml;

using ZuegerAdressbook.Extensions;
using ZuegerAdressbook.Model;

namespace ZuegerAdressbook.Service
{
    public class ExcelImportService : IExcelImportService
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IDocumentStoreFactory _documentStoreFactory;

        public ExcelImportService(IDocumentStoreFactory documentStoreFactory)
        {
            _documentStoreFactory = documentStoreFactory;
        }

        public int Import(string filename)
        {
            try
            {
                IList<Person> persons = new List<Person>();
                var file = new FileInfo(filename);
                using (var package = new ExcelPackage(file))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet != null)
                    {
                        if (IsOldAddressbookExcel(worksheet))
                        {
                            persons = ImportOldAddressbookExcel(worksheet);

                            _logger.Info(LoggerMessage.GetFunctionUsageMessage("Import Old Addressbook"));
                        }
                        else
                        {
                            // tODO
                        }

                        SavePersons(persons);
                    }
                }

                return persons.Count;
            }
            catch (Exception e)
            {
                _logger.Warn(e, "Exception on Import Persons");
                throw;
            }
        }

        private void SavePersons(IList<Person> persons)
        {
            using (var session = _documentStoreFactory.CreateDocumentStore().OpenSession())
            {
                foreach (var person in persons)
                {
                    session.Store(person);
                }

                session.SaveChanges();
            }
        }

        private IList<Person> ImportOldAddressbookExcel(ExcelWorksheet worksheet)
        {
            var persons = new List<Person>();

            var oldWorksheet = new OldAddressbookWorksheet(worksheet);
            for (int rowIndex = 2; ; rowIndex++)
            {
                oldWorksheet.SetRow(rowIndex);
                var person = new Person
                {
                    Gender = oldWorksheet.GetGender(),
                    Firstname = oldWorksheet.GetFirstName(),
                    Lastname = oldWorksheet.GetLastName(),
                    Street1 = oldWorksheet.GetStreet(),
                    Plz = oldWorksheet.GetPlz(),
                    City = oldWorksheet.GetCity(),
                    PhoneNumber = oldWorksheet.GetPhoneNumber(),
                    MobileNumber = oldWorksheet.GetMobilePhoneNumber(),
                    BusinessPhoneNumber = oldWorksheet.GetBusinessPhoneNumber(),
                    EmailAddress = oldWorksheet.GetEmailAddress(),
                    Birthdate = oldWorksheet.GetBirthdate(),
                    Notes = oldWorksheet.GetNotes(),
                    HasJuniorKarte = oldWorksheet.HasJuniorKarte(),
                    HasGeneralAbo = oldWorksheet.HasGeneralAbo(),
                    HasEnkelKarte = oldWorksheet.HasEnkelKarte(),
                    HasHalbtax = oldWorksheet.HasHalbtax(),
                    ChangeDate = DateTime.Now,
                    SbbInformationChangeDate = DateTime.Now
                };

                if (IsEmptyPerson(person) == false)
                {
                    persons.Add(person);
                }
                else
                {
                    break;
                }
            }

            return persons;
        }

        private bool IsEmptyPerson(Person person)
        {
            return person.Firstname.IsNullOrEmpty() && person.Lastname.IsNullOrEmpty();
        }

        private bool IsOldAddressbookExcel(ExcelWorksheet worksheet)
        {
            var isOldWorkbook = true;
            var columnIndex = 1;

            foreach (var oldWorkbookColumnTitle in GetOldWorkbookColumnTitles())
            {
                isOldWorkbook = isOldWorkbook && worksheet.Cells[1, columnIndex].Text == oldWorkbookColumnTitle;
                columnIndex++;
            }

            return isOldWorkbook;
        }

        private IEnumerable<string> GetOldWorkbookColumnTitles()
        {
            yield return "Firma";
            yield return "Firma_2";
            yield return "Anrede";
            yield return "Titel";
            yield return "Name";
            yield return "Vorname";
            yield return "Strasse_Nr";
            yield return "Land_kZ";
            yield return "PLZ";
            yield return "Wohnort";
            yield return "Land";
            yield return "TelP";
            yield return "TelG";
            yield return "Fax";
            yield return "Handy";
            yield return "BriefAnrede";
            yield return "Geburtstag";
            yield return "Beruf";
            yield return "Einteilung";
            yield return "Bemerkung";
            yield return "Kurz_Notiz";
            yield return "Extra1";
            yield return "Extra2";
            yield return "Extra3";
            yield return "Strasse_II";
            yield return "EMail";
            yield return "Homepage_URL";
            yield return "Kunden_Nr";
        }
    }
}
