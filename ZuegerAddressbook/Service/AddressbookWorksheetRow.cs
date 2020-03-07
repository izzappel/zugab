using System;

using OfficeOpenXml;

using ZuegerAdressbook.Model;

namespace ZuegerAdressbook.Service
{
    public class AddressbookWorksheetRow
    {
        private const string FemaleText = "Frau";

        private const string MaleText = "Mann";

        private const string Ja = "Ja";

        private const string Nein = "Nein";

        private readonly ExcelWorksheet _worksheet;

        public AddressbookWorksheetRow(ExcelWorksheet worksheet, int row)
        {
            _worksheet = worksheet;
            Row = row;
        }

        public int Row { get; set; }

        public string Id
        {
            get
            {
                return _worksheet.Cells[Row, 1].Text;
            }
            set
            {
                _worksheet.Cells[Row, 1].Value = value;
            }
        }

        public string Firstname
        {
            get
            {
                return _worksheet.Cells[Row, 2].Text;
            }
            set
            {
                _worksheet.Cells[Row, 2].Value = value;
            }
        }

        public string Lastname
        {
            get
            {
                return _worksheet.Cells[Row, 3].Text;
            }
            set
            {
                _worksheet.Cells[Row, 3].Value = value;
            }
        }

        public Gender Gender
        {
            get
            {
                return _worksheet.Cells[Row, 4].Text == FemaleText ? Gender.Female : Gender.Male;
            }
            set
            {
                _worksheet.Cells[Row, 4].Value = value == Gender.Female ? FemaleText : MaleText;
            }
        }

        public string Title
        {
            get
            {
                return _worksheet.Cells[Row, 5].Text;
            }
            set
            {
                _worksheet.Cells[Row, 5].Value = value;
            }
        }

        public string Street1
        {
            get
            {
                return _worksheet.Cells[Row, 6].Text;
            }
            set
            {
                _worksheet.Cells[Row, 6].Value = value;
            }
        }

        public string City
        {
            get
            {
                return _worksheet.Cells[Row, 7].Text;
            }
            set
            {
                _worksheet.Cells[Row, 7].Value = value;
            }
        }

        public string Plz
        {
            get
            {
                return _worksheet.Cells[Row, 8].Text;
            }
            set
            {
                _worksheet.Cells[Row, 8].Value = value;
            }
        }

        public DateTime? Birthdate
        {
            get
            {
                return string.IsNullOrEmpty(_worksheet.Cells[Row, 9].Text) == false ? DateTime.Parse(_worksheet.Cells[Row, 9].Text) : (DateTime?)null;
            }
            set
            {
                _worksheet.Cells[Row, 9].Value = value?.ToShortDateString() ?? string.Empty;
            }
        }

        public string EmailAddress
        {
            get
            {
                return _worksheet.Cells[Row, 10].Text;
            }
            set
            {
                _worksheet.Cells[Row, 10].Value = value;
            }
        }

        public string PhoneNumber
        {
            get
            {
                return _worksheet.Cells[Row, 11].Text;
            }
            set
            {
                _worksheet.Cells[Row, 11].Value = value;
            }
        }

        public string MobileNumber
        {
            get
            {
                return _worksheet.Cells[Row, 12].Text;
            }
            set
            {
                _worksheet.Cells[Row, 12].Value = value;
            }
        }

        public bool HasGeneralAbo
        {
            get
            {
                return _worksheet.Cells[Row, 13].Text == Ja;
            }
            set
            {
                _worksheet.Cells[Row, 13].Value = value ? Ja : Nein;
            }
        }

        public bool HasHalbtax
        {
            get
            {
                return _worksheet.Cells[Row, 14].Text == Ja;
            }
            set
            {
                _worksheet.Cells[Row, 14].Value = value ? Ja : Nein;
            }
        }

        public string PassportSurname
        {
            get
            {
                return _worksheet.Cells[Row, 15].Text;
            }
            set
            {
                _worksheet.Cells[Row, 15].Value = value;
            }
        }

        public string PassportGivenName
        {
            get
            {
                return _worksheet.Cells[Row, 16].Text;
            }
            set
            {
                _worksheet.Cells[Row, 16].Value = value;
            }
        }

        public string PassportNumber
        {
            get
            {
                return _worksheet.Cells[Row, 17].Text;
            }
            set
            {
                _worksheet.Cells[Row, 17].Value = value;
            }
        }

        public string PassportNationality
        {
            get
            {
                return _worksheet.Cells[Row, 18].Text;
            }
            set
            {
                _worksheet.Cells[Row, 18].Value = value;
            }
        }

        public string PassportNationalityCode
        {
            get
            {
                return _worksheet.Cells[Row, 19].Text;
            }
            set
            {
                _worksheet.Cells[Row, 19].Value = value;
            }
        }

        public string PlaceOfOrigin
        {
            get
            {
                return _worksheet.Cells[Row, 20].Text;
            }
            set
            {
                _worksheet.Cells[Row, 20].Value = value;
            }
        }

        public string PlaceOfBirth
        {
            get
            {
                return _worksheet.Cells[Row, 21].Text;
            }
            set
            {
                _worksheet.Cells[Row, 21].Value = value;
            }
        }

        public DateTime? PassportIssueDate
        {
            get
            {
                return string.IsNullOrEmpty(_worksheet.Cells[Row, 22].Text) == false ? DateTime.Parse(_worksheet.Cells[Row, 22].Text) : (DateTime?)null;
            }
            set
            {
                _worksheet.Cells[Row, 22].Value = value?.ToShortDateString() ?? string.Empty;
            }
        }

        public DateTime? PassportExpirationDate
        {
            get
            {
                return string.IsNullOrEmpty(_worksheet.Cells[Row, 23].Text) == false ? DateTime.Parse(_worksheet.Cells[Row, 23].Text) : (DateTime?)null;
            }
            set
            {
                _worksheet.Cells[Row, 23].Value = value?.ToShortDateString() ?? string.Empty;
            }
        }

        public string PlaceOfIssue
        {
            get
            {
                return _worksheet.Cells[Row, 24].Text;
            }
            set
            {
                _worksheet.Cells[Row, 24].Value = value;
            }
        }

        public bool HasJuniorKarte
        {
            get
            {
                return _worksheet.Cells[Row, 25].Text == Ja;
            }
            set
            {
                _worksheet.Cells[Row, 25].Value = value ? Ja : Nein;
            }
        }

        public bool HasEnkelKarte
        {
            get
            {
                return _worksheet.Cells[Row, 26].Text == Ja;
            }
            set
            {
                _worksheet.Cells[Row, 26].Value = value ? Ja : Nein;
            }
        }

        public string CancellationInsurance
        {
            get
            {
                return _worksheet.Cells[Row, 27].Text;
            }
            set
            {
                _worksheet.Cells[Row, 27].Value = value;
            }
        }

        public DateTime? CancellationInsuranceIssueDate
        {
            get
            {
                return string.IsNullOrEmpty(_worksheet.Cells[Row, 28].Text) == false ? DateTime.Parse(_worksheet.Cells[Row, 28].Text) : (DateTime?)null;
            }
            set
            {
                _worksheet.Cells[Row, 28].Value = value?.ToShortDateString() ?? string.Empty;
            }
        }

        public DateTime? CancellationInsuranceExpirationDate
        {
            get
            {
                return string.IsNullOrEmpty(_worksheet.Cells[Row, 29].Text) == false ? DateTime.Parse(_worksheet.Cells[Row, 29].Text) : (DateTime?)null;
            }
            set
            {
                _worksheet.Cells[Row, 29].Value = value?.ToShortDateString() ?? string.Empty;
            }
        }

        public string FrequentFlyerProgram
        {
            get
            {
                return _worksheet.Cells[Row, 30].Text;
            }
            set
            {
                _worksheet.Cells[Row, 30].Value = value;
            }
        }

        public string FrequentFlyerNumber
        {
            get
            {
                return _worksheet.Cells[Row, 31].Text;
            }
            set
            {
                _worksheet.Cells[Row, 31].Value = value;
            }
        }

        public string Notes
        {
            get
            {
                return _worksheet.Cells[Row, 32].Text;
            }
            set
            {
                _worksheet.Cells[Row, 32].Value = value;
            }
        }
    }
}