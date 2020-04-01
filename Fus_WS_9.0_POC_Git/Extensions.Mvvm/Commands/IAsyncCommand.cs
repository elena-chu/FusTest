using System.Threading.Tasks;
using System.Windows.Input;

namespace Ws.Extensions.Mvvm.Commands
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
