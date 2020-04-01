using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ws.Dicom.Persistency.Interfaces.Services
{
    public interface ISearchService
    {
        int MaxFindStudiesRequests { get; }
        int MaxGetSeriesImageRequests { get; }
        int MaxGetStudySeriesRequests { get; }

        Task FindStudiesAsync(FindStudiesRequest request, CancellationToken ct);

        Task GetStudySeriesAsync(GetStudySeriesRequest request, CancellationToken ct);

        Task GetSeriesImageAsync(GetSeriesImageRequest request, CancellationToken ct);

        Task GetSeriesImagesAsync(GetSeriesImagesRequest request, CancellationToken ct);
    }
}
