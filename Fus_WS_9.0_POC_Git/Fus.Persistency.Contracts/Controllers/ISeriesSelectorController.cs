using Ws.Dicom.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Ws.Dicom.Persistency.Interfaces.Controllers
{
    public interface ISeriesSelectorController
    {
        event EventHandler<ICollection<Series>> SeriesSelected;

        ICollection<KeyValuePair<Series, BitmapSource>> LoadedSeries { get; }
    }
}
