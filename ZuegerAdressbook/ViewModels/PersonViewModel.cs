using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Raven.Imports.Newtonsoft.Json;
using ZuegerAdressbook.Model;

namespace ZuegerAdressbook.ViewModels
{
    public class PersonViewModel : ViewModelBase
    {
        private Person _person;

        private bool _hasChanges;

        private string _id;

        private string _firstname;

        private string _lastname;

        private Gender _gender;

        private string _title;

        private string _street1;

        private string _street2;

        private string _city;

        private string _plz;

        private DateTime? _birthdate;

        private string _emailAddress;

        private string _phoneNumber;

        private string _mobileNumber;

        private bool _hasGeneralAbo;

        private DateTime? _generalAboExpirationDate;

        private bool _hasHalbtax;

        private DateTime? _halbtaxExpirationDate;

        private bool _hasJuniorKarte;

        private DateTime? _juniorKarteExpirationDate;

        private bool _hasEnkelKarte;

        private DateTime? _enkelKarteExpirationDate;

        private string _notes;

        private string _nameOnPassport;

        private string _passportNumber;

        private IList<string> _documents;

        private MainViewModel _parent;

        public PersonViewModel(MainViewModel parent = null)
        {
            _person = new Person();
            _parent = parent;
        }

        public PersonViewModel(Person person, MainViewModel parent)
        {
            CopyFromEntity(person);
            _person = person;
            _parent = parent;

            // TODO: not sure wheter we should use the properties or not
        }

        private void CopyFromEntity(Person person)
        {
            _id = person.Id;
            _firstname = person.Firstname;
            _lastname = person.Lastname;
            _gender = person.Gender;
            _title = person.Title;
            _street1 = person.Street1;
            _street2 = person.Street2;
            _city = person.City;
            _plz = person.Plz;
            _birthdate = person.Birthdate;
            _emailAddress = person.EmailAddress;
            _phoneNumber = person.PhoneNumber;
            _mobileNumber = person.MobileNumber;
            _hasGeneralAbo = person.HasGeneralAbo;
            _generalAboExpirationDate = person.GeneralAboExpirationDate;
            _hasHalbtax = person.HasHalbtax;
            _halbtaxExpirationDate = person.HalbtaxExpirationDate;
            _hasJuniorKarte = person.HasJuniorKarte;
            _juniorKarteExpirationDate = person.JuniorKarteExpirationDate;
            _hasEnkelKarte = person.HasEnkelKarte;
            _enkelKarteExpirationDate = person.EnkelKarteExpirationDate;
            _notes = person.Notes;
            _nameOnPassport = person.NameOnPassport;
            _passportNumber = person.PassportNumber;
        }

        public bool HasChanges
        {
            get { return _hasChanges; }
            set { ChangeAndNotify(value, ref _hasChanges); }
        }

        public string Id
        {
            get { return _id; }
            set { ChangeAndNotify(value, ref _id); }
        }

        public string Firstname
        {
            get { return _firstname; }
            set
            {
                ChangeAndNotify(value, ref _firstname);
                Notify("FullName");
            }
        }

        public string Lastname
        {
            get { return _lastname; }
            set
            {
                ChangeAndNotify(value, ref _lastname);
                Notify("FullName");
            }
        }

        public Gender Gender
        {
            get { return _gender; }
            set { ChangeAndNotify(value, ref _gender); }
        }

        public string Title
        {
            get { return _title; }
            set { ChangeAndNotify(value, ref _title); }
        }

        public string Street1
        {
            get { return _street1; }
            set { ChangeAndNotify(value, ref _street1); }
        }

        public string Street2
        {
            get { return _street2; }
            set { ChangeAndNotify(value, ref _street2); }
        }

        public string City
        {
            get { return _city; }
            set { ChangeAndNotify(value, ref _city); }
        }

        public string Plz
        {
            get { return _plz; }
            set { ChangeAndNotify(value, ref _plz); }
        }

        public DateTime? Birthdate
        {
            get { return _birthdate; }
            set
            {
                ChangeAndNotify(value, ref _birthdate);
                Notify("Age");
                Notify("IsChild");
            }
        }

        public string EmailAddress
        {
            get { return _emailAddress; }
            set { ChangeAndNotify(value, ref _emailAddress); }
        }

        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set { ChangeAndNotify(value, ref _phoneNumber); }
        }

        public string MobileNumber
        {
            get { return _mobileNumber; }
            set { ChangeAndNotify(value, ref _mobileNumber); }
        }

        public bool HasGeneralAbo
        {
            get { return _hasGeneralAbo; }
            set { ChangeAndNotify(value, ref _hasGeneralAbo); }
        }

        public DateTime? GeneralAboExpirationDate
        {
            get { return _generalAboExpirationDate; }
            set { ChangeAndNotify(value, ref _generalAboExpirationDate); }
        }

        public bool HasHalbTax
        {
            get { return _hasHalbtax; }
            set { ChangeAndNotify(value, ref _hasHalbtax); }
        }

        public DateTime? HalbTaxExpirationDate
        {
            get { return _halbtaxExpirationDate; }
            set { ChangeAndNotify(value, ref _halbtaxExpirationDate); }
        }

        public bool HasJuniorKarte
        {
            get { return _hasJuniorKarte; }
            set { ChangeAndNotify(value, ref _hasJuniorKarte); }
        }

        public DateTime? JuniorKarteExpirationDate
        {
            get { return _juniorKarteExpirationDate; }
            set { ChangeAndNotify(value, ref _juniorKarteExpirationDate); }
        }

        public bool HasEnkelKarte
        {
            get { return _hasEnkelKarte; }
            set { ChangeAndNotify(value, ref _hasEnkelKarte); }
        }

        public DateTime? EnkelKarteExpirationDate
        {
            get { return _enkelKarteExpirationDate; }
            set { ChangeAndNotify(value, ref _enkelKarteExpirationDate); }
        }

        public string Notes
        {
            get { return _notes; }
            set { ChangeAndNotify(value, ref _notes); }
        }

        public string NameOnPassport
        {
            get { return _nameOnPassport; }
            set { ChangeAndNotify(value, ref _nameOnPassport); }
        }

        public string PassportNumber
        {
            get { return _passportNumber; }
            set { ChangeAndNotify(value, ref _passportNumber); }
        }

        public Person AcceptChanges()
        {
            HasChanges = false;

            var person = new Person
            {
                Id = _id,
                Birthdate = _birthdate,
                PassportNumber = _passportNumber,
                City = _city,
                EmailAddress = _emailAddress,
                EnkelKarteExpirationDate = EnkelKarteExpirationDate,
                Firstname = _firstname,
                Gender = _gender,
                GeneralAboExpirationDate = _generalAboExpirationDate,
                HalbtaxExpirationDate = _halbtaxExpirationDate,
                HasEnkelKarte = _hasEnkelKarte,
                HasGeneralAbo = _hasGeneralAbo,
                HasHalbtax = _hasHalbtax,
                HasJuniorKarte = _hasJuniorKarte,
                JuniorKarteExpirationDate = _juniorKarteExpirationDate,
                Lastname = _lastname,
                MobileNumber = _mobileNumber,
                NameOnPassport = _nameOnPassport,
                Notes = _notes,
                PhoneNumber = _phoneNumber,
                Plz = _plz,
                Street1 = _street1,
                Street2 = _street2,
                Title = _title
            };

            _person = person;

            return person;
        }

        public void ResetChanges()
        {
            Id = _person.Id;
            Birthdate = _person.Birthdate;
            PassportNumber = _person.PassportNumber;
            City = _person.City;
            EmailAddress = _person.EmailAddress;
            EnkelKarteExpirationDate = _person.EnkelKarteExpirationDate;
            Firstname = _person.Firstname;
            Gender = _person.Gender;
            GeneralAboExpirationDate = _person.GeneralAboExpirationDate;
            HalbTaxExpirationDate = _person.HalbtaxExpirationDate;
            HasEnkelKarte = _person.HasEnkelKarte;
            HasGeneralAbo = _person.HasGeneralAbo;
            HasJuniorKarte = _person.HasJuniorKarte;
            JuniorKarteExpirationDate = _person.JuniorKarteExpirationDate;
            Lastname = _person.Lastname;
            MobileNumber = _person.MobileNumber;
            NameOnPassport = _person.NameOnPassport;
            Notes = _person.Notes;
            PhoneNumber = _person.PhoneNumber;
            Plz = _person.Plz;
            Street1 = _person.Street1;
            Street2 = _person.Street2;
            Title = _person.Title;

            HasChanges = false;
        }

        protected override bool ChangeAndNotify<T>(T value, ref T field, [CallerMemberName] string propertyName = null)
        {
            // Note: we should extract this into a superclass ChangeTrackingViewModel if needed for further entities
            var hasChanged = base.ChangeAndNotify(value, ref field, propertyName);

            if (hasChanged)
            {
                if (propertyName != "HasChanges")
                {
                    HasChanges = true;
                }

                _parent.SaveCommand.RaiseCanExecuteChanged();
                _parent.DeleteCommand.RaiseCanExecuteChanged();
                _parent.RevertCommand.RaiseCanExecuteChanged();
            }

            return hasChanged;
        }

        [JsonIgnore]
        public int? Age
        {
            get
            {
                if (Birthdate.HasValue == false)
                {
                    return null;
                }

                var today = DateTime.Today;
                var age = today.Year - Birthdate.Value.Year;

                if (today < Birthdate.Value.AddYears(age).Date)
                {
                    age--;
                }

                return age;
            }
        }

        [JsonIgnore]
        public bool? IsChild
        {
            get
            {
                if (Age.HasValue == false)
                {
                    return null;
                }

                return Age <= 16;
            }
        }

        [JsonIgnore]
        public string FullName
        {
            get { return (this.Firstname + " " + this.Lastname).Trim(); }
        }
    }
}