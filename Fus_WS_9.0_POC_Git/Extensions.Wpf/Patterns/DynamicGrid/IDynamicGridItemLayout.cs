using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Extensions.UI.Wpf.Patterns.DynamicGrid
{
    /// <summary>
    /// An interface, that any type used as an item for <see cref="DynamicGrid"/> control, MUST implement.
    /// </summary>
    public interface IDynamicGridItemLayout
    {
        int Row { get; }
        int Column { get; }
        int Span { get; }
    }
}
