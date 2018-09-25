// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimulationChannel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Channels
{
    using System;
    using System.IO;
    using System.Threading;

    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Motion.Protran.Ibis.Simulation;

    /// <summary>
    /// This object performs a complete IBIS simulation
    /// (acting as the IBIS master) basing on a specific
    /// file containing recorded IBIS telegrams.
    /// </summary>
    public class SimulationChannel : IbisChannel
    {
        #region VARIABLES
        /// <summary>
        /// Container of all the parameters required
        /// to perform an IBIS simulation.
        /// </summary>
        private readonly SimulationConfig simulationConfig;

        /// <summary>
        /// The object tasked to read the IBIS telegrams from the file.
        /// </summary>
        private readonly IbisFileReader reader;

        /// <summary>
        /// The event that manages the possibilities to simulate or not a telegram.
        /// </summary>
        private readonly AutoResetEvent waitEvent = new AutoResetEvent(false);

        /// <summary>
        /// Thread tasked to run the simulation
        /// until its end.
        /// </summary>
        private Thread threadSimulation;

        /// <summary>
        /// Flag that tells if the simulation has to run or not.
        /// </summary>
        private bool running;
        #endregion VARIABLES

        /// <summary>
        /// Initializes a new instance of the <see cref="SimulationChannel"/> class.
        /// </summary>
        /// <param name="configContext">
        /// The config context.
        /// </param>
        public SimulationChannel(IIbisConfigContext configContext)
            : base(configContext)
        {
            this.running = false;
            this.IsRunning = false;
            this.threadSimulation = null;
            this.simulationConfig = configContext.Config.Sources.Simulation;

            // make the path absolute
            this.simulationConfig.SimulationFile =
                configContext.GetAbsolutePathRelatedToConfig(this.simulationConfig.SimulationFile);
            this.reader = IbisFileReader.Create(this.simulationConfig);

            Logger.Info("Created {0} for {1}", this.reader.GetType().Name, this.simulationConfig.SimulationFile);
        }

        /// <summary>
        /// Gets a value indicating whether the
        /// simulation is running or not.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Starts the simulation.
        /// If you want to know if the simulation is running,
        /// call the property "IsRunning".
        /// <exception cref="FileNotFoundException">If the file from
        /// which take the simulation does not exist.</exception>
        /// </summary>
        protected override void DoOpen()
        {
            base.DoOpen();
            if (this.IsRunning)
            {
                // the simulation is already running.
                // I avoid to start it twice.
                return;
            }

            if (!File.Exists(this.simulationConfig.SimulationFile))
            {
                // the file from which take the simulation
                // does not exist.
                // I can run nothing.
                throw new FileNotFoundException(
                    "Couldn't find simulation file: " + this.simulationConfig.SimulationFile);
            }

            this.threadSimulation = new Thread(this.RunSimulation) { Name = "SimulationChannel" };
            this.threadSimulation.Start();
            this.IsRunning = true;
            this.running = true;
        }

        /// <summary>
        /// Stops the simulation.
        /// If you want to know if the simulation is running,
        /// call the property "IsRunning".
        /// </summary>
        protected override void DoClose()
        {
            base.DoClose();
            if (!this.IsRunning)
            {
                // the simulation is alredy stopped.
                // I avoid to stop it twice.
                return;
            }

            this.running = false;
            this.waitEvent.Set();
        }

        /// <summary>
        /// Subclasses implement this method to send an answer to the IBIS master.
        /// </summary>
        /// <param name="bytes">
        /// The buffer to send.
        /// </param>
        /// <param name="offset">
        /// The offset inside the buffer.
        /// </param>
        /// <param name="length">
        /// The number of bytes to send starting from <see cref="offset"/>.
        /// </param>
        protected override void SendAnswer(byte[] bytes, int offset, int length)
        {
            // now with a simple sleep,
            // I simulate the reply operation through the serial port.
            Thread.Sleep(50);
        }

        /// <summary>
        /// Runs the simulation.
        /// </summary>
        private void RunSimulation()
        {
            var ok = this.WaitBeforeStart();
            if (!ok)
            {
                // the simulation cannot start.
                this.IsRunning = false;
                return;
            }

            this.reader.Open();
            try
            {
                // now, I can really perform the simulation
                // reading the telegrams from the file and so on...
                int counterSimulations = 0;
                bool infiniteSimulations = this.simulationConfig.TimesToRepeat == 0;
                while (infiniteSimulations || counterSimulations++ != this.simulationConfig.TimesToRepeat)
                {
                    // if I reach this line of code, it means that
                    // now the simulation can really start.
                    this.Logger.Info("Simulation number " + counterSimulations + " started.\n.");

                    while (this.reader.ReadNext())
                    {
                        this.ManageTelegram(this.reader.CurrentTelegram);

                        var waitTime = this.simulationConfig.IntervalBetweenTelegrams == null
                                           ? this.reader.CurrentWaitTime
                                           : this.simulationConfig.IntervalBetweenTelegrams.Value;
                        this.waitEvent.WaitOne((int)waitTime.TotalMilliseconds, false);

                        if (!this.running)
                        {
                            // during the simulation, this thread
                            // was ordered to be closed.
                            // ok, I close it.
                            this.IsRunning = false;
                            return;
                        }
                    }

                    this.Logger.Info("Simulation number " + counterSimulations + " ended.\n");
                    if (!this.reader.Reset())
                    {
                        this.Logger.Info("Can't reset file reader, stopping simulation.\n");
                        break;
                    }
                }
            }
            finally
            {
                this.reader.Close();
            }

            this.IsRunning = false;
        }

        /// <summary>
        /// Asks to the user to really start with the simulation
        /// and wait the configured seconds before start.
        /// </summary>
        /// <returns>
        /// True when the simulation can really start.
        /// </returns>
        private bool WaitBeforeStart()
        {
            // second, I've to wait the initial delay.
            if (this.simulationConfig.InitialDelay <= TimeSpan.Zero)
            {
                return true;
            }

            this.Logger.Info("The simulation will start in {0}", this.simulationConfig.InitialDelay);

            return !this.waitEvent.WaitOne((int)this.simulationConfig.InitialDelay.TotalMilliseconds, false);
        }
    }
}
