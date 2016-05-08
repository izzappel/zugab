using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using System;
using System.Linq;

using ZuegerAdressbook.Model;

namespace ZuegerAdressbook.Index
{
	public class PersonByBirthdate : AbstractIndexCreationTask<Person>
	{
		public class Result
		{
			public string FirstName { get; set; }

			public string LastName { get; set; }

			public DateTime Birthdate { get; set; }
		}

		public PersonByBirthdate()
		{
			Map = persons => from person in persons
				select new
				{
					person.Firstname,
					person.Lastname,
					person.Birthdate
				};

			Index(x => x.Birthdate, FieldIndexing.Default);
		}
	}
}
