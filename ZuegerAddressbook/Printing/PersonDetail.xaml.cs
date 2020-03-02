using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
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

            var fontFamily = new FontFamily(new Uri("pack://application:,,,/ZuegerAddressbook.Name;component/Resources/Fonts/"), "#Frutiger LT Com 55 Roman");

            var table = new Table();
            table.FontFamily = fontFamily;
            table.FontSize = 11;
            table.CellSpacing = 0;
            var tableRowGroup = new TableRowGroup();

            tableRowGroup.Rows.Add(CreatePersonHeaderRow());
            tableRowGroup.Rows.Add(CreateEmptyPersonRow());
            tableRowGroup.Rows.Add(CreatePersonRow("Anrede", person => CreateGenderString(person.Gender)));
            tableRowGroup.Rows.Add(CreatePersonRow("Name", person => person.Lastname));
            tableRowGroup.Rows.Add(CreatePersonRow("Vorname", person => person.Firstname));
            tableRowGroup.Rows.Add(CreatePersonRow("Strasse / Hausnr.", person => person.Street1));
            tableRowGroup.Rows.Add(CreatePersonRow("PLZ / Ort", person => person.Plz + " " + person.City));
            tableRowGroup.Rows.Add(CreateEmptyPersonRow());
            tableRowGroup.Rows.Add(CreatePersonRow("Festnetz", person => person.PhoneNumber));
            tableRowGroup.Rows.Add(CreatePersonRow("Mobile", person => person.MobileNumber));
            tableRowGroup.Rows.Add(CreatePersonRow("E-Mail", person => person.EmailAddress));
            tableRowGroup.Rows.Add(CreateEmptyPersonRow());
            tableRowGroup.Rows.Add(CreatePersonRow("Geburtsdatum", person => person.Birthdate.HasValue ? person.Birthdate.Value.ToShortDateString() : "-"));
            tableRowGroup.Rows.Add(CreateEmptyPersonRow());
            tableRowGroup.Rows.Add(CreatePersonRow("SBB-Ermässigung", person => CreateReductionString(person)));
            tableRowGroup.Rows.Add(CreateEmptyPersonRow());
            tableRowGroup.Rows.Add(CreatePersonRow("Name auf Pass", person => string.Format("{0} {1}", person.PassportGivenName, person.PassportSurname)));
            tableRowGroup.Rows.Add(CreatePersonRow("Passnummer", person => person.PassportNumber));
            tableRowGroup.Rows.Add(CreatePersonRow("Heimatort", person => person.PlaceOfOrigin));
            tableRowGroup.Rows.Add(CreatePersonRow("Geburtsort", person => person.PlaceOfBirth));
            tableRowGroup.Rows.Add(CreatePersonRow("Nationalität", person => CreatePassportNationalityString(person)));
            tableRowGroup.Rows.Add(CreatePersonRow("Pass ausgestellt am", person => person.PassportIssueDate.HasValue ? person.PassportIssueDate.Value.ToShortDateString() : "-"));
            tableRowGroup.Rows.Add(CreatePersonRow("Pass gültig bis", person => person.PassportExpirationDate.HasValue ? person.PassportExpirationDate.Value.ToShortDateString() : "-"));
            tableRowGroup.Rows.Add(CreateEmptyPersonRow());
            tableRowGroup.Rows.Add(CreatePersonRow("Annullationsversicherung", person => person.HasCancellationInsurance ? string.Format("{0} (von: {1:dd.MM.yyyy}, bis: {2:dd.MM.yyyy})", person.CancellationInsurance, person.CancellationInsuranceIssueDate?.ToShortDateString(), person.CancellationInsuranceExpirationDate?.ToShortDateString()) : "Nein"));
            tableRowGroup.Rows.Add(CreateEmptyPersonRow());
            tableRowGroup.Rows.Add(CreatePersonRow("Bemerkungen", person => person.Notes));
            tableRowGroup.Rows.Add(CreateEmptyPersonRow());

            table.RowGroups.Add(tableRowGroup);

            var feedbackTable = new Table();
            feedbackTable.FontFamily = fontFamily;
            feedbackTable.FontSize = 11;
            feedbackTable.CellSpacing = 0;

            var feedbackTableRowGroup = new TableRowGroup();
            feedbackTableRowGroup.Rows.Add(CreateFeedbackTitleRow());
            feedbackTableRowGroup.Rows.Add(CreateFeedbackHeadrRow());
            feedbackTableRowGroup.Rows.Add(CreateEmptyFeedbackRow());
            feedbackTableRowGroup.Rows.Add(CreateEmptyFeedbackRow());
            feedbackTableRowGroup.Rows.Add(CreateEmptyFeedbackRow());
            feedbackTableRowGroup.Rows.Add(CreateEmptyFeedbackRow());
            feedbackTableRowGroup.Rows.Add(CreateEmptyFeedbackRow());
            feedbackTableRowGroup.Rows.Add(CreateEmptyFeedbackRow());
            feedbackTableRowGroup.Rows.Add(CreateEmptyFeedbackRow());

            feedbackTable.RowGroups.Add(feedbackTableRowGroup);

            Section.Blocks.Add(table);
            Section.Blocks.Add(TextBlock(string.Format("gedruckt am: {0}", DateTime.Now.ToShortDateString())));
            Section.Blocks.Add(feedbackTable);
        }

        private string CreateGenderString(Model.Gender gender)
        {
            return gender == Model.Gender.Male ? "Mann" : "Frau";
        }

        private string CreatePassportNationalityString(PersonViewModel person)
        {
            var nationality = person.PassportNationality;
            var nationalityCode = !string.IsNullOrEmpty(person.PassportNationalityCode) ? string.Format("({0})", person.PassportNationalityCode) : "";
            return string.Format("{0} {1}", nationality, nationalityCode);
        }

        private string CreateReductionString(PersonViewModel person)
        {
            var reductions = new List<string>();
            if (person.HasHalbTax)
            {
                reductions.Add("Halbtax");
            }

            if (person.HasGeneralAbo)
            {
                reductions.Add("GA");
            }

            if(person.HasEnkelKarte)
            {
                reductions.Add("Enkelkarte");
            }

            if (person.HasJuniorKarte)
            {
                reductions.Add("Juniorkarte");
            }

            return string.Join(", ", reductions);
        }

        private TableRow CreatePersonRow(string title, Func<PersonViewModel, string> personPropertySelector)
        {
            var row = new TableRow();
            row.Cells.Add(Cell(Text(title)));
            _persons.ForEach(person => row.Cells.Add(Cell(Text(personPropertySelector(person)))));

            return row;
        }

        private TableRow CreatePersonHeaderRow()
        {
            var headerRow = new TableRow();
            headerRow.FontWeight = FontWeights.Bold;
            headerRow.Background = new SolidColorBrush(Colors.Yellow);
            headerRow.Cells.Add(Cell(Text("Stammdaten")));
            _persons.ForEach(person => headerRow.Cells.Add(Cell(Text(string.Format("{0} {1}", person.Lastname, person.Firstname)))));
            
            return headerRow;
        }
        
        private TableRow CreateEmptyPersonRow()
        {
            var emptyRow = new TableRow();
            emptyRow.Cells.Add(Cell(Text(string.Empty)));
            _persons.ForEach(person => emptyRow.Cells.Add(Cell(Text(string.Empty))));

            return emptyRow;
        }

        private TableRow CreateFeedbackTitleRow()
        {
            var titleCell = Cell(Text("Kundenfeedback"));
            titleCell.ColumnSpan = 3;

            var titleRow = new TableRow();
            titleRow.FontWeight = FontWeights.Bold;
            titleRow.Background = new SolidColorBrush(Colors.Yellow);
            titleRow.Cells.Add(titleCell);

            return titleRow;
        }

        private TableRow CreateFeedbackHeadrRow()
        {
            var headerRow = new TableRow();
            headerRow.FontWeight = FontWeights.Bold;
            headerRow.Cells.Add(Cell(Text("erhalten am")));
            headerRow.Cells.Add(Cell(Text("Gutschein")));
            headerRow.Cells.Add(Cell(Text("eingelöst am")));

            return headerRow;
        }

        private TableRow CreateEmptyFeedbackRow()
        {
            var emptyRow = new TableRow();
            emptyRow.Cells.Add(Cell(Text(string.Empty)));
            emptyRow.Cells.Add(Cell(Text(string.Empty)));
            emptyRow.Cells.Add(Cell(Text(string.Empty)));

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
            var fontFamily = new FontFamily(new Uri("pack://application:,,,/ZuegerAddressbook.Name;component/Resources/Fonts/"), "#Frutiger LT Com 55 Roman");

            var paragraph = new Paragraph(new Run(text));
            paragraph.FontFamily = fontFamily;
            paragraph.FontSize = 11;

            return paragraph;
        }

        private Paragraph TextBlock(string text)
        {
            var fontFamily = new FontFamily(new Uri("pack://application:,,,/ZuegerAddressbook.Name;component/Resources/Fonts/"), "#Frutiger LT Com 55 Roman");

            var paragraph = new Paragraph(new Run(text));
            paragraph.FontFamily = fontFamily;
            paragraph.FontSize = 11;
            paragraph.Padding = new Thickness(5);

            return paragraph;
        }

    }
}
