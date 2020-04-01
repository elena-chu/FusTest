using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.Menus.Builders;

namespace WpfUI.Menus.ViewModels
{
    /// <summary>
    /// Menu for View actions
    /// </summary>
    public class ViewMenuViewModel : ToolsMenuViewModelBase
    {
        private readonly ViewActionBuilder _builder;

        public ViewMenuViewModel(ViewActionBuilder builder)
        {
            _builder = builder;
            Initialize();
        }

        public LayoutActionViewModel Layout { get; set; }

        protected override void CreateChildActions()
        {
            Layout = _builder.BuildLayout();
            Actions.Add(Layout);
        }

    }
}
