using Ws.Extensions.Mvvm.ViewModels;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ws.Extensions.Mvvm.Commands
{
    public class AsyncCommand<T> : AsyncCommandBase<T>, INotifyPropertyChanged
    {
        private readonly Func<T, CancellationToken, Task> _command;
        private readonly CancelAsyncCommand _cancelCommand = new CancelAsyncCommand();
        private readonly Predicate<T> _canExecute;
        private NotifyTaskCompletion _execution;

        public event PropertyChangedEventHandler PropertyChanged;

        public AsyncCommand(Func<T, CancellationToken, Task> command, Predicate<T> canExecute)
        {
            _command = command;
            _canExecute = canExecute;
        }

        public AsyncCommand(Func<T, Task> command, Predicate<T> canExecute) : this(async (p, _) => { await command(p); }, canExecute)
        {
        }

        public AsyncCommand(Func<T, CancellationToken, Task> command)
            : this(command, (parameter) => { return true; })
        {
        }

        public AsyncCommand(Func<T, Task> command) : this(async (p, _) => { await command(p); })
        {
        }

        public NotifyTaskCompletion Execution
        {
            get
            {
                return _execution;
            }
            private set
            {
                _execution = value;
                OnPropertyChanged();
            }
        }

        public bool Executing
        {
            get
            {
                return Execution != null && !Execution.IsCompleted;
            }
        }

        public ICommand CancelCommand
        {
            get { return _cancelCommand; }
        }

        public override bool CanExecute(object parameter)
        {
            return (Execution == null || Execution.IsCompleted) && _canExecute((T)parameter);
        }

        public override async Task ExecuteAsync(object parameter)
        {
            _cancelCommand.NotifyCommandStarting();
            Execution = new NotifyTaskCompletion(_command((T)parameter, _cancelCommand.Token));
            OnPropertyChanged(nameof(Executing));
            RaiseCanExecuteChanged();
            await Execution.TaskCompletion;
            OnPropertyChanged(nameof(Executing));
            _cancelCommand.NotifyCommandFinished();
            RaiseCanExecuteChanged();
        }

        public void Reset()
        {
            Execution = null;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class AsyncCommand : AsyncCommand<object>
    {
        public AsyncCommand(Func<Task> command, Func<bool> canExecute)
            : base(_ => command(), _ => canExecute())
        {
        }

        public AsyncCommand(Func<CancellationToken, Task> command, Func<bool> canExecute)
            : base((_, ct) => command(ct), _ => canExecute())
        {
        }

        public AsyncCommand(Func<Task> command)
            : this(command, () => true)
        {
        }

        public AsyncCommand(Func<CancellationToken, Task> command)
            : this(command, () => true)
        {
        }

        public async virtual void Execute()
        {
            Execute(null);
        }

        public virtual bool CanExecute()
        {
            return CanExecute(null);
        }
    }
}
