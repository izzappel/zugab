using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using ParkSquare.Testing.Helpers;
using Raven.Client;
using Raven.Client.Embedded;
using ZuegerAdressbook.Commands;
using ZuegerAdressbook.Extensions;
using ZuegerAdressbook.Model;
using ZuegerAdressbook.Service;

namespace ZuegerAdressbook.ViewModels
{
    public class MainViewModel : ViewModelBase, INotifyPropertyChanged
    {
		private static IDocumentStore _documentStore;

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
                    Application.Current.Dispatcher.BeginInvoke(
                        new Action(
                            () =>
                            {
                                // Do this against the underlying value so 
                                //  that we don't invoke the cancellation question again.
                                _selectedListPerson = origValue;
                                OnPropertyChanged();
                            }),
                        DispatcherPriority.ContextIdle,
                        null);
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

        public RelayCommand NewCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand RevertCommand { get; set; }
        public RelayCommand AddDocumentCommand { get; set; }

        public MainViewModel()
        {
			NewCommand = new RelayCommand(CreateNewPerson);
			SaveCommand = new RelayCommand(SaveSelectedPerson, CanSaveSelectedPerson);
			DeleteCommand = new RelayCommand(DeleteSelectedPerson, CanDeleteSelectedPerson);
            RevertCommand = new RelayCommand(RevertChanges, CanRevertChanges);
            AddDocumentCommand = new RelayCommand(CreateNewPerson);

			InitializeDocumentStore();

         	using (var session = _documentStore.OpenSession())
			{
				var personList = session.LoadAll<Person>();

				if (personList.Count == 0)
				{
					// TODO: remove for production :-)
					GenerateTestData(personList, session);
				}

				Persons = new ObservableCollection<PersonViewModel>(personList.OrderBy(t => t.Lastname).ThenBy(t => t.Firstname).Select(s => new PersonViewModel(s, this)).ToList());
			}

			SelectedListPerson = Persons.First();
        }

        private void OnSelectedDetailedPersonChanged()
        {
            SaveCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
            RevertCommand.RaiseCanExecuteChanged();
        }

		private static void InitializeDocumentStore()
		{
			var appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "zugab");

			_documentStore = new EmbeddableDocumentStore
			{
				DataDirectory = Path.Combine(appDataFolder, "data")
			};

			_documentStore.Initialize();
		}

		private static void GenerateTestData(List<Person> listOfPersons, IDocumentSession session)
		{
			Enumerable.Range(0, 800).ToList().ForEach(n =>
			{
				listOfPersons.Add(new Person
				{
					Firstname = NameGenerator.AnyForename(),
					Lastname = NameGenerator.AnyName(),
					Gender = Gender.Female,
					EmailAddress = EmailAddressGenerator.AnyEmailAddress(),
					Birthdate = DateTimeGenerator.AnyDateBetween(new DateTime(1930, 1, 1), DateTime.Now),
					PhoneNumber = TelephoneNumberGenerator.AnyTelephoneNumber(),
					MobileNumber = TelephoneNumberGenerator.AnyMobileNumber()
				});
			});
			
			listOfPersons.ForEach(person =>
			{
				session.Store(person);
			});

			session.SaveChanges();
		}
        
        private void CreateNewPerson()
        {
            var canChangeSelectedDetaiedPerson = true;

            if (SelectedDetailedPerson != null && SelectedDetailedPerson.HasChanges)
            {
                canChangeSelectedDetaiedPerson = MessageDialogService.OpenConfirmationDialog("Änderungen verwerfen", "Wollen Sie die Änderungen verwerfen?");
            }

            if (canChangeSelectedDetaiedPerson)
            {
                SelectedDetailedPerson?.ResetChanges();
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
                SelectedListPerson = SelectedDetailedPerson;
            }

	        var entity = SelectedDetailedPerson?.AcceptChanges();

			using (var session = _documentStore.OpenSession())
			{
				session.Store(entity);
			    SelectedDetailedPerson.Id = entity.Id;
				session.SaveChanges();
			}
        }

        private bool CanDeleteSelectedPerson()
        {
            return SelectedListPerson != null && !IsNewModeActive;
        }

        private void DeleteSelectedPerson()
        {
            if (MessageDialogService.OpenConfirmationDialog("Löschen", $"Wollen Sie '{SelectedDetailedPerson.Firstname} {SelectedDetailedPerson.Lastname}' wirklich löschen?"))
            {
                var toDelete = SelectedDetailedPerson;
                Persons.Remove(SelectedDetailedPerson);
                SelectedDetailedPerson = Persons.FirstOrDefault();
                SelectedListPerson = Persons.FirstOrDefault();

                if (string.IsNullOrEmpty(toDelete.Id) == false)
                {
                    using (var session = _documentStore.OpenSession())
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
            if (MessageDialogService.OpenConfirmationDialog("Änderungen verwerfen", "Wollen Sie die Änderungen verwerfen?"))
            {
                SelectedDetailedPerson?.ResetChanges();
            }
        }

        private void AddDocument()
        {
            if (SelectedDetailedPerson != null)
            {
                var filename = MessageDialogService.OpenFileDialog();
                if (filename.IsNullOrEmpty() == false)
                {
                    //SelectedDetailedPerson.Documents.Add(filename);
                    
                }
            }
        }

        private bool ChangeSelectedDetailedPerson()
        {
            var canChangeSelectedDetaiedPerson = true;

            if (IsNewModeActive || (SelectedDetailedPerson != null && SelectedDetailedPerson.HasChanges))
            {
                canChangeSelectedDetaiedPerson = MessageDialogService.OpenConfirmationDialog("Änderungen verwerfen", "Wollen Sie die Änderungen verwerfen?");
            }

            if (canChangeSelectedDetaiedPerson)
            {
                SelectedDetailedPerson?.ResetChanges();
                SelectedDetailedPerson = SelectedListPerson;
            }

            return canChangeSelectedDetaiedPerson;
        }
    }
}
