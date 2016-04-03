using Raven.Imports.Newtonsoft.Json;
using System;

namespace ZuegerAdressbook.Model
{
    public enum Gender
    {
        Male,

        Female
    }

    public class Person
    {
        public string Id { get; set; }

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

		[JsonIgnore]
		public string FullName 
		{
			get { return (this.Firstname + " " + this.Lastname).Trim(); }
		}

        public static string GenerateId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
