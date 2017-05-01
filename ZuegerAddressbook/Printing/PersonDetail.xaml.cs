using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZuegerAdressbook.ViewModels;

namespace ZuegerAdressbook.Printing
{
    /// <summary>
    /// Interaction logic for PersonDetail.xaml
    /// </summary>
    public partial class PersonDetail : FlowDocument
    {
        private List<PersonViewModel> _persons;

        public PersonDetail(IList<PersonViewModel> persons)
        {
            InitializeComponent();
            PagePadding = new Thickness(20);
            _persons = new List<PersonViewModel>(persons);

            var table = new Table();
            table.FontFamily = new FontFamily(new Uri("pack://application:,,,/ZuegerAddressbook.Name;component/Resources/Fonts/"), "#Frutiger LT Com 55 Roman");
            table.FontSize = 11;
            table.CellSpacing = 0;
            var tableRowGroup = new TableRowGroup();

            tableRowGroup.Rows.Add(CreateHeaderRow());
            tableRowGroup.Rows.Add(CreateEmptyRow());
            tableRowGroup.Rows.Add(CreateRow("Anrede", person => person.Title));
            tableRowGroup.Rows.Add(CreateRow("Name", person => person.Lastname));
            tableRowGroup.Rows.Add(CreateRow("Vorname", person => person.Firstname));
            tableRowGroup.Rows.Add(CreateRow("Strasse / Hausnr.", person => person.Street1));
            tableRowGroup.Rows.Add(CreateRow("PLZ / Ort", person => person.Plz + " " + person.City));
            tableRowGroup.Rows.Add(CreateEmptyRow());
            tableRowGroup.Rows.Add(CreateRow("Festnetz", person => person.PhoneNumber));
            tableRowGroup.Rows.Add(CreateRow("Mobile", person => person.MobileNumber));
            tableRowGroup.Rows.Add(CreateRow("E-Mail", person => person.EmailAddress));
            tableRowGroup.Rows.Add(CreateEmptyRow());
            tableRowGroup.Rows.Add(CreateRow("Geburtsdatum", person => person.Birthdate.HasValue ? person.Birthdate.Value.ToShortDateString() : "-"));
            tableRowGroup.Rows.Add(CreateEmptyRow());
            tableRowGroup.Rows.Add(CreateRow("SBB-Ermässigung", person => "-"));
            tableRowGroup.Rows.Add(CreateEmptyRow());
            tableRowGroup.Rows.Add(CreateRow("Passnummer", person => person.PassportNumber));
            tableRowGroup.Rows.Add(CreateRow("Pass gültig bis", person => ""));
            tableRowGroup.Rows.Add(CreateEmptyRow());
            tableRowGroup.Rows.Add(CreateRow("Bemerkungen", person => person.Notes));
            tableRowGroup.Rows.Add(CreateEmptyRow());

            table.RowGroups.Add(tableRowGroup);
            Section.Blocks.Add(table);
        }

        private TableRow CreateRow(string title, Func<PersonViewModel, string> personPropertySelector)
        {
            var row = new TableRow();
            row.Cells.Add(Cell(Text(title)));
            _persons.ForEach(person => row.Cells.Add(Cell(Text(personPropertySelector(person)))));

            return row;
        }

        private TableRow CreateHeaderRow()
        {
            var headerRow = new TableRow();
            headerRow.FontWeight = FontWeights.Bold;
            headerRow.Background = new SolidColorBrush(Colors.Yellow);
            headerRow.Cells.Add(Cell(Text("Stammdaten")));
            var personEnumerator = _persons.GetEnumerator(); 
            for (var index = 1; personEnumerator.MoveNext() == true; index++)
            {
                headerRow.Cells.Add(Cell(Text(string.Format("{0}. Person", index))));
            }

            return headerRow;
        }
        
        private TableRow CreateEmptyRow()
        {
            var emptyRow = new TableRow();
            emptyRow.Cells.Add(Cell(Text(string.Empty)));
            _persons.ForEach(person => emptyRow.Cells.Add(Cell(Text(string.Empty))));

            return emptyRow;
        }

        private TableCell Cell(Block inlineElement)
        {
            var cell = new TableCell(inlineElement);
            cell.Padding = new Thickness(5);
            cell.BorderBrush = new SolidColorBrush(Colors.Black);
            cell.BorderThickness = new Thickness(0.5);

            return cell;
        }

        private Paragraph Text(string text)
        {
            return new Paragraph(new Run(text));
        }
        
    }
}
