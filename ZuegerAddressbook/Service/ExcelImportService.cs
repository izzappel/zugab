using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NLog;

using OfficeOpenXml;

using ZuegerAdressbook.Model;

namespace ZuegerAdressbook.Service
{
    public class ExcelImportService : IExcelImportService
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IDocumentStoreFactory _documentStoreFactory;

        public ExcelImportService(IDocumentStoreFactory documentStoreFactory)
        {
            _documentStoreFactory = documentStoreFactory;
        }

        public int Import(string filename)
        {
            try
            {
                IList<Person> persons = new List<Person>();
                var file = new FileInfo(filename);
                using (var package = new ExcelPackage(file))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet != null)
                    {
                        if (OldAddressbookWorksheet.IsOldAddressbookExcel(worksheet))
                        {
                            persons = ImportOldAddressbookExcel(worksheet);

                            _logger.Info(LoggerMessage.GetFunctionUsageMessage("Import Old Addressbook"));
                        }
                        else
                        {
                            persons = ImportAddressbookExcel(worksheet);
                            _logger.Info(LoggerMessage.GetFunctionUsageMessage("Import Addressbook"));
                        }

                        SavePersons(persons);
                    }
                }

                return persons.Count;
            }
            catch (Exception e)
            {
                _logger.Warn(e, "Exception on Import Persons");
                throw;
            }
        }

        private void SavePersons(IList<Person> persons)
        {
            using (var session = _documentStoreFactory.CreateDocumentStore().OpenSession())
            {
                foreach (var person in persons)
                {
                    session.Store(person);
                }

                session.SaveChanges();
            }
        }

        private IList<Person> ImportOldAddressbookExcel(ExcelWorksheet worksheet)
        {
            var oldWorksheet = new OldAddressbookWorksheet(worksheet);
            return oldWorksheet.ImportPersons();
        }

        private IList<Person> ImportAddressbookExcel(ExcelWorksheet worksheet)
        {
            var addressbookWorksheet = new AddressbookWorksheet(worksheet);
            return addressbookWorksheet.ReadPersons();
        }
    }
}
