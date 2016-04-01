using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using ZuegerAdressbook.Annotations;
using ZuegerAdressbook.Commands;
using ZuegerAdressbook.Extensions;
using ZuegerAdressbook.Model;

namespace ZuegerAdressbook.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Person _selectedListPerson;

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
                _selectedListPerson = value;
                OnPropertyChanged();
            }
        }

        private Person _selectedDetailedPerson;

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

        private RelayCommand _newCommand;
        public RelayCommand NewCommand
        {
            get
            {
                _newCommand = new RelayCommand(NewPerson);
                return _newCommand;
            }
            set
            {
                _newCommand = value;
            }
        }

        private RelayCommand _saveCommand;

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


            Persons = new ObservableCollection<Person>(listOfPersons.OrderBy(t => t.Lastname).ThenBy(t => t.Firstname));

            SelectedListPerson = Persons.First();
        }

        private void NewPerson()
        {
            SelectedDetailedPerson = new Person();
        }

        private void SaveSelectedPerson()
        {
            if (SelectedDetailedPerson != null && SelectedDetailedPerson.Id.IsNullOrEmpty())
            {
                SelectedDetailedPerson.Id = Person.GenerateId();

                AddPerson(SelectedDetailedPerson);
                SelectedListPerson = SelectedDetailedPerson;
            }
            else
            {
                // save existing person
            }
        }

        private void AddPerson(Person person)
        {
            Persons.Add(person);
        }

        public void ChangeSelectedDetailedPerson()
        {
            if (CanChangeSelectedDetailedPerson())
            {
                SelectedDetailedPerson = SelectedListPerson;
            }
        }

        private bool CanChangeSelectedDetailedPerson()
        {
            return SelectedDetailedPerson == null || !SelectedDetailedPerson.Id.IsNullOrEmpty();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
