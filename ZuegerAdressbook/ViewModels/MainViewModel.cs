using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;

using ZuegerAdressbook.Annotations;
using ZuegerAdressbook.Commands;
using ZuegerAdressbook.Extensions;
using ZuegerAdressbook.Model;
using ZuegerAdressbook.Service;

namespace ZuegerAdressbook.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private bool IsNewModeActive => SelectedDetailedPerson != null && SelectedDetailedPerson.Id.IsNullOrEmpty();

        private Person _selectedListPerson;

        private Person _selectedDetailedPerson;

        private RelayCommand _newCommand;

        private RelayCommand _saveCommand;

        private RelayCommand _deleteCommand;

        public Person SelectedListPerson
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

        public Person SelectedDetailedPerson
        {
            get
            {
                return _selectedDetailedPerson;
            }
            set
            {
                if (Equals(value, _selectedDetailedPerson))
                {
                    return;
                }
                _selectedDetailedPerson = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Person> Persons { get; set; }

        public RelayCommand NewCommand
        {
            get
            {
                _newCommand = new RelayCommand(CreateNewPerson);
                return _newCommand;
            }
            set
            {
                _newCommand = value;
            }
        }

        public RelayCommand SaveCommand
        {
            get
            {
                _saveCommand = new RelayCommand(SaveSelectedPerson);
                return _saveCommand;
            }
            set
            {
                _saveCommand = value;
            }
        }

        public RelayCommand DeleteCommand
        {
            get
            {
                _deleteCommand = new RelayCommand(DeleteSelectedPerson);
                return _deleteCommand;
            }
            set
            {
                _deleteCommand = value;
            }
        }

        public MainViewModel()
        {
            var listOfPersons = new List<Person>();
            listOfPersons.Add(
                new Person
                {
                    Id = Person.GenerateId(),
                    Gender = Gender.Female,
                    Firstname = "Isabel",
                    Lastname = "Züger",
                    Street1 = "Hegistrasses 39d",
                    City = "Winterthur",
                    Plz = "8404",
                    MobileNumber = "+41764767838",
                    Birthdate = new DateTime(1990, 10, 24),
                    EmailAddress = "isabel.zueger@gmail.com"
                });
            listOfPersons.Add(
                new Person
                {
                    Id = Person.GenerateId(),
                    Gender = Gender.Male,
                    Firstname = "David",
                    Lastname = "Boos",
                    Street1 = "Neuwiesenstrasse 10",
                    City = "Sirnach",
                    Plz = "8370",
                    Birthdate = new DateTime(1991, 02, 11)
                });

            listOfPersons.ForEach(person => person.AcceptChanges());


            Persons = new ObservableCollection<Person>(listOfPersons.OrderBy(t => t.Lastname).ThenBy(t => t.Firstname));

            SelectedListPerson = Persons.First();
        }

        private void CreateNewPerson()
        {
            SelectedDetailedPerson = new Person();
        }

        private bool CanSaveSelectedPerson()
        {
            return IsNewModeActive || (SelectedDetailedPerson != null && SelectedDetailedPerson.IsChanged);
        }

        private void SaveSelectedPerson()
        {
            if (IsNewModeActive)
            {
                SelectedDetailedPerson.Id = Person.GenerateId();

                AddPerson(SelectedDetailedPerson);
                SelectedListPerson = SelectedDetailedPerson;

                // TODO: save new person
            }
            else
            {
                // TODO: save existing person
            }

            SelectedDetailedPerson?.AcceptChanges();
        }

        private void AddPerson(Person person)
        {
            Persons.Add(person);
        }

        private bool CanDeleteSelectedPerson()
        {
            return SelectedListPerson != null && !IsNewModeActive;
        }

        private void DeleteSelectedPerson()
        {
            if (MessageDialogService.OpenConfirmationDialog("Löschen", $"Wollen Sie '{SelectedDetailedPerson.Firstname} {SelectedDetailedPerson.Lastname}' wirklich löschen?"))
            {
                Persons.Remove(SelectedDetailedPerson);
                SelectedDetailedPerson = Persons.FirstOrDefault();
                SelectedListPerson = Persons.FirstOrDefault();

                // TODO: delete person
            }
        }

        public bool ChangeSelectedDetailedPerson()
        {
            var canChangeSelectedDetaiedPerson = true;

            if (IsNewModeActive || (SelectedDetailedPerson != null && SelectedDetailedPerson.IsChanged))
            {
                canChangeSelectedDetaiedPerson = MessageDialogService.OpenConfirmationDialog("Änderungen verwerfen", "Wollen Sie die Änderungen verwerfen?");
            }

            if (canChangeSelectedDetaiedPerson)
            {
                // TODO: reload person
                SelectedDetailedPerson?.AcceptChanges();
                SelectedDetailedPerson = SelectedListPerson;
            }

            return canChangeSelectedDetaiedPerson;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
