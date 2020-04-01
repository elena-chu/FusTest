using Dicom;
using Ws.Dicom.Interfaces.Entities;
using Ws.Dicom.Persistency.Interfaces.Services;
using Ws.Dicom.Persistency.Fo.Entities;
using Ws.Dicom.Persistency.Fo.Settings;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Ws.Dicom.Persistency.Fo.Services
{
    class FileSysSearchService : SearchServiceBase, IFileSysSearchService
    {
        private static readonly ILogger _logger = Log.ForContext<FileSysSearchService>();

        public string SearchDir { get; }

        private readonly FileSysSearchServiceSettings _settings;
        private readonly Task _findFileSysStudiesTask;

        private Dictionary<string, FileSysStudyImp> _fileSysStudies;

        public FileSysSearchService(string searchDir, FileSysSearchServiceSettings settings) : base(settings.BaseSettings)
        {
            SearchDir = searchDir;
            _settings = settings;

            _findFileSysStudiesTask = FindFileSysStudiesAsync();
        }

        public override async Task FindStudiesAsync(FindStudiesRequest request, CancellationToken ct)
        {
            var studiesInDir = await WaitForStudiesInDirAsync(ct);

            var query = studiesInDir.Values.AsQueryable();

            var patientName = string.Empty;
            if (!string.IsNullOrWhiteSpace(request.Family))
                patientName += request.Family;
            if (!string.IsNullOrWhiteSpace(request.FirstName))
            {
                if (string.IsNullOrWhiteSpace(patientName))
                    patientName += '*';

                patientName += '^' + request.FirstName;
            }

            if (!string.IsNullOrWhiteSpace(patientName))
                query = query.Where(s => Where(s.PatientName, patientName, StringComparison.InvariantCultureIgnoreCase));

            if (!string.IsNullOrWhiteSpace(request.PatientId))
                query = query.Where(s => Where(s.PatientId, request.PatientId, StringComparison.Ordinal));

            if (!string.IsNullOrWhiteSpace(request.Description))
                query = query.Where(s => Where(s.StudyDescription, request.Description, StringComparison.InvariantCultureIgnoreCase));

            if (request.From.HasValue || request.To.HasValue)
            {
                var from = request.From.GetValueOrDefault(DateTime.MinValue);
                var to = request.To.GetValueOrDefault(DateTime.MaxValue);

                query = query.Where(s => s.StudyDate.HasValue && s.StudyDate >= from && s.StudyDate <= to);
            }

            Parallel.ForEach(query, study => request.RaiseStudyFound(study));
        }

        internal override async Task GetStudySeriesImpAsync(GetStudySeriesRequest request, CancellationToken ct)
        {
            FileSysStudyImp fsStudy = request.Study as FileSysStudyImp;
            if (fsStudy == null)
                throw new ApplicationException($"The {GetType()} implementation can receive study instances of {typeof(FileSysStudyImp)} only");

            var createSeries = new ActionBlock<string>(async seriesDir =>
            {
                var series = await CreateFromSeriesDirAsync(seriesDir, (dicomFile) =>
                {
                    return dicomFile.Dataset.CreateObject(() => new FileSysSeriesImp(seriesDir));
                });

                if (series != null)
                {
                    if (series.NumberOfSeriesRelatedInstances < 1)
                        series.NumberOfSeriesRelatedInstances = Directory.EnumerateFiles(seriesDir).Count();

                    series.Study = request.Study;
                    request.RaiseSeriesGot(series);
                }

            }, new ExecutionDataflowBlockOptions { CancellationToken = ct, MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded });


            foreach (var seriesDir in fsStudy.SeriesDirs)
                await createSeries.SendAsync(seriesDir, ct);

            createSeries.Complete();
            await createSeries.Completion;
        }

        internal override async Task GetSeriesImageImpAsync(GetSeriesImageRequest request, CancellationToken ct)
        {
            var createImage = new ActionBlock<Series>(async ser =>
            {
                var fsSeries = ser as FileSysSeriesImp;
                if (fsSeries == null)
                {
                    _logger.Warning("{service} got unexpected type of series object {@Series}", GetType(), ser);
                    return;
                }

                var image = await CreateFromSeriesDirAsync(fsSeries.SeriesDir, (dicomFile) => { return dicomFile.CreateImage(); });
                if (image != null)
                {
                    if (ser.Orientation == FDCSeriesOrientation.eFDC_NO_ORIENTATION &&
                        image.Orientation != FDCSeriesOrientation.eFDC_NO_ORIENTATION)
                        ser.Orientation = image.Orientation;

                    request.RaiseImageGot(image);
                }
            }, new ExecutionDataflowBlockOptions { CancellationToken = ct, MaxDegreeOfParallelism = 1 });


            foreach (var ser in request.Series)
                await createImage.SendAsync(ser, ct);

            createImage.Complete();
            await createImage.Completion;
        }

        internal override async Task GetSeriesImagesImpAsync(GetSeriesImagesRequest request, CancellationToken ct)
        {
            double progress = 0;
            foreach (var ser in request.Series)
            {
                var fsSeries = ser as FileSysSeriesImp;
                if (fsSeries == null)
                {
                    _logger.Warning("{service} got unexpected type of series object {@Series}", GetType(), ser);
                    continue;
                }

                fsSeries.ImagesUri = fsSeries.SeriesDir;

                progress++;

                request.RaiseProgress((int)(progress / request.Series.Count() * 100));
                request.RaiseSeriesDone(fsSeries);
            }

            await Task.FromResult(true);
        }

        private static bool Where(string actual, string expected, StringComparison cmp)
        {
            if (string.IsNullOrWhiteSpace(actual))
                return false;

            if (expected.StartsWith("*") && expected.EndsWith("*"))
                return actual.IndexOf(expected.Trim('*'), cmp) >= 0;
            else if (expected.StartsWith("*"))
                return actual.EndsWith(expected.TrimStart('*'), cmp);
            else if (expected.EndsWith("*"))
                return actual.StartsWith(expected.TrimEnd('*'), cmp);
            else
                return actual.Equals(expected, cmp);
        }

        private static Task<T> CreateFromSeriesDirAsync<T>(string seriesDir) where T : new() =>
            CreateFromSeriesDirAsync(seriesDir, (dicomFile) => { return dicomFile.Dataset.CreateObject<T>(); });

        private static async Task<T> CreateFromSeriesDirAsync<T>(string seriesDir, Func<DicomFile, T> factory)
        {
            try
            {
                var seriesImagesFiles = Directory.EnumerateFiles(seriesDir);
                if (!seriesImagesFiles.Any()) // no files in directory
                    return default(T);

                var spokesmanFile = seriesImagesFiles.ElementAt(seriesImagesFiles.Count() / 2);
                var dicomFile = await DicomFile.OpenAsync(spokesmanFile);
                if (dicomFile == null)
                {
                    _logger.Error("Failed to create {obj} from series directory {path}", typeof(T), seriesDir);
                    return default(T);
                }
                var obj = factory(dicomFile);
                //obj.SeriesDirs.AddLast(seriesDir);
                return obj;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to create {obj} from series directory {path}", typeof(T), seriesDir);
                return default(T);
            }
        }

        private async Task FindFileSysStudiesAsync()
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            var studiesInDir = new ConcurrentDictionary<string, FileSysStudyImp>();

            var buffer = new BufferBlock<string>(new DataflowBlockOptions { CancellationToken = cts.Token });
            var createStudy = new TransformBlock<string, FileSysStudyImp>(async seriesDir =>
            {
                var study = await CreateFromSeriesDirAsync<FileSysStudyImp>(seriesDir);
                if (study == null)
                    return null;

                study.SeriesDirs.AddLast(seriesDir);
                return study;
            }, new ExecutionDataflowBlockOptions { CancellationToken = cts.Token, MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded });

            var addStudy = new ActionBlock<FileSysStudyImp>(async study =>
            {
                if (string.IsNullOrWhiteSpace(study?.StudyInstanceUid))
                {
                    if (study != null)
                        _logger.Warning("The study {@Study} created from {path} doesn't have StudyInstanceUid", study);

                    return;
                }

                studiesInDir.AddOrUpdate(study.StudyInstanceUid, study, (k, v) =>
                {
                    v.SeriesDirs.AddLast(study.SeriesDirs.ElementAt(0));
                    return v;
                });

                await Task.FromResult(true);
            }, new ExecutionDataflowBlockOptions { CancellationToken = cts.Token, MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded });

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            buffer.LinkTo(createStudy, linkOptions);
            createStudy.LinkTo(addStudy, linkOptions);

            var leafDirs = Directory.EnumerateDirectories(SearchDir, "*", SearchOption.AllDirectories)
                .Where(dir => !Directory.EnumerateDirectories(dir).Any());

            foreach (var leafDir in leafDirs)
                await buffer.SendAsync(leafDir);

            buffer.Complete();

            //if (await Task.WhenAny(addStudy.Completion, Task.Delay(TimeSpan.FromSeconds(_settings.DirScanTimeoutSec))) == addStudy.Completion)
            {
                // Task completed within timeout.
                // Consider that the task may have faulted or been canceled.
                // We re-await the task so that any exceptions/cancellation is rethrown.
                await addStudy.Completion;

            }
            //else
            //{
            //    cts.Cancel();
            //    await addStudy.Completion;
            //}

            _fileSysStudies = studiesInDir.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            Parallel.ForEach(_fileSysStudies, study =>
            {
                if (!study.Value.NumberOfStudyRelatedSeries.HasValue)
                    study.Value.NumberOfStudyRelatedSeries = (uint)study.Value.SeriesDirs.Count;
            });
        }

        private async Task<Dictionary<string, FileSysStudyImp>> WaitForStudiesInDirAsync(CancellationToken ct)
        {
            if (await Task.WhenAny(_findFileSysStudiesTask, Task.Delay(-1, ct)) == _findFileSysStudiesTask)
            {
                // Task completed within timeout.
                // Consider that the task may have faulted or been canceled.
                // We re-await the task so that any exceptions/cancellation is rethrown.
                await _findFileSysStudiesTask;
            }
            else
            {
                ct.ThrowIfCancellationRequested();
            }

            return _fileSysStudies;
        }
    }
}
