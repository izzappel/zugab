using System.ComponentModel;
using System.Runtime.CompilerServices;

using ZuegerAdressbook.Annotations;

namespace ZuegerAdressbook.Model
{
    public abstract class BaseModel : IChangeTracking, INotifyPropertyChanged
    {
        private bool _isChanged = false;

        public void AcceptChanges()
        {
            _isChanged = false;
        }

        public bool IsChanged { get { return _isChanged; } }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            _isChanged = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
