using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using Raven.Abstractions.Extensions;

using ZuegerAdressbook.Commands;
using ZuegerAdressbook.Extensions;
using ZuegerAdressbook.Model;
using ZuegerAdressbook.Service;

namespace ZuegerAdressbook.ViewModels
{
    public class MainViewModel : ViewModelBase, INotifyPropertyChanged, IChangeListener
    {
        private readonly IDocumentStoreFactory _documentStoreFactory;

        private readonly IDispatcher _dispatcher;

        private readonly IMessageDialogService _messageDialogService;

        private bool IsNewModeActive => SelectedDetailedPerson != null && SelectedDetailedPerson.Id.IsNullOrEmpty();

        private PersonViewModel _selectedListPerson;

        private PersonViewModel _selectedDetailedPerson;

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

        public ObservableCollection<PersonViewModel> Persons { get; set; }

        public ObservableCollection<DocumentViewModel> Documents { get; set; }

        public RelayCommand NewCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand RevertCommand { get; set; }
        public RelayCommand AddDocumentCommand { get; set; }
        public RelayCommand RemoveDocumentsCommand { get; set; }

        public MainViewModel()
        {
        }

        public MainViewModel(IDocumentStoreFactory documentStoreFactory, IDispatcher dispatcher, IMessageDialogService messageDialogService)
        {
            _documentStoreFactory = documentStoreFactory;
            _dispatcher = dispatcher;
            _messageDialogService = messageDialogService;

            NewCommand = new RelayCommand(CreateNewPerson);
            SaveCommand = new RelayCommand(SaveSelectedPerson, CanSaveSelectedPerson);
            DeleteCommand = new RelayCommand(DeleteSelectedPerson, CanDeleteSelectedPerson);
            RevertCommand = new RelayCommand(RevertChanges, CanRevertChanges);
            AddDocumentCommand = new RelayCommand(AddDocument, CanAddDocument);
            RemoveDocumentsCommand = new RelayCommand(RemoveDocuments, CanRemoveDocuments);

            Documents = new ObservableCollection<DocumentViewModel>();

            using (var session = _documentStoreFactory.CreateDocumentStore().OpenSession())
            {
                var persons = session.LoadAll<Person>();
                Persons = new ObservableCollection<PersonViewModel>(persons.OrderBy(t => t.Lastname).ThenBy(t => t.Firstname).Select(s => new PersonViewModel(s, this)).ToList());
            }


            SelectedListPerson = Persons.FirstOrDefault();
        }

        private void OnSelectedDetailedPersonChanged()
        {
            Documents.Clear();
            if (SelectedDetailedPerson != null && !IsNewModeActive)
            {
                using (var session = _documentStoreFactory.CreateDocumentStore().OpenSession())
                {
                    var documents = session.Query<Document>().Where(t => t.PersonId == SelectedDetailedPerson.Id);
                    documents.ToList().Select(s => new DocumentViewModel(s, this)).ForEach(Documents.Add);
                }
            }

            RemoveDocumentsCommand.RaiseCanExecuteChanged();
            AddDocumentCommand.RaiseCanExecuteChanged();
            SaveCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
            RevertCommand.RaiseCanExecuteChanged();
        }

        private void CreateNewPerson()
        {
            var canChangeSelectedDetaiedPerson = true;

            if (IsNewModeActive || SelectedDetailedPerson != null && SelectedDetailedPerson.HasChanges)
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
                SelectedDetailedPerson = new PersonViewModel(this);
            }
        }

        private bool CanSaveSelectedPerson()
        {
            return IsNewModeActive || (SelectedDetailedPerson != null && SelectedDetailedPerson.HasChanges);
        }

        private void SaveSelectedPerson()
        {
            if (IsNewModeActive)
            {
                Persons.Add(SelectedDetailedPerson);
            }

            var entity = SelectedDetailedPerson?.AcceptChanges();

            using (var session = _documentStoreFactory.CreateDocumentStore().OpenSession())
            {
                session.Store(entity);
                session.SaveChanges();
            }

            SelectedDetailedPerson.Id = entity.Id;

            SelectedListPerson = SelectedDetailedPerson;
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

                if (string.IsNullOrEmpty(toDelete.Id) == false)
                {
                    using (var session = _documentStoreFactory.CreateDocumentStore().OpenSession())
                    {
                        session.Delete(toDelete.Id);
                        session.SaveChanges();
                    }
                }
            }
        }

        private bool CanRevertChanges()
        {
            return IsNewModeActive || (SelectedDetailedPerson != null && SelectedDetailedPerson.HasChanges);
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
                }
            }
        }

        private bool CanAddDocument()
        {
            return SelectedDetailedPerson != null && !IsNewModeActive;
        }

        private void AddDocument()
        {
            var filename = _messageDialogService.OpenFileDialog();
            if (filename.IsNullOrEmpty() == false)
            {
                var documentViewModel = new DocumentViewModel(this);
                documentViewModel.FileName = filename;
                documentViewModel.PersonId = SelectedDetailedPerson.Id;

                Documents.Add(documentViewModel);

                var entity = documentViewModel.AcceptChanges();

                using (var session = _documentStoreFactory.CreateDocumentStore().OpenSession())
                {
                    session.Store(entity);
                    session.SaveChanges();
                }

                documentViewModel.Id = entity.Id;
            }

            RemoveDocumentsCommand.RaiseCanExecuteChanged();
        }

        private bool CanRemoveDocuments()
        {
            return !IsNewModeActive && SelectedDetailedPerson != null && Documents.Any(t => t.IsSelected);
        }

        private void RemoveDocuments()
        {
            var toDelete = Documents.Where(t => t.IsSelected).ToList();
            toDelete.ForEach(t => Documents.Remove(t));

            using (var session = _documentStoreFactory.CreateDocumentStore().OpenSession())
            {
                toDelete.ForEach(t => session.Delete(t.Id));
                session.SaveChanges();
            }

            RemoveDocumentsCommand.RaiseCanExecuteChanged();
        }

        private bool ChangeSelectedDetailedPerson()
        {
            var canChangeSelectedDetaiedPerson = true;

            if (IsNewModeActive || (SelectedDetailedPerson != null && SelectedDetailedPerson.HasChanges))
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
            AddDocumentCommand.RaiseCanExecuteChanged();
            RemoveDocumentsCommand.RaiseCanExecuteChanged();
        }
    }
}
