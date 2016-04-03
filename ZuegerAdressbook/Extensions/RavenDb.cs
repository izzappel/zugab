using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client;

namespace ZuegerAdressbook.Extensions
{
	public static class RavenDb
	{
		public static List<T> LoadAll<T>(this IDocumentSession session)
		{
			int start = 0;
			var list = new List<T>();

			while (true)
			{
				var current = session.Query<T>().Take(1024).Skip(start).ToList();
				if (current.Count == 0)
				{
					break;
				}

				start += current.Count;
				list.AddRange(current);
			}

			return list;
		}
	}
}
