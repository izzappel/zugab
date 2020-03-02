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
        private bool _hasGeneralAbo;
        private bool _hasHalbtax;
        private bool _hasJuniorKarte;
        private bool _hasEnkelKarte;
        private string _notes;
        private string _passportSurname;
        private string _passportGivenName;
        private string _passportNationality;
        private string _passportNationalityCode;
        private string _placeOfOrigin;
        private string _placeOfBirth;
        private string _passportNumber;
        private DateTime? _passportIssueDate;
        private DateTime? _passportExpirationDate;
        private bool _hasCancellationInsurance;
        private string _cancellationInsurance;
        private DateTime? _canellationInsuranceIssueDate;
        private DateTime? _canellationInsuranceExpirationDate;
        private string _frequentFlyerProgram;
        private string _frequentFylerNumber;
        private DateTime? _passportInformationChangeDate;
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
            _hasGeneralAbo = person.HasGeneralAbo;
            _hasHalbtax = person.HasHalbtax;
            _hasJuniorKarte = person.HasJuniorKarte;
            _hasEnkelKarte = person.HasEnkelKarte;
            _notes = person.Notes;
            _passportSurname = person.PassportSurname;
            _passportGivenName = person.PassportGivenName;
            _passportNationality = person.PassportNationality;
            _passportNationalityCode = person.PassportNationalityCode;
            _placeOfBirth = person.PlaceOfBirth;
            _placeOfOrigin = person.PlaceOfOrigin;
            _passportNumber = person.PassportNumber;
            _passportIssueDate = person.PassportIssueDate;
            _passportExpirationDate = person.PassportExpirationDate;
            _hasCancellationInsurance = person.HasCancellationInsurance;
            _cancellationInsurance = person.CancellationInsurance;
            _canellationInsuranceIssueDate = person.CancellationInsuranceIssueDate;
            _canellationInsuranceExpirationDate = person.CancellationInsuranceExpirationDate;
            _frequentFlyerProgram = person.FrequentFlyerProgram;
            _frequentFylerNumber = person.FrequentFlyerNumber;
            _passportInformationChangeDate = person.PassportInformationChangeDate;
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

        public bool HasGeneralAbo
        {
            get { return _hasGeneralAbo; }
            set { ChangeAndNotify(value, ref _hasGeneralAbo); }
        }

        public bool HasHalbTax
        {
            get { return _hasHalbtax; }
            set { ChangeAndNotify(value, ref _hasHalbtax); }
        }

        public bool HasJuniorKarte
        {
            get { return _hasJuniorKarte; }
            set { ChangeAndNotify(value, ref _hasJuniorKarte); }
        }

        public bool HasEnkelKarte
        {
            get { return _hasEnkelKarte; }
            set { ChangeAndNotify(value, ref _hasEnkelKarte); }
        }

        public string Notes
        {
            get { return _notes; }
            set { ChangeAndNotify(value, ref _notes); }
        }

        public string PassportSurname
        {
            get { return _passportSurname; }
            set { ChangeAndNotify(value, ref _passportSurname); }
        }

        public string PassportGivenName
        {
            get { return _passportGivenName; }
            set { ChangeAndNotify(value, ref _passportGivenName); }
        }

        public string PassportNationality
        {
            get { return _passportNationality; }
            set { ChangeAndNotify(value, ref _passportNationality); }
        }

        public string PassportNationalityCode
        {
            get { return _passportNationalityCode; }
            set { ChangeAndNotify(value, ref _passportNationalityCode); }
        }

        public string PlaceOfOrigin
        {
            get { return _placeOfOrigin; }
            set { ChangeAndNotify(value, ref _placeOfOrigin); }
        }

        public string PlaceOfBirth
        {
            get { return _placeOfBirth; }
            set { ChangeAndNotify(value, ref _placeOfBirth); }
        }

        public string PassportNumber
        {
            get { return _passportNumber; }
            set { ChangeAndNotify(value, ref _passportNumber); }
        }

        public DateTime? PassportIssueDate
        {
            get { return _passportIssueDate; }
            set { ChangeAndNotify(value, ref _passportIssueDate); }
        }

        public DateTime? PassportExpirationDate
        {
            get { return _passportExpirationDate; }
            set { ChangeAndNotify(value, ref _passportExpirationDate); }
        }

        public bool HasCancellationInsurance
        {
            get { return _hasCancellationInsurance; }
            set { ChangeAndNotify(value, ref _hasCancellationInsurance); }
        }

        public string CancellationInsurance
        {
            get { return _cancellationInsurance; }
            set { ChangeAndNotify(value, ref _cancellationInsurance); }
        }

        public DateTime? CancellationInsuranceIssueDate
        {
            get { return _canellationInsuranceIssueDate; }
            set { ChangeAndNotify(value, ref _canellationInsuranceIssueDate); }
        }

        public DateTime? CancellationInsuranceExpirationDate
        {
            get { return _canellationInsuranceExpirationDate; }
            set { ChangeAndNotify(value, ref _canellationInsuranceExpirationDate); }
        }

        public string FrequentFylerProgram
        {
            get { return _frequentFlyerProgram; }
            set { ChangeAndNotify(value, ref _frequentFlyerProgram); }
        }

        public string FrequentFlyerNumber
        {
            get { return _frequentFylerNumber; }
            set { ChangeAndNotify(value, ref _frequentFylerNumber); }
        }

        public DateTime? PassportInformationChangeDate
        {
            get { return _passportInformationChangeDate; }
            set { ChangeAndNotify(value, ref _passportInformationChangeDate); }
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
            if (HavePassportInformationedChanged())
            {
                PassportInformationChangeDate = _changeDate;
            }

            HasChanges = false;

            var person = new Person
            {
                Id = _id,
                Birthdate = _birthdate,
                PassportNumber = _passportNumber,
                CancellationInsuranceIssueDate = _canellationInsuranceIssueDate,
                CancellationInsuranceExpirationDate = _canellationInsuranceExpirationDate,
                City = _city,
                EmailAddress = _emailAddress,
                Firstname = _firstname,
                Gender = _gender,
                HasCancellationInsurance = _hasCancellationInsurance,
                CancellationInsurance = _cancellationInsurance,
                HasEnkelKarte = _hasEnkelKarte,
                HasGeneralAbo = _hasGeneralAbo,
                HasHalbtax = _hasHalbtax,
                HasJuniorKarte = _hasJuniorKarte,
                Lastname = _lastname,
                MobileNumber = _mobileNumber,
                PassportSurname = _passportSurname,
                PassportGivenName = _passportGivenName,
                PassportNationality = _passportNationality,
                PassportNationalityCode = _passportNationalityCode,
                PlaceOfOrigin = _placeOfOrigin,
                PlaceOfBirth = _placeOfBirth,
                PassportIssueDate = _passportIssueDate,
                PassportExpirationDate = _passportExpirationDate,
                Notes = _notes,
                PhoneNumber = _phoneNumber,
                Plz = _plz,
                Street1 = _street1,
                Title = _title,
                FrequentFlyerProgram = _frequentFlyerProgram,
                FrequentFlyerNumber = _frequentFylerNumber,
                PassportInformationChangeDate = _passportInformationChangeDate,
                SbbInformationChangeDate = _sbbInformationChangeDate,
                ChangeDate = _changeDate,
            };

            _person = person;

            return person;
        }

        private bool HavePassportInformationedChanged()
        {
            return
                !((PassportSurname == null && _person.PassportSurname == null || PassportSurname.Equals(_person.PassportSurname))
                  && (PassportGivenName == null && _person.PassportGivenName == null || PassportGivenName.Equals(_person.PassportGivenName))
                  && (PassportNumber == null && _person.PassportNumber == null || PassportNumber.Equals(_person.PassportNumber))
                  && (PassportNationality == null && _person.PassportNationality == null || PassportNationality.Equals(_person.PassportNationality))
                  && (PassportNationalityCode == null && _person.PassportNationalityCode == null || PassportNationalityCode.Equals(_person.PassportNationalityCode))
                  && (PlaceOfBirth == null && _person.PlaceOfBirth == null || PlaceOfBirth.Equals(_person.PlaceOfBirth))
                  && (PlaceOfOrigin == null && _person.PlaceOfOrigin == null || PlaceOfOrigin.Equals(_person.PlaceOfOrigin))
                  && (PassportIssueDate == null && _person.PassportIssueDate == null || PassportIssueDate.Equals(_person.PassportIssueDate))
                  && (PassportExpirationDate == null && _person.PassportExpirationDate == null || PassportExpirationDate.Equals(_person.PassportExpirationDate)));
        }

        private bool HaveSbbInformationedChanged()
        {
            return
                !(HasEnkelKarte.Equals(_person.HasEnkelKarte) && HasJuniorKarte.Equals(_person.HasJuniorKarte) && HasGeneralAbo.Equals(_person.HasGeneralAbo)
                  && HasHalbTax.Equals(_person.HasHalbtax));
        }

        public override void ResetChanges()
        {
            Id = _person.Id;
            Birthdate = _person.Birthdate;
            PassportNumber = _person.PassportNumber;
            City = _person.City;
            EmailAddress = _person.EmailAddress;
            Firstname = _person.Firstname;
            Gender = _person.Gender;
            HasHalbTax = _person.HasHalbtax;
            HasEnkelKarte = _person.HasEnkelKarte;
            HasGeneralAbo = _person.HasGeneralAbo;
            HasJuniorKarte = _person.HasJuniorKarte;
            Lastname = _person.Lastname;
            MobileNumber = _person.MobileNumber;
            PassportSurname = _person.PassportSurname;
            PassportGivenName = _person.PassportGivenName;
            PassportNationality = _person.PassportNationality;
            PassportNationalityCode = _person.PassportNationalityCode;
            PlaceOfOrigin = _person.PlaceOfOrigin;
            PlaceOfBirth = _person.PlaceOfBirth;
            Notes = _person.Notes;
            PhoneNumber = _person.PhoneNumber;
            PassportIssueDate = _person.PassportIssueDate;
            PassportExpirationDate = _person.PassportExpirationDate;
            HasCancellationInsurance = _person.HasCancellationInsurance;
            CancellationInsuranceExpirationDate = _person.CancellationInsuranceExpirationDate;
            Plz = _person.Plz;
            Street1 = _person.Street1;
            Title = _person.Title;
            FrequentFylerProgram = _person.FrequentFlyerProgram;
            FrequentFlyerNumber = _person.FrequentFlyerNumber;
            PassportInformationChangeDate = _person.PassportInformationChangeDate;
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