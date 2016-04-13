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

        private RevertableObservableCollection<DocumentViewModel, Document> _documents;

        public bool HasChanges
        {
            get
            {
                return (SelectedDetailedPerson != null && SelectedDetailedPerson.HasChanges) || Documents.Any(t => t.HasChanges) || _documents.HasChanges;
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
        
        public ObservableCollection<PersonViewModel> Persons { get; set; }

        public RevertableObservableCollection<DocumentViewModel, Document> Documents
        {
            get { return _documents; }
            set
            {
                ChangeAndNotify(value, ref _documents);
            }
        }

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

            Documents = new RevertableObservableCollection<DocumentViewModel, Document>(this);

            using (var session = _documentStoreFactory.CreateDocumentStore().OpenSession())
            {
                var persons = session.LoadAll<Person>();
                Persons = new ObservableCollection<PersonViewModel>(persons.OrderBy(t => t.Lastname).ThenBy(t => t.Firstname).Select(s => new PersonViewModel(s, this)).ToList());
            }

            SelectedListPerson = Persons.FirstOrDefault();
        }

        private void OnSelectedDetailedPersonChanged()
        {
            Documents = new RevertableObservableCollection<DocumentViewModel, Document>(this);
            if (SelectedDetailedPerson != null && !IsNewModeActive)
            {
                using (var session = _documentStoreFactory.CreateDocumentStore().OpenSession())
                {
                    var documents = session.Query<Document>().Where(t => t.PersonId == SelectedDetailedPerson.Id).ToList();
                    var documentViewModels = documents.Select(s => new DocumentViewModel(s, this)).ToList();

                    Documents = new RevertableObservableCollection<DocumentViewModel, Document>(documentViewModels, this);
                }
            }

            Notify("HasChanges");

            RemoveDocumentsCommand.RaiseCanExecuteChanged();
            AddDocumentCommand.RaiseCanExecuteChanged();
            SaveCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
            RevertCommand.RaiseCanExecuteChanged();
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
                    Documents.ResetChanges();
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
            return IsNewModeActive || HasChanges;
        }

        private void SaveSelectedPerson()
        {
            if (IsNewModeActive)
            {
                Persons.Add(SelectedDetailedPerson);
            }

            var entity = SelectedDetailedPerson?.AcceptChanges();
            var documents = Documents.AcceptChanges();

            using (var session = _documentStoreFactory.CreateDocumentStore().OpenSession())
            {
                session.Store(entity);

                foreach (var documentViewModel in documents)
                {
                    var document = documentViewModel.AcceptChanges();
                    session.Store(document);
                    documentViewModel.Id = document.Id;
                }

                var documentsToDelete = session.Query<Document>().Where(t => t.PersonId == entity.Id).ToList().Where(t => documents.Any(d => d.Id == t.Id) == false);
                foreach (var document in documentsToDelete)
                {
                    session.Delete(document.Id);
                }

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

                Documents.ResetChanges();

                if (string.IsNullOrEmpty(toDelete.Id) == false)
                {
                    using (var session = _documentStoreFactory.CreateDocumentStore().OpenSession())
                    {
                        session.Delete(toDelete.Id);
                        foreach (var t in Documents)
                        {
                            session.Delete(t.Id);
                        }

                        session.SaveChanges();
                    }
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
                Documents.ResetChanges();

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
                var documentViewModel = new DocumentViewModel(this)
                {
                    FileName = filename,
                    PersonId = SelectedDetailedPerson.Id
                };

                Documents.Add(documentViewModel);
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

            RemoveDocumentsCommand.RaiseCanExecuteChanged();
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
                    Documents.ResetChanges();
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

            Notify("HasChanges");
        }
    }
}
