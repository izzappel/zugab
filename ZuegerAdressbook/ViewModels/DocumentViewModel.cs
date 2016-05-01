using System.Runtime.CompilerServices;

using ZuegerAdressbook.Model;

namespace ZuegerAdressbook.ViewModels
{
    public class DocumentViewModel : RevertableViewModelBase<Document>
    {
        private Document _document;

        private bool _hasChanges;

        private string _id;

        private string _fileName;

        private string _personId;

        private bool _isSelected;

        private IChangeListener _parent;

        public DocumentViewModel(Document document, IChangeListener parent)
        {
            if (document == null)
            {
                document = new Document();
            }

            _document = document;
            _parent = parent;

            CopyFromEntity(document);
        }

        private void CopyFromEntity(Document document)
        {
            _id = document.Id;
            _fileName = document.FileName;
            _personId = document.PersonId;
        }

        public override bool HasChanges
        {
            get { return _hasChanges; }
            set { ChangeAndNotify(value, ref _hasChanges); }
        }

        public string Id
        {
            get { return _id; }
            set { ChangeAndNotify(value, ref _id); }
        }
        
        public string FileName
        {
            get { return _fileName; }
            set { ChangeAndNotify(value, ref _fileName); }
        }
        
        public string PersonId
        {
            get { return _personId; }
            set { ChangeAndNotify(value, ref _personId); }
        }
        
        public bool IsSelected
        {
            get { return _isSelected; }
            set { ChangeAndNotify(value, ref _isSelected); }
        }

        protected override bool ChangeAndNotify<T>(T value, ref T field, [CallerMemberName] string propertyName = null)
        {
            // Note: we should extract this into a superclass ChangeTrackingViewModel if needed for further entities
            var hasChanged = base.ChangeAndNotify(value, ref field, propertyName);

            if (hasChanged)
            {
                if (propertyName != "HasChanges" && propertyName != "Id" && propertyName != "IsSelected") // Id will never be set from UI and should never trigger the change tracking
                {
                    HasChanges = true;
                }

                _parent?.ReportChange();
            }

            return hasChanged;
        }

        public override Document AcceptChanges()
        {
            HasChanges = false;

            var document = new Document
            {
                Id = _id,
                FileName = _fileName,
                PersonId = _personId
            };

            _document = document;

            return document;
        }

        public override void ResetChanges()
        {
            Id = _document.Id;
            FileName = _document.FileName;
            PersonId = _document.PersonId;

            HasChanges = false;
        }
    }
}
