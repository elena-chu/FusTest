using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUI.Menus.Interfaces
{
    public interface IPinnableMenu
    {
        bool IsStayOpen { get; set; }
        bool IsOpen { get; set; }
    }
}
