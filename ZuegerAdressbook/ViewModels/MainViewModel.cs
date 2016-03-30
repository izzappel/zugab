using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using ZuegerAdressbook.Annotations;
using ZuegerAdressbook.Commands;
using ZuegerAdressbook.Model;

namespace ZuegerAdressbook.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Person _selectedPerson;

        public Person SelectedPerson
        {
            get
            {
                return _selectedPerson;
            }
            set
            {
                if (Equals(value, _selectedPerson))
                {
                    return;
                }
                _selectedPerson = value;
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

        public MainViewModel()
        {
            Persons = new ObservableCollection<Person>();
            Persons.Add(
                new Person
                {
                    Id = 1,
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

            SelectedPerson = Persons.First();
        }

        private void NewPerson()
        {
            SelectedPerson = null;
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
