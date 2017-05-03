using System;

using OfficeOpenXml;

using ZuegerAdressbook.Model;

namespace ZuegerAdressbook.Service
{
    public class AddressbookWorksheetRow
    {
        private const string FemaleText = "weiblich";

        private const string MaleText = "männlich";

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
                return string.IsNullOrEmpty(_worksheet.Cells[Row, 9].Text) == false ? DateTime.Parse(_worksheet.Cells[Row, 0].Text) : (DateTime?)null;
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

        public string BusinessPhoneNumber
        {
            get
            {
                return _worksheet.Cells[Row, 13].Text;
            }
            set
            {
                _worksheet.Cells[Row, 13].Value = value;
            }
        }

        public bool HasGeneralAbo
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

        public DateTime? GeneralAboExpirationDate
        {
            get
            {
                return string.IsNullOrEmpty(_worksheet.Cells[Row, 15].Text) == false ? DateTime.Parse(_worksheet.Cells[Row, 15].Text) : (DateTime?)null;
            }
            set
            {
                _worksheet.Cells[Row, 15].Value = value?.ToShortDateString() ?? string.Empty;
            }
        }

        public bool HasHalbtax
        {
            get
            {
                return _worksheet.Cells[Row, 16].Text == Ja;
            }
            set
            {
                _worksheet.Cells[Row, 16].Value = value ? Ja : Nein;
            }
        }

        public DateTime? HalbtaxExpirationDate
        {
            get
            {
                return string.IsNullOrEmpty(_worksheet.Cells[Row, 17].Text) == false ? DateTime.Parse(_worksheet.Cells[Row, 17].Text) : (DateTime?)null;
            }
            set
            {
                _worksheet.Cells[Row, 17].Value = value?.ToShortDateString() ?? string.Empty;
            }
        }

        public string NameOnPassport
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

        public string PassportNumber
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

        public bool HasJuniorKarte
        {
            get
            {
                return _worksheet.Cells[Row, 20].Text == Ja;
            }
            set
            {
                _worksheet.Cells[Row, 20].Value = value ? Ja : Nein;
            }
        }

        public DateTime? JuniorKarteExpirationDate
        {
            get
            {
                return string.IsNullOrEmpty(_worksheet.Cells[Row, 21].Text) == false ? DateTime.Parse(_worksheet.Cells[Row, 21].Text) : (DateTime?)null;
            }
            set
            {
                _worksheet.Cells[Row, 21].Value = value?.ToShortDateString() ?? string.Empty;
            }
        }

        public bool HasEnkelKarte
        {
            get
            {
                return _worksheet.Cells[Row, 22].Text == Ja;
            }
            set
            {
                _worksheet.Cells[Row, 22].Value = value ? Ja : Nein;
            }
        }

        public DateTime? EnkelKarteExpirationDate
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

        public string Notes
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
    }
}