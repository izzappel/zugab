using System.Collections.Generic;
using ZuegerAdressbook.Model;

namespace ZuegerAdressbook.DataAccess
{
    public interface IDataAccess
    {
        void DeletePerson(string entity);

        IEnumerable<Person> LoadPersons();

        string SavePerson(Person entity);
    }
}