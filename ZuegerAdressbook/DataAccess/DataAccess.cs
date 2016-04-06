using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ParkSquare.Testing.Helpers;

using Raven.Client;
using Raven.Client.Embedded;

using ZuegerAdressbook.Extensions;
using ZuegerAdressbook.Model;

namespace ZuegerAdressbook.DataAccess
{
    public class DataAccess : IDataAccess
    {
        private static IDocumentStore _documentStore;

        public DataAccess()
        {
            InitializeDocumentStore();
        }

        public IEnumerable<Person> LoadPersons()
        {
            using (var session = _documentStore.OpenSession())
            {
                var personList = session.LoadAll<Person>();

                if (personList.Count == 0)
                {
                    // TODO: remove for production :-)
                    GenerateTestData(personList, session);
                }

                return personList;
            }
        }

        public string SavePerson(Person entity)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(entity);
                session.SaveChanges();
            }

            return entity.Id;
        }

        public void DeletePerson(string id)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Delete(id);
                session.SaveChanges();
            }
        }

        private static void InitializeDocumentStore()
        {
            var appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "zugab");

            if (_documentStore == null)
            {
                _documentStore = new EmbeddableDocumentStore
                {
                    DataDirectory = Path.Combine(appDataFolder, "data")
                };

                _documentStore.Initialize();
            }
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

    }
}
