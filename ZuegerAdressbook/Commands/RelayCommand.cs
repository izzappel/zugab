using System;
using System.Windows.Input;

namespace ZuegerAdressbook.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action _methodToExecute;

        private readonly Func<bool> _canExecuteEvaluator;

        public RelayCommand(Action methodToExecute, Func<bool> canExecuteEvaluator = null)
        {
            _methodToExecute = methodToExecute;
            _canExecuteEvaluator = canExecuteEvaluator;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecuteEvaluator == null)
            {
                return true;
            }

            return _canExecuteEvaluator.Invoke();
        }

        public void Execute(object parameter)
        {
            _methodToExecute.Invoke();
        }

        public event EventHandler CanExecuteChanged;
    }
}
