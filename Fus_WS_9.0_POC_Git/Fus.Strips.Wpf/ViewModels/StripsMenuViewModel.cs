using Fus.Strips.Contracts.Entities;
using Fus.Strips.Contracts.Services;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Fus.Strips.Wpf.ViewModels
{
    class StripsMenuViewModel : BindableBase
    {
        private readonly Dispatcher _dispatcher;
        private readonly IStripsManager _stripsManager;

        public StripsMenuViewModel(Dispatcher dispatcher, IStripsManager stripsManager)
        {
            _dispatcher = dispatcher;
            _stripsManager = stripsManager;
            _stripsManager.Changed += StripsManager_Changed;
        }        

        public ObservableCollection<IStrip> Strips { get; } = new ObservableCollection<IStrip>();

        private void StripsManager_Changed(object sender, EventArgs e)
        {
            _dispatcher.Invoke(() =>
            {
                var strips = _stripsManager.GetStrips();

                Strips.Clear();

                foreach (var strip in strips)
                    Strips.Add(strip);
            });
        }
    }
}
