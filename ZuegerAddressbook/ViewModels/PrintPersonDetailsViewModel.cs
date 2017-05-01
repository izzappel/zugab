using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Documents;
using ZuegerAdressbook.Commands;
using ZuegerAdressbook.Extensions;
using ZuegerAdressbook.Model;
using ZuegerAdressbook.Printing;
using ZuegerAdressbook.Service;

namespace ZuegerAdressbook.ViewModels
{
    public class PrintPersonDetailsViewModel : ViewModelBase, INotifyPropertyChanged, IChangeListener
    {
        private readonly IDocumentStoreFactory _documentStoreFactory;
        private readonly IMessageDialogService _messageDialogService;

        private PersonViewModel _selectedListPerson;
        private PersonViewModel _selectedSelectedPerson;

        public ObservableCollection<PersonViewModel> Persons { get; set; }
        public ObservableCollection<PersonViewModel> SelectedPersons { get; set; }
        public PersonViewModel SelectedListPerson
        {
            get { return _selectedListPerson; }
            set
            {
                ChangeAndNotify(value, ref _selectedListPerson);
                AddSelectedToSelectedPersonsCommand.RaiseCanExecuteChanged();
            }
        }
        public PersonViewModel SelectedSelectedPerson
        {
            get { return _selectedSelectedPerson; }
            set
            {
                ChangeAndNotify(value, ref _selectedSelectedPerson);
                RemoveSelectedFromSelectedPersonsCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand AddSelectedToSelectedPersonsCommand { get; set; }
        public RelayCommand RemoveSelectedFromSelectedPersonsCommand { get; set; }
        public RelayCommand PrintPersonDetailCommand { get; set; }

        public PrintPersonDetailsViewModel() { }

        public PrintPersonDetailsViewModel(IDocumentStoreFactory documentStoreFactory, IMessageDialogService messageDialogService)
        {
            _documentStoreFactory = documentStoreFactory;
            _messageDialogService = messageDialogService;

            AddSelectedToSelectedPersonsCommand = new RelayCommand(AddSelectedToSelectedPersons, CanAddSelectedToSelectedPersons);
            RemoveSelectedFromSelectedPersonsCommand = new RelayCommand(RemoveSelectedFromSelectedPersons, CanRemoveSelectedFromSelectedPersons);
            PrintPersonDetailCommand = new RelayCommand(PrintPersonDetail);
            SelectedPersons = new ObservableCollection<PersonViewModel>();

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
        }

        private bool CanAddSelectedToSelectedPersons()
        {
            return SelectedListPerson == null ? false : true;
        }

        private void AddSelectedToSelectedPersons()
        {
            SelectedPersons.Add(SelectedListPerson);
            Persons.Remove(SelectedListPerson);
        }

        private bool CanRemoveSelectedFromSelectedPersons()
        {
            return SelectedSelectedPerson == null ? false : true;
        }

        private void RemoveSelectedFromSelectedPersons()
        {
            Persons.Add(SelectedSelectedPerson);
            SelectedPersons.Remove(SelectedSelectedPerson);
        }

        private void PrintPersonDetail()
        {
            FlowDocument personDetailsDocument = new PersonDetail(SelectedPersons);
            _messageDialogService.OpenPrintDialog(personDetailsDocument, "Person Details");
        }

        public void ReportChange()
        {
        }
    }
}
