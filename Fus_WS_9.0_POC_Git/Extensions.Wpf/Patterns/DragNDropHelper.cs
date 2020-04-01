using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Ws.Extensions.UI.Wpf.Patterns
{
    public class DragNDropHelper
    {
        public class DragNDropData
        {
            public object Data { get; set; }
            public DependencyObject DragSource { get; set; }
            public DragDropEffects Effects { get; set; }
        }

        private readonly Point _startPoint;
        private readonly string _dataObjectFormat;
        private readonly Func<object, MouseEventArgs, DragNDropData> _dataCreator;

        /// <summary>
        /// Supposed to be used upon <see cref="UIElement.PreviewMouseLeftButtonDown" event/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public DragNDropHelper(object sender, MouseButtonEventArgs e, string dataObjectFormat, Func<object, MouseEventArgs, DragNDropData> dataCreator)
        {
            // Store the mouse position
            _startPoint = e.GetPosition(null);
            _dataObjectFormat = dataObjectFormat;
            _dataCreator = dataCreator;
        }

        /// <summary>
        /// Supposed to be called upon <see cref="UIElement.PreviewMouseMove" event/> 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PreviewMouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = _startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                var ddd = _dataCreator(sender, e);
                if (ddd != null)
                {
                    // Initialize the drag & drop operation
                    DataObject dragData = new DataObject(_dataObjectFormat, ddd.Data);
                    DragDrop.DoDragDrop(ddd.DragSource, dragData, ddd.Effects);
                }
            }
        }
    }
}
