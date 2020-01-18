using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DanielsWpfCoaster.Mvvm
{
    public static class CommandFactory
    {
        public static ICommand Create(Action execute, Func<bool> canExecute = null)
        {
            var wrappedCanExecute = new Func<object, bool>(_ => canExecute?.Invoke() ?? true);
            return new ActionCommand<object>(_ => execute(), wrappedCanExecute);
        }

        public static ICommand Create(Func<Task> execute, Func<bool> canExecute = null)
        {
            var wrappedCanExecute = new Func<object, bool>(_ => canExecute?.Invoke() ?? true);
            return new AsyncCommand<object>(_ => execute(), wrappedCanExecute);
        }

        public static ICommand Create<T>(Action<T> execute, Func<T, bool> canExecute = null)
        {
            return new ActionCommand<T>(execute, t => canExecute?.Invoke(t) ?? true);
        }

        public static ICommand Create<T>(Func<T, Task> execute, Func<T, bool> canExecute = null)
        {
            return new AsyncCommand<T>(execute, t => canExecute?.Invoke(t) ?? true);
        }

        private class ActionCommand<T> : ICommand
        {
            private readonly Action<T> _execute;
            private readonly Func<T, bool> _canExecute;

            public ActionCommand(Action<T> execute, Func<T, bool> canExecute)
            {
                _execute = execute;
                _canExecute = canExecute;
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
        private class AsyncCommand<T> : ICommand
        {
            private readonly Func<T, Task> _execute;
            private readonly Func<T, bool> _canExecute;

            public AsyncCommand(Func<T, Task> execute, Func<T, bool> canExecute)
            {
                _execute = execute;
                _canExecute = canExecute;
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

            public async void Execute(object parameter)
            {
                try
                {
                    if (parameter is IConvertible)
                    {
                        await _execute((T)Convert.ChangeType(parameter, typeof(T)));
                    }
                    else
                    {
                        await _execute((T)parameter);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}