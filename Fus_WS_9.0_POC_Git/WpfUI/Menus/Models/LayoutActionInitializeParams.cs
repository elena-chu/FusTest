using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ws.Fus.DicomViewer.Interfaces.Entities;

namespace WpfUI.Menus.Models
{
    public class LayoutActionInitializeParams : ActionInitializeParams
    {
        public StripsViewerLayout Layout { get; set; }
    }
}
