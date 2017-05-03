using System.Collections.Generic;
using System.Collections.ObjectModel;
using ZuegerAdressbook.ViewModels;

namespace ZuegerAdressbook.Printing
{
    public class PersonDetailViewModel : ViewModelBase
    {
        public ObservableCollection<PersonViewModel> Persons { get; set; }

        public PersonDetailViewModel()
        {
            Persons = new ObservableCollection<PersonViewModel>();
        }

        public PersonDetailViewModel(IList<PersonViewModel> persons)
        {
            Persons = new ObservableCollection<PersonViewModel>(persons);
            for (int i = 0; i < 100; i++)
            {
                foreach (var person in persons)
                {
                    Persons.Add(person);
                }
            }
            
        }
    }
}
