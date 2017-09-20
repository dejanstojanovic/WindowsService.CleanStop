using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using log4net;

namespace WindowsService.CleanStop
{
    public partial class Service : ServiceBase
    {
        private readonly ILog log;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly CancellationToken cancellationToken;
        private readonly List<ManualResetEvent> resetEvents;

        public Service()
        {

            this.log = log4net.LogManager.GetLogger(this.GetType().FullName);

            this.cancellationTokenSource = new CancellationTokenSource();
            this.cancellationToken = cancellationTokenSource.Token;
            this.resetEvents = new List<ManualResetEvent>();

            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            this.StartService();
        }

        protected override void OnStop()
        {
            log.Info("Stopping service...");
            if (!int.TryParse(ConfigurationManager.AppSettings["service.stoptimeout"], out int serviceStopTimeout))
            {
                serviceStopTimeout = 3000;
            }
            this.cancellationTokenSource.Cancel();
            WaitHandle.WaitAll(resetEvents.Select(e => e as WaitHandle).ToArray(), serviceStopTimeout); //Wait for all workers to exit
            log.Info("Service stopped.");
        }

        public void StartService()
        {
            log.Info("Service start invoked...");
            if (!int.TryParse(ConfigurationManager.AppSettings["serice.workersnumber"], out int workersNumber))
            {
                workersNumber = 1;
            }

            log.Info($"Workers count {workersNumber}");

            for (int i = 0; i < workersNumber; i++)
            {
                var resetEvent = new ManualResetEvent(false);
                resetEvents.Add(resetEvent);
                StartWorker(resetEvent);
            }

            log.Info("Service start finished.");
        }

        public void StartWorker(ManualResetEvent resetEvent)
        {
            Task.Run(() =>
            {
                log.Info("Starting worker...");

                while (!cancellationToken.IsCancellationRequested)
                {
                    //Logic here

                    /* Simulation code start */
                    Thread.Sleep(5000);
                    log.Info("Worker done work");
                    /* Simulation code end */

                    Thread.Sleep(100); //Avoid high CPU
                }

                resetEvent.Set();
                log.Info("Stopping worker...");
            }, this.cancellationToken);
        }

    }
}
