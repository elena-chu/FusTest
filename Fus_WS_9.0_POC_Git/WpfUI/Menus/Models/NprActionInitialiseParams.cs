using FusInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUI.Menus.Models
{
    public class NprActionInitialiseParams : ActionInitializeParams
    {
        public string NPRRadius { get; set; }
        public UiMode Layer { get; set; }
    }
}
