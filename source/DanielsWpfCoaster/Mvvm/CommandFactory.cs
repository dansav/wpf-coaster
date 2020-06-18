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

        public static T ConvertParameter<T>(object value)
        {
            if (typeof(T).IsValueType && value == null) return default;

            return value is IConvertible
                ? (T)Convert.ChangeType(value, typeof(T))
                : (T)value;
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
                return _canExecute(ConvertParameter<T>(parameter));
            }

            public void Execute(object parameter)
            {
                _execute(ConvertParameter<T>(parameter));
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
                return _canExecute(ConvertParameter<T>(parameter));
            }

            public async void Execute(object parameter)
            {
                try
                {
                    await _execute(ConvertParameter<T>(parameter));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}