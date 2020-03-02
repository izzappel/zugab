using System;
using System.Collections.Generic;

using OfficeOpenXml;

using ZuegerAdressbook.Extensions;
using ZuegerAdressbook.Model;

namespace ZuegerAdressbook.Service
{
    public class OldAddressbookWorksheet
    {
        private readonly ExcelWorksheet _worksheet;

        private int _row;

        public OldAddressbookWorksheet(ExcelWorksheet worksheet)
        {
            _worksheet = worksheet;
        }

        public void SetRow(int row)
        {
            _row = row;
        }

        public IList<Person> ImportPersons()
        {
            var persons = new List<Person>();

            for (int rowIndex = 2; ; rowIndex++)
            {
                SetRow(rowIndex);
                var person = new Person
                {
                    Gender = GetGender(),
                    Firstname = GetFirstName(),
                    Lastname = GetLastName(),
                    Street1 = GetStreet(),
                    Plz = GetPlz(),
                    City = GetCity(),
                    PhoneNumber = GetPhoneNumber(),
                    MobileNumber = GetMobilePhoneNumber(),
                    EmailAddress = GetEmailAddress(),
                    Birthdate = GetBirthdate(),
                    Notes = GetNotes(),
                    HasJuniorKarte = HasJuniorKarte(),
                    HasGeneralAbo = HasGeneralAbo(),
                    HasEnkelKarte = HasEnkelKarte(),
                    HasHalbtax = HasHalbtax(),
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

        public Gender GetGender()
        {
            var value = _worksheet.Cells[_row, 3].Text;
            if (value == "Frau")
            {
                return Gender.Female;
            }

            return Gender.Male;
        }

        public string GetLastName()
        {
            return _worksheet.Cells[_row, 5].Text;
        }

        public string GetFirstName()
        {
            return _worksheet.Cells[_row, 6].Text;
        }

        public string GetStreet()
        {
            return _worksheet.Cells[_row, 7].Text;
        }

        public string GetPlz()
        {
            return _worksheet.Cells[_row, 9].Text;
        }

        public string GetCity()
        {
            return _worksheet.Cells[_row, 10].Text;
        }

        public string GetPhoneNumber()
        {
            return _worksheet.Cells[_row, 12].Text;
        }

        public string GetBusinessPhoneNumber()
        {
            return _worksheet.Cells[_row, 13].Text;
        }

        public string GetMobilePhoneNumber()
        {
            return _worksheet.Cells[_row, 15].Text;
        }

        public DateTime? GetBirthdate()
        {
            var value = _worksheet.Cells[_row, 17].Text;
            if (value.IsNullOrEmpty() == false)
            {
                DateTime birthdate;
                if (DateTime.TryParse(value, out birthdate))
                {
                    return birthdate;
                }
            }

            return null;
        }

        public string GetEmailAddress()
        {
            return _worksheet.Cells[_row, 26].Text;
        }

        public string GetNotes()
        {
            return _worksheet.Cells[_row, 21].Text;
        }

        public bool HasHalbtax()
        {
            return GetNotes().ToUpperInvariant().Contains("HALBTAX");
        }

        public bool HasGeneralAbo()
        {
            return GetNotes().ToUpperInvariant().Contains("SBB-ERMÄSSIGUNG: GA");
        }

        public bool HasJuniorKarte()
        {
            return GetNotes().ToUpperInvariant().Contains("JUNIORKARTE");
        }

        public bool HasEnkelKarte()
        {
            return GetNotes().ToUpperInvariant().Contains("ENKELKARTE");
        }

        public static bool IsOldAddressbookExcel(ExcelWorksheet worksheet)
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

        private static IEnumerable<string> GetOldWorkbookColumnTitles()
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
