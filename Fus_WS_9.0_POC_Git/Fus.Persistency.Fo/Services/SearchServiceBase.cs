using Ws.Dicom.Persistency.Interfaces.Services;
using Ws.Dicom.Persistency.Fo.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ws.Dicom.Persistency.Fo.Services
{
    abstract class SearchServiceBase : ISearchService
    {
        private readonly SearchServiceSettingsBase _settings;

        protected readonly SemaphoreSlim _findStudiesSemaphore;
        protected readonly SemaphoreSlim _getStudySeriesSemaphore;
        protected readonly SemaphoreSlim _getSeriesImageSemaphore;

        public SearchServiceBase(SearchServiceSettingsBase settings)
        {
            _settings = settings;

            _findStudiesSemaphore = new SemaphoreSlim(MaxFindStudiesRequests, MaxFindStudiesRequests);
            _getStudySeriesSemaphore = new SemaphoreSlim(MaxFindStudiesRequests, MaxGetStudySeriesRequests);
            _getSeriesImageSemaphore = new SemaphoreSlim(MaxFindStudiesRequests, MaxGetSeriesImageRequests);
        }

        public int MaxFindStudiesRequests => _settings.MaxFindStudiesRequests;
        public int MaxGetStudySeriesRequests => _settings.MaxGetStudySeriesRequests;
        public int MaxGetSeriesImageRequests => _settings.MaxGetSeriesImageRequests;

        public async Task GetStudySeriesAsync(GetStudySeriesRequest request, CancellationToken ct)
        {
            Validator.ValidateObject(request.Study, new ValidationContext(request.Study), true);

            await GetStudySeriesImpAsync(request, ct);
        }

        public async Task GetSeriesImageAsync(GetSeriesImageRequest request, CancellationToken ct)
        {
            foreach (var series in request.Series)
                Validator.ValidateObject(series, new ValidationContext(series), true);

            await GetSeriesImageImpAsync(request, ct);
        }

        public async Task GetSeriesImagesAsync(GetSeriesImagesRequest request, CancellationToken ct)
        {
            foreach (var series in request.Series)
                Validator.ValidateObject(series, new ValidationContext(series), true);

            await GetSeriesImagesImpAsync(request, ct);
        }

        public abstract Task FindStudiesAsync(FindStudiesRequest request, CancellationToken ct);

        internal abstract Task GetStudySeriesImpAsync(GetStudySeriesRequest request, CancellationToken ct);
        internal abstract Task GetSeriesImageImpAsync(GetSeriesImageRequest request, CancellationToken ct);
        internal abstract Task GetSeriesImagesImpAsync(GetSeriesImagesRequest request, CancellationToken ct);
    }
}
