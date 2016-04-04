using System.ComponentModel;
using System.Runtime.CompilerServices;
using ZuegerAdressbook.Annotations;

namespace ZuegerAdressbook.ViewModels
{
	public abstract class ViewModelBase
	{
		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual bool ChangeAndNotify<T>(T value, ref T field, [CallerMemberName]string propertyName = null)
		{
			if (Equals(field, value))
			{
				return false;
			}

			field = value;
			Notify(propertyName);

			return true;
		}

		protected void Notify([CallerMemberName]string propertyName = null)
		{
			Notify(new[] { propertyName });
		}

		protected void Notify(params string[] propertyName)
		{
			foreach (var name in propertyName)
			{
				PropertyChangedEventHandler handler = PropertyChanged;
				if (handler != null)
				{
					handler(this, new PropertyChangedEventArgs(name));
				}
			}
		}

		[NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
	}
}