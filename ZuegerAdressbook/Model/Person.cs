using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZuegerAdressbook.Model
{
    public enum Gender
    {
        Male,

        Female
    }

    public class Person
    {
        public long Id { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public Gender Gender { get; set; }

        public string Title { get; set; }

        public string Street1 { get; set; }

        public string Street2 { get; set; }

        public string City { get; set; }

        public string Plz { get; set; }

        public DateTime? Birthdate { get; set; }

        public string EmailAddress { get; set; }

        public string PhoneNumber { get; set; }

        public string MobileNumber { get; set; }

        public bool HasGeneralAbo { get; set; }

        public bool HasHalbtax { get; set; }

        public string Notes { get; set; }
    }
}
