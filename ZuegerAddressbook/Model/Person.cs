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

        public bool HasGeneralAbo { get; set; }

        public bool HasHalbtax { get; set; }

        public string Notes { get; set; }

        public string PassportSurname { get; set; }

        public string PassportGivenName { get; set; }

        public string PassportNumber { get; set; }

        public string PassportNationality { get; set; }

        public string PassportNationalityCode { get; set; }

        public string PlaceOfIssue { get; set; }

        public string PlaceOfOrigin { get; set; }

        public string PlaceOfBirth { get; set; }

        public DateTime? PassportIssueDate { get; set; }

        public DateTime? PassportExpirationDate { get; set; }

        public bool HasJuniorKarte { get; set; }

        public bool HasEnkelKarte { get; set; }

        public string CancellationInsurance { get; set; }

        public DateTime? CancellationInsuranceIssueDate { get; set; }

        public DateTime? CancellationInsuranceExpirationDate { get; set; }

        public string FrequentFlyerProgram { get; set; }

        public string FrequentFlyerNumber { get; set; }

        public DateTime? PassportInformationChangeDate { get; set; }

        public DateTime? SbbInformationChangeDate { get; set; }

        public DateTime? ChangeDate { get; set; }
    }
}
