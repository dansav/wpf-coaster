using System;
using System.Windows.Input;

namespace DanielsWpfCoaster.Mvvm
{
    public static class CommandFactory
    {
        public static ICommand Create(Action execute, Func<bool> canExecute = null)
        {
            return new ActionCommand(execute, canExecute);
        }

        public static ICommand Create<T>(Action<T> execute, Func<T, bool> canExecute = null)
        {
            return new ActionCommand<T>(execute, canExecute);
        }

        private class ActionCommand : ICommand
        {
            private readonly Action _execute;
            private readonly Func<bool> _canExecute;

            public ActionCommand(Action execute, Func<bool> canExecute = null)
            {
                _execute = execute;
                _canExecute = canExecute ?? (() => true);
            }

            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }

            public bool CanExecute(object _)
            {
                return _canExecute();
            }

            public void Execute(object _)
            {
                _execute();
            }
        }

        private class ActionCommand<T> : ICommand
        {
            private readonly Action<T> _execute;
            private readonly Func<T, bool> _canExecute;

            public ActionCommand(Action<T> execute, Func<T, bool> canExecute = null)
            {
                _execute = execute;
                _canExecute = canExecute ?? (_ => true);
            }

            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }

            public bool CanExecute(object parameter)
            {
                if (typeof(T).IsValueType && parameter == null) return false;

                return parameter is IConvertible
                    ? _canExecute((T)Convert.ChangeType(parameter, typeof(T)))
                    : _canExecute((T)parameter);
            }

            public void Execute(object parameter)
            {
                if (parameter is IConvertible)
                {
                    _execute((T)Convert.ChangeType(parameter, typeof(T)));
                }
                else
                {
                    _execute((T)parameter);
                }
            }
        }
    }

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