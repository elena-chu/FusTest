using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ws.Extensions.Mvvm.Commands
{
    public abstract class AsyncCommandBase<T> : IAsyncCommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public async void Execute(object parameter)
        {
            Execute((T)parameter);
        }

        public async void Execute(T parameter)
        {
            await ExecuteAsync(parameter);
        }

        public virtual void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public abstract bool CanExecute(object parameter);
        public abstract Task ExecuteAsync(object parameter);
    }
}
