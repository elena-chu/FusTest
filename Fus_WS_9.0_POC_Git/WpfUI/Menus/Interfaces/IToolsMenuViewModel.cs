using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUI.Menus.Interfaces
{
    public interface IToolsMenuViewModel : IPinnableMenu
    {
        event EventHandler IsOpening;
        void TryToClose();
    }
}
