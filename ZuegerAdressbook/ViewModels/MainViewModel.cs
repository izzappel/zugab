using ParkSquare.Testing.Helpers;
using Raven.Client.Embedded;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using Raven.Client;
using ZuegerAdressbook.Commands;
using ZuegerAdressbook.Extensions;
using ZuegerAdressbook.Model;

namespace ZuegerAdressbook.ViewModels
{
	public class MainViewModel : ViewModelBase, INotifyPropertyChanged
	{
		private Person _selectedListPerson;

		private string _filter;

		public string Filter
		{
			get { return _filter; }
			set { ChangeAndNotify(value, ref _filter); }
		}

		public Person SelectedListPerson
		{
			get { return _selectedListPerson; }
			set { ChangeAndNotify(value, ref _selectedListPerson); }
		}

		private Person _selectedDetailedPerson;

		public Person SelectedDetailedPerson
		{
			get { return _selectedDetailedPerson; }
			set { ChangeAndNotify(value, ref _selectedDetailedPerson); }
		}

		public ObservableCollection<Person> Persons { get; set; }

		public RelayCommand NewCommand { get; set; }
		public RelayCommand SaveCommand { get; set; }

		private static EmbeddableDocumentStore _documentStore;

		public MainViewModel()
		{
			NewCommand = new RelayCommand(NewPerson);
			SaveCommand = new RelayCommand(SaveSelectedPerson);

			InitializeDocumentStore();

			using (var session = _documentStore.OpenSession())
			{
				var personList = session.LoadAll<Person>();

				if (personList.Count == 0)
				{
					// TODO: remove for production :-)
					GenerateTestData(personList, session);
				}

				Persons = new ObservableCollection<Person>(personList.OrderBy(t => t.Lastname).ThenBy(t => t.Firstname));
			}

			SelectedListPerson = Persons.First();
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

		private void NewPerson()
		{
			SelectedDetailedPerson = new Person();
		}

		private void SaveSelectedPerson()
		{
			if (SelectedDetailedPerson != null && SelectedDetailedPerson.Id.IsNullOrEmpty())
			{
				//SelectedDetailedPerson.Id = Person.GenerateId();

				AddPerson(SelectedDetailedPerson);
				SelectedListPerson = SelectedDetailedPerson;
			}
			else
			{
				// save existing person
			}

			// TODO: to properly persist, property change triggers should be adjusted
			using (var session = _documentStore.OpenSession())
			{
				session.Store(SelectedDetailedPerson);
				session.SaveChanges();
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
	}
}
