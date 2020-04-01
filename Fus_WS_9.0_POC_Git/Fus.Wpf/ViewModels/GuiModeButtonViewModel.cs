using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ws.Fus.Interfaces.Overlays;

namespace Ws.Fus.UI.Wpf.ViewModels
{
    class GuiModeButtonViewModel : BindableBase
    {
        public IUiModeChanges UiModeChanges { get; }

        public GuiModeButtonViewModel(IUiModeChanges uiModeChanges)
        {
            UiModeChanges = uiModeChanges;
        }
    }
}
