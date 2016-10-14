using System.Collections.Generic;
using System.IO;

using NLog;

using OfficeOpenXml;

using ZuegerAdressbook.Extensions;
using ZuegerAdressbook.Model;

namespace ZuegerAdressbook.Service
{
    public class ExcelExportService : IExcelExportService
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IDocumentStoreFactory _documentStoreFactory;

        public ExcelExportService(IDocumentStoreFactory documentStoreFactory)
        {
            _documentStoreFactory = documentStoreFactory;
        }

        public int Export(string filename)
        {
            _logger.Info(LoggerMessage.GetFunctionUsageMessage("Export Addressbook"));

            List<Person> persons = new List<Person>();

            using (var session = _documentStoreFactory.CreateDocumentStore().OpenSession())
            {
                persons = session.LoadAll<Person>();
            }

            var excelPackage = new ExcelPackage();

            excelPackage.Workbook.Properties.SetCustomPropertyValue("Version", 1);
            var worksheet = excelPackage.Workbook.Worksheets.Add("Zugab");

            var addressbookWorksheet = new AddressbookWorksheet(worksheet);
            addressbookWorksheet.WritePersons(persons);

            excelPackage.SaveAs(new FileInfo(filename));

            return persons.Count;
        }
    }
}
