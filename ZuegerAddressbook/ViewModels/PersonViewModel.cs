using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

using NLog;

using Raven.Imports.Newtonsoft.Json;

using ZuegerAdressbook.Commands;
using ZuegerAdressbook.Extensions;
using ZuegerAdressbook.Model;
using ZuegerAdressbook.Service;

namespace ZuegerAdressbook.ViewModels
{
    public class PersonViewModel : RevertableViewModelBase<Person>, IChangeListener
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IMessageDialogService _messageDialogService;

        private readonly IDocumentStoreFactory _documentStoreFactory;

        private Person _person;
        private bool _hasChanges;
        private string _id;
        private string _firstname;
        private string _lastname;
        private Gender _gender;
        private string _title;
        private string _street1;
        private string _city;
        private string _plz;
        private DateTime? _birthdate;
        private string _emailAddress;
        private string _phoneNumber;
        private string _mobileNumber;
        private string _businessPhoneNumber;
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
        private DateTime? _sbbInformationChangeDate;
        private DateTime? _changeDate;
        private IChangeListener _parent;
        private RevertableObservableCollection<DocumentViewModel, Document> _documents;

        public PersonViewModel(IMessageDialogService messageDialogService, IDocumentStoreFactory documentStoreFactory, Person person, IChangeListener parent)
        {
            if (person == null)
            {
                person = new Person();
            }

            _documentStoreFactory = documentStoreFactory;
            _messageDialogService = messageDialogService;
            _person = person;
            _parent = parent;

            CopyFromEntity(person);

            AddDocumentCommand = new RelayCommand(AddDocument, CanAddDocument);
            RemoveDocumentsCommand = new RelayCommand(RemoveDocuments, CanRemoveDocuments);
        }

        private void CopyFromEntity(Person person)
        {
            _id = person.Id;
            _firstname = person.Firstname;
            _lastname = person.Lastname;
            _gender = person.Gender;
            _title = person.Title;
            _street1 = person.Street1;
            _city = person.City;
            _plz = person.Plz;
            _birthdate = person.Birthdate;
            _emailAddress = person.EmailAddress;
            _phoneNumber = person.PhoneNumber;
            _mobileNumber = person.MobileNumber;
            _businessPhoneNumber = person.BusinessPhoneNumber;
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
            _sbbInformationChangeDate = person.SbbInformationChangeDate;
            _changeDate = person.ChangeDate;

            LoadDocuments();
        }

        private void LoadDocuments()
        {
            _documents = new RevertableObservableCollection<DocumentViewModel, Document>(this);

            if (!IsNew)
            {
                using (var session = _documentStoreFactory.CreateDocumentStore().OpenSession())
                {
                    var documents = session.Query<Document>().Where(t => t.PersonId == _id).ToList();
                    var documentViewModels = documents.Select(s => IocKernel.GetDocumentViewModel(this, s)).ToList();

                    _documents = new RevertableObservableCollection<DocumentViewModel, Document>(documentViewModels, this);
                }
            }
        }

        private bool IsNew => _id.IsNullOrEmpty();

        #region Properties
        public override bool HasChanges
        {
            get { return _hasChanges || Documents.Any(t => t.HasChanges) || _documents.HasChanges; }
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

        public string BusinessPhoneNumber
        {
            get { return _businessPhoneNumber; }
            set { ChangeAndNotify(value, ref _businessPhoneNumber); }
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

        public DateTime? SbbInformationChangeDate
        {
            get { return _sbbInformationChangeDate; }
            set { ChangeAndNotify(value, ref _sbbInformationChangeDate); }
        }

        public DateTime? ChangeDate
        {
            get { return _changeDate; }
            set { ChangeAndNotify(value, ref _changeDate); }
        }

        public RevertableObservableCollection<DocumentViewModel, Document> Documents
        {
            get { return _documents; }
            set
            {
                ChangeAndNotify(value, ref _documents);
            }
        }

        public RelayCommand AddDocumentCommand { get; set; }

        public RelayCommand RemoveDocumentsCommand { get; set; }

        #endregion

        public override Person AcceptChanges()
        {
            ChangeDate = DateTime.Now;
            if (HaveSbbInformationedChanged())
            {
                SbbInformationChangeDate = _changeDate;
            }

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
                BusinessPhoneNumber = _businessPhoneNumber,
                NameOnPassport = _nameOnPassport,
                Notes = _notes,
                PhoneNumber = _phoneNumber,
                Plz = _plz,
                Street1 = _street1,
                Title = _title,
                SbbInformationChangeDate = _sbbInformationChangeDate,
                ChangeDate = _changeDate
            };

            _person = person;

            return person;
        }

        private bool HaveSbbInformationedChanged()
        {
            return
                !(EnkelKarteExpirationDate.Equals(_person.EnkelKarteExpirationDate) && JuniorKarteExpirationDate.Equals(_person.JuniorKarteExpirationDate)
                  && GeneralAboExpirationDate.Equals(_person.GeneralAboExpirationDate) && HalbTaxExpirationDate.Equals(_person.HalbtaxExpirationDate)
                  && HasEnkelKarte.Equals(_person.HasEnkelKarte) && HasJuniorKarte.Equals(_person.HasJuniorKarte) && HasGeneralAbo.Equals(_person.HasGeneralAbo)
                  && HasHalbTax.Equals(_person.HasHalbtax));
        }

        public override void ResetChanges()
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
            HasHalbTax = _person.HasHalbtax;
            HasEnkelKarte = _person.HasEnkelKarte;
            HasGeneralAbo = _person.HasGeneralAbo;
            HasJuniorKarte = _person.HasJuniorKarte;
            JuniorKarteExpirationDate = _person.JuniorKarteExpirationDate;
            Lastname = _person.Lastname;
            MobileNumber = _person.MobileNumber;
            NameOnPassport = _person.NameOnPassport;
            Notes = _person.Notes;
            PhoneNumber = _person.PhoneNumber;
            BusinessPhoneNumber = _person.BusinessPhoneNumber;
            Plz = _person.Plz;
            Street1 = _person.Street1;
            Title = _person.Title;
            SbbInformationChangeDate = _person.SbbInformationChangeDate;
            ChangeDate = _person.ChangeDate;

            HasChanges = false;

            Documents.ResetChanges();
        }

        public void SaveDocuments()
        {
            foreach (var documentViewModel in Documents)
            {
                documentViewModel.PersonId = _id;
            }

            var documents = Documents.AcceptChanges();

            using (var session = _documentStoreFactory.CreateDocumentStore().OpenSession())
            {
                foreach (var documentViewModel in documents)
                {
                    var document = documentViewModel.AcceptChanges();
                    session.Store(document);
                    documentViewModel.Id = document.Id;
                }

                var documentsToDelete = session.Query<Document>().Where(t => t.PersonId == _id).ToList().Where(t => documents.Any(d => d.Id == t.Id) == false);
                foreach (var document in documentsToDelete)
                {
                    session.Delete(document.Id);
                }

                session.SaveChanges();
            }
        }

        public void DeleteDocuments()
        {
            Documents.ResetChanges();

            if (!IsNew)
            {
                using (var session = _documentStoreFactory.CreateDocumentStore().OpenSession())
                {
                    foreach (var t in Documents)
                    {
                        session.Delete(t.Id);
                    }

                    session.SaveChanges();
                }

                _logger.Info(LoggerMessage.GetFunctionUsageMessage("Delete Documents"));
            }
        }

        public void CheckDocuments()
        {
            foreach (var documentViewModel in Documents)
            {
                documentViewModel.NotExists = !File.Exists(documentViewModel.FileName);
            }
        }

        private bool CanAddDocument()
        {
            return true;
        }

        private void AddDocument()
        {
            var filename = _messageDialogService.OpenFileDialog();
            if (filename.IsNullOrEmpty() == false)
            {
                var documentViewModel = IocKernel.GetDocumentViewModel(this);
                documentViewModel.FileName = filename;
                documentViewModel.PersonId = Id;

                Documents.Add(documentViewModel);

                _logger.Info(LoggerMessage.GetFunctionUsageMessage("Add Document"));
            }

            RemoveDocumentsCommand.RaiseCanExecuteChanged();
        }

        private bool CanRemoveDocuments()
        {
            return Documents.Any(t => t.IsSelected);
        }

        private void RemoveDocuments()
        {
            var toDelete = Documents.Where(t => t.IsSelected).ToList();
            toDelete.ForEach(t => Documents.Remove(t));

            _logger.Info(LoggerMessage.GetFunctionUsageMessage("Remove Document"));

            RemoveDocumentsCommand.RaiseCanExecuteChanged();
        }

        protected override bool ChangeAndNotify<T>(T value, ref T field, [CallerMemberName] string propertyName = null)
        {
            // Note: we should extract this into a superclass ChangeTrackingViewModel if needed for further entities
            var hasChanged = base.ChangeAndNotify(value, ref field, propertyName);

            if (hasChanged)
            {
                if (propertyName != "HasChanges" && propertyName != "Id") // Id will never be set from UI and should never trigger the change tracking
                {
                    HasChanges = true;
                }

                _parent?.ReportChange();
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

        public void ReportChange()
        {
            _parent.ReportChange();

            Notify("HasChanges");
            AddDocumentCommand.RaiseCanExecuteChanged();
            RemoveDocumentsCommand.RaiseCanExecuteChanged();
        }
    }
}