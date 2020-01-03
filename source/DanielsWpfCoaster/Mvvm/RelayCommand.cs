using System;
using System.Windows.Input;

namespace DanielsWpfCoaster.Mvvm
{
    public class RelayCommand : ICommand
    {
        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;
        private bool _enabled = true;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;

        }

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled == value) return;

                _enabled = value;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool CanExecute(object parameter)
        {
            return Enabled && (_canExecute?.Invoke(parameter) ?? true);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public virtual event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}