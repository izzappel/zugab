namespace ZuegerAdressbook.Model
{
    public class Document : IIdentifiable
    {
        public string Id { get; set; }

        public string FileName { get; set; }

        public string PersonId { get; set; }
    }
}
