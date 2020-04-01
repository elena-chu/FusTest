using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUI.Overlays.ViewModels
{
    public class RigidNPRTypesViewModel
    {
        public RigidNPRTypesViewModel(string rigidNPRRadius, string name)
        {
            RigidNPRRadius = rigidNPRRadius;
            Name = name;
        }

        public string RigidNPRRadius { get; private set; }
        public string Name { get; private set; }
    }

}
