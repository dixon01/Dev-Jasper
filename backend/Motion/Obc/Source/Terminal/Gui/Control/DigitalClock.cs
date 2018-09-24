// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DigitalClock.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VMxDigitalClock type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Gui.Control
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using Gorba.Common.Gioom.Core.Utility;
    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Obc.Terminal.Gui.Utility;

    /// <summary>
    /// The digital clock shown on VM.x.
    /// </summary>
    public partial class VMxDigitalClock : UserControl
    {
        private static readonly object Locker = new object();

        private static readonly List<IUpdateable> Updateables = new List<IUpdateable>();

        private readonly PortListener gpsCoverage;

        private bool hasGps;

        /// <summary>
        /// Initializes a new instance of the <see cref="VMxDigitalClock"/> class.
        /// </summary>
        public VMxDigitalClock()
        {
            this.InitializeComponent();

            this.gpsCoverage = new PortListener(MediAddress.Broadcast, "GpsCoverage");
            this.gpsCoverage.ValueChanged += this.GpsCoverageOnValueChanged;
            this.gpsCoverage.Start(TimeSpan.FromSeconds(5));

            ScreenUtil.Adapt4Ihmi(this, true, false);
        }

        /// <summary>
        /// Adds an IUpdateable object
        /// Make sure that the updateable object will handle fast the Update() method.
        /// Otherwise it may block the hole system!
        /// </summary>
        /// <param name = "updateable">
        /// The item to be updated
        /// </param>
        /// <returns>
        ///   true: if the updatable was added
        ///   false: if it couldn't be added. -> it was already added before
        /// </returns>
        internal static bool AddUpdateable(IUpdateable updateable)
        {
            lock (Locker)
            {
                if (Updateables.Contains(updateable) == false)
                {
                    Updateables.Add(updateable);
                    return true;
                }
            }

            return false;
        }

        private static void UpdateUpdateables()
        {
            lock (Locker)
            {
                foreach (var updateable in Updateables)
                {
                    updateable.SecondUpdate();
                }
            }
        }

        private void GpsCoverageOnValueChanged(object sender, EventArgs eventArgs)
        {
            this.hasGps = FlagValues.True.Equals(this.gpsCoverage.Value);
        }

        private void Timer1Tick(object sender, EventArgs e)
        {
            if (this.hasGps)
            {
                this.lblTime.Text = TimeProvider.Current.Now.ToString("HH:mm:ss"); // MLHIDE
            }
            else
            {
                this.lblTime.Text = ml.ml_string(75, "No GPS");
            }

            UpdateUpdateables();
        }
    }
}