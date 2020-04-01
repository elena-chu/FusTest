using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.Menus.Enums;
using WpfUI.Menus.Models;

namespace WpfUI.Menus.Interfaces
{
    /// <summary>
    /// Base Class Interface for all actions(buttons). Has tree structure to support Submenus
    /// </summary>
    public interface IActionViewModel
    {
        string Name { get; set; }
        bool IsActive { get; set; }
        DelegateCommand ActionCommand { get; }


        IActionViewModel Parent { get; }
        List<IActionViewModel> ChildActions { get; set; }
        bool HasChildActions { get; }
        bool? HasActiveChildActions { get; }
        NodeType NodeType { get; set; }


        void Initialize(ActionInitializeParams param);
        void ChildActionUpdated(IActionViewModel action);
    }
}
