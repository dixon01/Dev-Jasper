namespace Core
{
    using System;
    using System.Activities;
    using System.Collections.Generic;
    using System.Threading;

    using Microsoft.Owin.Hosting;

    public class PerturbationManagerServer
    {
        private static readonly Simulation SimulationActivity = new Simulation();

        private CancellationTokenSource cancellationTokenSource;

        public PerturbationManagerServer(Startup startup)
        {
            this.Startup = startup;
        }

        protected Startup Startup { get; private set; }

        protected WorkflowApplication Simulation { get; private set; }

        public Action ServerStarted { get; set; }

        public Action ServerStopped { get; set; }

        public void Start()
        {
            const string Uri = "http://localhost:8080/";

            if (this.Startup.FeedMessage != null)
            {
                var inputs = new Dictionary<string, object> { { "Startup", this.Startup } };
                this.Simulation = new WorkflowApplication(SimulationActivity, inputs);
                this.Simulation.Run();
            }

            this.cancellationTokenSource = new CancellationTokenSource();

            using (WebApp.Start(Uri, this.Startup.Configuration))
            {
                var serverStarted = this.ServerStarted;
                if (serverStarted != null)
                {
                    serverStarted();
                }

                this.cancellationTokenSource.Token.WaitHandle.WaitOne();
                var serverStopped = this.ServerStopped;
                if (serverStopped == null)
                {
                    return;
                }

                serverStopped();
            }
        }

        public void Stop()
        {
            if (this.Simulation != null)
            {
                this.Simulation.Cancel();
            }

            if (this.cancellationTokenSource == null)
            {
                return;
            }

            this.cancellationTokenSource.Cancel();
        }
    }
}