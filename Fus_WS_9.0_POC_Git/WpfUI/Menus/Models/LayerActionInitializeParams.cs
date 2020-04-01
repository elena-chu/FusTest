using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.Menus.Enums;
using Ws.Fus.Interfaces.Overlays;

namespace WpfUI.Menus.Models
{
    public class LayerActionInitializeParams : ActionInitializeParams
    {
        public UiMode Layer { get; set; }

        public string SubUiMode { get; set; }

    }
}
