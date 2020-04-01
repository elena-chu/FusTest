using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUI.Menus.Enums
{
    /// <summary>
    /// NodeType specifies the node's behavior
    /// </summary>
    public enum NodeType
    {
        /// <summary>
        /// NodeType is not matter in execution flow
        /// </summary>
        None = 0,
        
        /// <summary>
        /// Child doesn't execute any business logic/action
        /// </summary>
        ChildNoActionType = 1,
        
        /// <summary>
        /// Child's action (business logic) is always executed
        /// </summary>
        ChildExecuteAlways = 2,

        /// <summary>
        /// Temp: Parent doesn't have its indication status
        /// </summary>
        ContainerParentType = 4,

        /// <summary>
        /// Parent executes action on behalf of children (without direct UI interaction)
        /// </summary>
        ProxyContainerParentType = 5,
    }
}
