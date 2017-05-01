using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using NLog;

using ZuegerAdressbook.Commands;
using ZuegerAdressbook.Extensions;
using ZuegerAdressbook.Model;
using ZuegerAdressbook.Service;
using ZuegerAdressbook.View;

namespace ZuegerAdressbook.ViewModels
{
    public class MainViewModel : ViewModelBase, INotifyPropertyChanged, IChangeListener
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IDocumentStoreFactory _documentStoreFactory;

        private readonly IDispatcher _dispatcher;

        private readonly IMessageDialogService _messageDialogService;

        private readonly IExcelImportService _excelImportService;

        private readonly IExcelExportService _excelExportService;

        private bool IsNewModeActive => SelectedDetailedPerson != null && SelectedDetailedPerson.Id.IsNullOrEmpty();

        private PersonViewModel _selectedListPerson;

        private PersonViewModel _selectedDetailedPerson;

        private RevertableObservableCollection<DocumentViewModel, Document> _documents;

        private bool _isFilterByBirthdate = false;

        public bool HasChanges
        {
            get
            {
                return (SelectedDetailedPerson != null && SelectedDetailedPerson.HasChanges);
            }
        }

        public PersonViewModel SelectedListPerson
        {
            get
            {
                return _selectedListPerson;
            }
            set
            {
                if (Equals(value, _selectedListPerson))
                {
                    return;
                }

                var origValue = _selectedListPerson;

                _selectedListPerson = value;

                if (ChangeSelectedDetailedPerson() == false)
                {
                    _dispatcher.Dispatch(new Action(
                            () =>
                            {
                                // Do this against the underlying value so 
                                //  that we don't invoke the cancellation question again.
                                _selectedListPerson = origValue;
                                OnPropertyChanged();
                            }));
                }
                else
                {
                    _selectedListPerson = value;
                    OnPropertyChanged();
                }
            }
        }

        public PersonViewModel SelectedDetailedPerson
        {
            get { return _selectedDetailedPerson; }
            set
            {
                ChangeAndNotify(value, ref _selectedDetailedPerson);
                OnSelectedDetailedPersonChanged();
            }
        }

        public bool IsFilterByBirthdate
        {
            get
            {
                return _isFilterByBirthdate;
            }
            set
            {
                ChangeAndNotify(value, ref _isFilterByBirthdate);
            }
        }

        public ObservableCollection<PersonViewModel> Persons { get; set; }

        public RelayCommand NewCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand RevertCommand { get; set; }
        public RelayCommand ImportCommand { get; set; }
        public RelayCommand ExportCommand { get; set; }
        public RelayCommand PrintPersonDetailCommand { get; set; }
        public RelayCommand ToggleFilter { get; set; }


        public MainViewModel()
        {
        }

        public MainViewModel(IDocumentStoreFactory documentStoreFactory, IDispatcher dispatcher, IMessageDialogService messageDialogService, IExcelImportService excelImportService, IExcelExportService excelExportService)
        {
            _documentStoreFactory = documentStoreFactory;
            _dispatcher = dispatcher;
            _messageDialogService = messageDialogService;
            _excelImportService = excelImportService;
            _excelExportService = excelExportService;

            NewCommand = new RelayCommand(CreateNewPerson);
            SaveCommand = new RelayCommand(SaveSelectedPerson, CanSaveSelectedPerson);
            DeleteCommand = new RelayCommand(DeleteSelectedPerson, CanDeleteSelectedPerson);
            RevertCommand = new RelayCommand(RevertChanges, CanRevertChanges);
            ImportCommand = new RelayCommand(ImportPersons);
            ExportCommand = new RelayCommand(ExportPersons);
            PrintPersonDetailCommand = new RelayCommand(PrintPersonDetail);
            ToggleFilter = new RelayCommand(TogglePersonsFilter, CanTogglePersonsFilter);

            InitializePersons();
        }

        private void InitializePersons()
        {
            using (var session = _documentStoreFactory.CreateDocumentStore().OpenSession())
            {
                var persons = session.LoadAll<Person>();
                Persons = new ObservableCollection<PersonViewModel>(persons.OrderBy(t => t.Lastname).ThenBy(t => t.Firstname).Select(s => IocKernel.GetPersonViewModel(this, s)).ToList());
            }

            Notify("Persons");

            SelectedListPerson = Persons.FirstOrDefault();
        }

        private void OnSelectedDetailedPersonChanged()
        {
            Notify("HasChanges");

            SaveCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
            RevertCommand.RaiseCanExecuteChanged();

            SelectedDetailedPerson?.CheckDocuments();
        }

        private void CreateNewPerson()
        {
            var canChangeSelectedDetaiedPerson = true;

            if (IsNewModeActive || HasChanges)
            {
                canChangeSelectedDetaiedPerson = _messageDialogService.OpenConfirmationDialog("Änderungen verwerfen", "Wollen Sie die Änderungen verwerfen?");
                if (canChangeSelectedDetaiedPerson)
                {
                    SelectedDetailedPerson.ResetChanges();
                }
            }

            if (canChangeSelectedDetaiedPerson)
            {
                SelectedListPerson = null;
                SelectedDetailedPerson = IocKernel.GetPersonViewModel(this);

                _logger.Info(LoggerMessage.GetFunctionUsageMessage("Create New Person"));
            }
        }

        private bool CanSaveSelectedPerson()
        {
            return IsNewModeActive || HasChanges;
        }

        private void SaveSelectedPerson()
        {
            if (SelectedDetailedPerson == null)
            {
                throw new ArgumentException("SelectedDetailedPerson must not be null");
            }

            if (IsNewModeActive)
            {
                Persons.Add(SelectedDetailedPerson);
            }

            var entity = SelectedDetailedPerson.AcceptChanges();

            using (var session = _documentStoreFactory.CreateDocumentStore().OpenSession())
            {
                session.Store(entity);
                session.SaveChanges();
            }

            SelectedDetailedPerson.Id = entity.Id;

            SelectedDetailedPerson.SaveDocuments();

            SelectedListPerson = SelectedDetailedPerson;

            _logger.Info(LoggerMessage.GetFunctionUsageMessage("Save Selected Person"));
        }

        private bool CanDeleteSelectedPerson()
        {
            return SelectedListPerson != null && !IsNewModeActive;
        }

        private void DeleteSelectedPerson()
        {
            if (_messageDialogService.OpenConfirmationDialog("Löschen", $"Wollen Sie '{SelectedDetailedPerson.Firstname} {SelectedDetailedPerson.Lastname}' wirklich löschen?"))
            {
                var toDelete = SelectedDetailedPerson;
                Persons.Remove(SelectedDetailedPerson);
                SelectedDetailedPerson = Persons.FirstOrDefault();
                SelectedListPerson = Persons.FirstOrDefault();

                toDelete.DeleteDocuments();

                if (!IsNewModeActive)
                {
                    using (var session = _documentStoreFactory.CreateDocumentStore().OpenSession())
                    {
                        session.Delete(toDelete.Id);
                        session.SaveChanges();
                    }

                    _logger.Info(LoggerMessage.GetFunctionUsageMessage("Delete Selected Person"));
                }
            }
        }

        private bool CanRevertChanges()
        {
            return IsNewModeActive || HasChanges;
        }

        private void RevertChanges()
        {
            if (_messageDialogService.OpenConfirmationDialog("Änderungen verwerfen", "Wollen Sie die Änderungen verwerfen?"))
            {
                SelectedDetailedPerson?.ResetChanges();

                if (IsNewModeActive)
                {
                    SelectedDetailedPerson = null;
                    SelectedListPerson = Persons.FirstOrDefault();

                    _logger.Info(LoggerMessage.GetFunctionUsageMessage("Revert Changes on New Mode"));
                }
                else
                {
                    _logger.Info(LoggerMessage.GetFunctionUsageMessage("Revert Changes on Edit Mode"));
                }

            }
        }

        private void ImportPersons()
        {
            var filename = _messageDialogService.OpenExcelFileDialog();
            if (filename.IsNullOrEmpty() == false)
            {
                try
                {
                    var numberOfImportedPersons = _excelImportService.Import(filename);
                    _messageDialogService.OpenInformationDialog("Erfolgreich Importiert", $"{numberOfImportedPersons} Personen wurden importiert.");

                    InitializePersons();
                }
                catch (Exception)
                {
                    _messageDialogService.OpenErrorDialog("Fehler beim Importieren", "Die Datei konnte nicht importiert werden.");
                }
            }
        }

        private void ExportPersons()
        {
            var filename = _messageDialogService.SaveExcelFileDialog();
            if (filename.IsNullOrEmpty() == false)
            {
                try
                {
                    var numberOfExportedPersons = _excelExportService.Export(filename);
                    _messageDialogService.OpenInformationDialog("Erfolgreich Exportiert", $"{numberOfExportedPersons} Personen wurden exportiert.");

                    InitializePersons();
                }
                catch (Exception)
                {
                    _messageDialogService.OpenErrorDialog("Fehler beim Exportieren", "Die Personen konnten nicht exportiert werden.");
                }
            }
        }

        private void PrintPersonDetail()
        {
            var dialog = new PrintPersonDetailsDialog();
            dialog.ShowDialog();
        }

        private bool CanTogglePersonsFilter()
        {
            return !IsNewModeActive && !HasChanges;
        }

        private void TogglePersonsFilter()
        {
            if (IsFilterByBirthdate)
            {
                Persons = new ObservableCollection<PersonViewModel>(Persons.OrderBy(t => t.Lastname).ThenBy(t => t.Firstname).ToList());
                IsFilterByBirthdate = false;
            }
            else
            {
                Persons = new ObservableCollection<PersonViewModel>(Persons.OrderBy(t => t.Birthdate?.Month).ThenBy(t => t.Birthdate?.Day).ThenBy(t => t.Lastname).ThenBy(t => t.Firstname).ToList());
                IsFilterByBirthdate = true;
            }

            Notify("Persons");

            SelectedListPerson = Persons.FirstOrDefault();
        }

        private bool ChangeSelectedDetailedPerson()
        {
            var canChangeSelectedDetaiedPerson = true;

            if (IsNewModeActive || HasChanges)
            {
                canChangeSelectedDetaiedPerson = _messageDialogService.OpenConfirmationDialog("Änderungen verwerfen", "Wollen Sie die Änderungen verwerfen?");
                if (canChangeSelectedDetaiedPerson)
                {
                    SelectedDetailedPerson.ResetChanges();
                }
            }

            if (canChangeSelectedDetaiedPerson)
            {
                SelectedDetailedPerson = SelectedListPerson;
            }

            return canChangeSelectedDetaiedPerson;
        }

        public void ReportChange()
        {
            SaveCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
            RevertCommand.RaiseCanExecuteChanged();

            Notify("HasChanges");
        }
    }
}
