using Extensions.Mvvm.ViewModels;
using Fus.Strips.Contracts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Fus.Strips.Wpf.ViewModels
{
    class StripVm : BindableWrapper<IStrip>
    {
        public StripVm(IStrip strip) : base(strip)
        {
        }

        public IStrip Strip => Value;

        public object Thumbnail => Strip.Image;
    }
}
