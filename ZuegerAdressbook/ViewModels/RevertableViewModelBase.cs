namespace ZuegerAdressbook.ViewModels
{
	public abstract class RevertableViewModelBase<T> : ViewModelBase
	{
        public abstract bool HasChanges { get; set; }

        public abstract T AcceptChanges();

	    public abstract void ResetChanges();
	}
}