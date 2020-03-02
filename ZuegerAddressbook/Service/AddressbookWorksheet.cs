using System;
using System.Collections.Generic;

using OfficeOpenXml;

using ZuegerAdressbook.Model;
using ZuegerAdressbook.Extensions;

namespace ZuegerAdressbook.Service
{
    public class AddressbookWorksheet
    {
        private readonly ExcelWorksheet _worksheet;

        public int Row { get; set; }

        public AddressbookWorksheet(ExcelWorksheet worksheet)
        {
            _worksheet = worksheet;
        }

        public IList<Person> ReadPersons()
        {
            var persons = new List<Person>();

            for (int rowIndex = 2; ; rowIndex++)
            {
                var row = GetRow(rowIndex);
                var person = new Person
                {
                    Id = row.Id,
                    Gender = row.Gender,
                    Firstname = row.Firstname,
                    Lastname = row.Lastname,
                    Title = row.Title,
                    Street1 = row.Street1,
                    Plz = row.Plz,
                    City = row.City,
                    PhoneNumber = row.PhoneNumber,
                    MobileNumber = row.MobileNumber,
                    EmailAddress = row.EmailAddress,
                    Birthdate = row.Birthdate,
                    PassportSurname = row.PassportSurname,
                    PassportGivenName = row.PassportGivenName,
                    PassportNationality = row.PassportNationality,
                    PassportNationalityCode = row.PassportNationalityCode,
                    PlaceOfBirth = row.PlaceOfBirth,
                    PlaceOfOrigin = row.PlaceOfOrigin,
                    PassportNumber = row.PassportNumber,
                    PassportIssueDate = row.PassportIssueDate,
                    PassportExpirationDate = row.PassportExpirationDate,
                    HasJuniorKarte = row.HasJuniorKarte,
                    HasGeneralAbo = row.HasGeneralAbo,
                    HasEnkelKarte = row.HasEnkelKarte,
                    HasHalbtax = row.HasHalbtax,
                    HasCancellationInsurance = row.HasCancellationInsurance,
                    CancellationInsurance = row.CancellationInsurance,
                    CancellationInsuranceIssueDate = row.CancellationInsuranceIssueDate,
                    CancellationInsuranceExpirationDate = row.CancellationInsuranceExpirationDate,
                    FrequentFlyerProgram = row.FrequentFlyerProgram,
                    FrequentFlyerNumber = row.FrequentFlyerNumber,
                    Notes = row.Notes,
                    ChangeDate = DateTime.Now,
                    PassportInformationChangeDate = DateTime.Now,
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

        public void WritePersons(IList<Person> persons)
        {
            WriteColumnHeaders();

            var rowIndex = 2;
            foreach (var person in persons)
            {
                var row = GetRow(rowIndex);
                row.Id = person.Id;
                row.Firstname = person.Firstname;
                row.Lastname = person.Lastname;
                row.Gender = person.Gender;
                row.Title = person.Title;
                row.Street1 = person.Street1;
                row.City = person.City;
                row.Plz = person.Plz;
                row.Birthdate = person.Birthdate;
                row.EmailAddress = person.EmailAddress;
                row.PhoneNumber = person.PhoneNumber;
                row.MobileNumber = person.MobileNumber;
                row.HasGeneralAbo = person.HasGeneralAbo;
                row.HasHalbtax = person.HasHalbtax;
                row.PassportSurname = person.PassportSurname;
                row.PassportGivenName = person.PassportGivenName;
                row.PassportNationality = person.PassportNationality;
                row.PassportNationalityCode = person.PassportNationalityCode;
                row.PlaceOfOrigin = person.PlaceOfOrigin;
                row.PlaceOfBirth = person.PlaceOfBirth;
                row.PassportNumber = person.PassportNumber;
                row.PassportExpirationDate = person.PassportExpirationDate;
                row.HasJuniorKarte = person.HasJuniorKarte;
                row.HasEnkelKarte = person.HasEnkelKarte;
                row.HasCancellationInsurance = person.HasCancellationInsurance;
                row.CancellationInsurance = person.CancellationInsurance;
                row.CancellationInsuranceIssueDate = person.CancellationInsuranceIssueDate;
                row.CancellationInsuranceExpirationDate = person.CancellationInsuranceExpirationDate;
                row.FrequentFlyerProgram = person.FrequentFlyerProgram;
                row.FrequentFlyerNumber = person.FrequentFlyerNumber;
                row.Notes = person.Notes;

                rowIndex++;
            }
        }

        private void WriteColumnHeaders()
        {
            _worksheet.Cells[1, 1].Value = "Id";
            _worksheet.Cells[1, 2].Value = "Vorname";
            _worksheet.Cells[1, 3].Value = "Nachname";
            _worksheet.Cells[1, 4].Value = "Geschlecht";
            _worksheet.Cells[1, 5].Value = "Titel";
            _worksheet.Cells[1, 6].Value = "Strasse";
            _worksheet.Cells[1, 7].Value = "Stadt";
            _worksheet.Cells[1, 8].Value = "PLZ";
            _worksheet.Cells[1, 9].Value = "Geburtsdatum";
            _worksheet.Cells[1, 10].Value = "E-Mail";
            _worksheet.Cells[1, 11].Value = "Tel Privat";
            _worksheet.Cells[1, 12].Value = "Tel Mobile";
            _worksheet.Cells[1, 13].Value = "GA";
            _worksheet.Cells[1, 14].Value = "HalbTax";
            _worksheet.Cells[1, 15].Value = "Nachname auf Pass";
            _worksheet.Cells[1, 16].Value = "Vorname auf Pass";
            _worksheet.Cells[1, 17].Value = "Passnummer";
            _worksheet.Cells[1, 18].Value = "Nationalität";
            _worksheet.Cells[1, 19].Value = "Nationalität-Code";
            _worksheet.Cells[1, 20].Value = "Heimatort";
            _worksheet.Cells[1, 21].Value = "Geburtsort";
            _worksheet.Cells[1, 22].Value = "Pass ausgestellt am";
            _worksheet.Cells[1, 23].Value = "Pass gültig bis";
            _worksheet.Cells[1, 24].Value = "Junior Karte";
            _worksheet.Cells[1, 25].Value = "Enkel Karte";
            _worksheet.Cells[1, 26].Value = "Hat Annullationsversicherung";
            _worksheet.Cells[1, 27].Value = "Annullationsversicherung";
            _worksheet.Cells[1, 28].Value = "Annullationsversicherung gültig von";
            _worksheet.Cells[1, 29].Value = "Annullationsversicherung gültig bis";
            _worksheet.Cells[1, 30].Value = "Vielflieger-Programm";
            _worksheet.Cells[1, 31].Value = "Vielflieger-Nummer";
            _worksheet.Cells[1, 32].Value = "Notizen";
        }

        private AddressbookWorksheetRow GetRow(int row)
        {
            return new AddressbookWorksheetRow(_worksheet, row);
        }

        private bool IsEmptyPerson(Person person)
        {
            return person.Firstname.IsNullOrEmpty() && person.Lastname.IsNullOrEmpty();
        }
    }
}
