using System;

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
    }
}
