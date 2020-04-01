using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.Menus.Enums;
using WpfUI.Menus.Interfaces;

namespace WpfUI.Menus.Models
{
    /// <summary>
    /// Params object for Action's initialization
    /// </summary>
    public class ActionInitializeParams
    {
        /// <summary>
        /// Action's Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Reference to Parent node in Child nodes
        /// </summary>
        public IActionViewModel Parent { get; set; }

        /// <summary>
        /// Child actions
        /// </summary>
        public IList<ActionInitializeParams> ChildActions { get; set; }

        /// <summary>
        /// Node's behavior
        /// </summary>
        public NodeType NodeType { get; set; }

    }
}
