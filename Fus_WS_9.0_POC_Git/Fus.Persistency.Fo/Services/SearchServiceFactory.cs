using Dicom.Network;
using Ws.Extensions.Patterns;
using Ws.Dicom.Persistency.Interfaces.Services;
using Ws.Dicom.Persistency.Fo.Settings;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ws.Dicom.Persistency.Fo.Services
{
    class SearchServiceFactory : ISearchServiceFactory
    {
        private const string FileSysServerName = "DISK";

        private static readonly ILogger _logger = Log.ForContext<SearchServiceFactory>();

        private readonly IDicomServer _storeScp;
        private readonly FusPersistencyFoSettings _settings;

        public SearchServiceFactory(Wrapper<FusPersistencyFoSettings> settings)
        {
            _settings = settings.Value;

            try
            {
                CStoreScp.StorageCleanup(_settings.StorageHiWaterMarkGb, _settings.StorageLoWaterMarkGb);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Dicom storage cleanup failed");
            }

            _storeScp = DicomServer.Create<CStoreScp>(_settings.CStoreScpPort);
        }

        public async Task GetAvailableServersAsync(IProgress<string> progress)
        {
            List<Task<bool>> tasks2 = new List<Task<bool>>();

            foreach (var kvp in _settings.DicomServices)
                tasks2.Add(ConfigureService(kvp));

            var tasks = _settings.DicomServices.ToDictionary(kvp => ConfigureService(kvp), kvp => kvp.Key);

            while (tasks.Any())
            {
                var done = await Task.WhenAny(tasks.Keys);

                if (done.Result)
                    progress.Report(tasks[done]);

                tasks.Remove(done);
            }

            if (_settings.FileSysService != null)
                progress.Report(FileSysServerName);
        }

        public bool IsFileSysService(string serverName) => FileSysServerName == serverName;

        public ISearchService CreateService(string serverName)
        {
            if (IsFileSysService(serverName))
                throw new ApplicationException($"{serverName} is a File System Service. Use {nameof(CreateFileSysSearchService)} method instead");

            DicomSearchServiceSettings serviceSettings;

            if (!_settings.DicomServices.TryGetValue(serverName, out serviceSettings))
                throw new ArgumentException($"Server {serverName} not listed", nameof(serverName));

            return new DicomSearchService(serviceSettings);
        }

        public IFileSysSearchService CreateFileSysSearchService(string searchDir)
        {
            return new FileSysSearchService(searchDir, _settings.FileSysService);
        }

        private static async Task<bool> ConfigureService(KeyValuePair<string, DicomSearchServiceSettings> kvp)
        {
            var serverName = kvp.Key;
            var serviceSettings = kvp.Value;

            _logger.Debug("Sending echo to {Server}", serverName);

            var client = serviceSettings.DicomSettings.CreateClient();
            var request = new DicomCEchoRequest();

            var ok = false;

            request.OnResponseReceived += (rq, resp) =>
            {
                if (resp.Status.State == DicomState.Success)
                    ok = true;
            };

            using (var cts = new CancellationTokenSource())
            {
                var timeoutMs = serviceSettings.DicomSettings.EchoTimeoutMs;
                try
                {
                    await client.AddRequestAsync(request).ConfigureAwait(false);
                    cts.CancelAfter(timeoutMs);
                    var sendTask = client.SendAsync(cts.Token);
                    var t = await Task.WhenAny(sendTask, Task.Delay(timeoutMs));

                    if (t != sendTask)
                        _logger.Warning("{Server}: DicomCEchoRequest timeout ({EchoTimeoutMs}ms)", kvp.Key, timeoutMs);
                    else
                        CStoreScp.AllowedCalledAE.Add(kvp.Value.DicomSettings.AET);
                }
                catch (OperationCanceledException ex)
                {
                    _logger.Warning("{Server}: DicomCEchoRequest timeout ({EchoTimeoutMs}ms)", kvp.Key, timeoutMs);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "{Server}: DicomCEchoRequest error", kvp.Key);
                }
            }

            return ok;
        }
    }
}
