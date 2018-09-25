// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimulationManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SimulationManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Simulation
{
    using System;
    using System.ComponentModel;
    using System.Linq;

    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Simulation.Composers;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    using NLog;

    /// <summary>
    /// The manager that takes care of the separate simulation window showing the DirectX renderer output.
    /// </summary>
    public class SimulationManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IMediaShell shell;

        private LayoutComposer rootComposer;

        private ISimulator simulator;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimulationManager"/> class.
        /// </summary>
        /// <param name="shell">
        /// The shell.
        /// </param>
        public SimulationManager(IMediaShell shell)
        {
            this.shell = shell;

            this.shell.PropertyChanged += this.ShellOnPropertyChanged;
        }

        private void ShowSimulation()
        {
            Logger.Info("Showing simulation");

            var virtualDisplay = this.shell.MediaApplicationState.CurrentVirtualDisplay;
            var width = virtualDisplay.Width.Value;
            var height = virtualDisplay.Height.Value;
            if (this.simulator != null && (this.simulator.Width != width || this.simulator.Height != height))
            {
                // we have to recreate the simulator, therefore first hide it
                Logger.Debug("Simulator has the wrong dimensions, recreating it");
                this.HideSimulation();
            }

            if (this.simulator == null)
            {
                this.simulator = new DirectXSimulator();
                this.simulator.Stopped += this.SimulatorOnStopped;
                this.simulator.Configure(width, height);
                this.simulator.Start();

                this.shell.MediaApplicationState.PropertyChanged += this.MediaApplicationStateOnPropertyChanged;
                this.shell.Closed += this.ShellOnClosed;
            }

            this.ReloadLayout();
        }

        private void HideSimulation()
        {
            if (this.rootComposer != null)
            {
                this.rootComposer.Dispose();
                this.rootComposer = null;
            }

            if (this.simulator == null)
            {
                return;
            }

            Logger.Info("Hiding simulation");
            this.shell.MediaApplicationState.PropertyChanged -= this.MediaApplicationStateOnPropertyChanged;
            this.shell.Closed -= this.ShellOnClosed;

            this.simulator.Stopped -= this.SimulatorOnStopped;
            this.simulator.Stop();
            this.simulator = null;
        }

        private void ReloadLayout()
        {
            Logger.Debug("Reloading the current layout");

            var layout = this.shell.MediaApplicationState.CurrentLayout as LayoutConfigDataViewModel;
            if (layout == null)
            {
                Logger.Warn("Layout is not a standard layout config");
                return;
            }

            var virtualDisplay = this.shell.MediaApplicationState.CurrentVirtualDisplay;
            var width = virtualDisplay.Width.Value;
            var height = virtualDisplay.Height.Value;
            var resolution = layout.Resolutions.FirstOrDefault(r => r.Width.Value == width && r.Height.Value == height);
            if (resolution == null)
            {
                Logger.Warn("Couldn't find resolution {0}x{1} in {2}", width, height, layout.Name.Value);
                return;
            }

            if (this.rootComposer == null)
            {
                this.rootComposer = new LayoutComposer();
            }

            this.rootComposer.Load(resolution, this.shell.MediaApplicationState.CurrentPhysicalScreen.Type.Value);
        }

        private void ShellOnClosed(object sender, EventArgs eventArgs)
        {
            this.HideSimulation();
        }

        private void ShellOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "SimulationIsVisible")
            {
                return;
            }

            if (this.shell.SimulationIsVisible)
            {
                this.ShowSimulation();
            }
            else
            {
                this.HideSimulation();
            }
        }

        private void MediaApplicationStateOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CurrentProject":
                    this.shell.SimulationIsVisible = false;
                    return;
                case "CurrentSection":
                case "CurrentLayout":
                    if (this.shell.MediaApplicationState.CurrentPhysicalScreen.Type.Value != PhysicalScreenType.TFT)
                    {
                        this.shell.SimulationIsVisible = false;
                        return;
                    }

                    this.ReloadLayout();
                    return;
            }
        }

        private void SimulatorOnStopped(object sender, EventArgs eventArgs)
        {
            this.shell.SimulationIsVisible = false;
        }
    }
}
