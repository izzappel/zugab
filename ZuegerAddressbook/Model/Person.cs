using System;

namespace ZuegerAdressbook.Model
{
    public class Person : IIdentifiable
    {
        public string Id { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public Gender Gender { get; set; }

        public string Title { get; set; }

        public string Street1 { get; set; }

        public string City { get; set; }

        public string Plz { get; set; }

        public DateTime? Birthdate { get; set; }

        public string EmailAddress { get; set; }

        public string PhoneNumber { get; set; }

        public string MobileNumber { get; set; }

        public string BusinessPhoneNumber { get; set; }

        public bool HasGeneralAbo { get; set; }

        public bool HasHalbtax { get; set; }

        public string Notes { get; set; }

        public string NameOnPassport { get; set; }

        public string PassportNumber { get; set; }

        public DateTime? GeneralAboExpirationDate { get; set; }

        public DateTime? HalbtaxExpirationDate { get; set; }

        public bool HasJuniorKarte { get; set; }

        public DateTime? JuniorKarteExpirationDate { get; set; }

        public bool HasEnkelKarte { get; set; }

        public DateTime? EnkelKarteExpirationDate { get; set; }

        public DateTime? SbbInformationChangeDate { get; set; }

        public DateTime? ChangeDate { get; set; }
    }
}
