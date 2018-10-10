// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TabControl.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TabControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabiMerge
{
    using System;
    using System.Windows.Forms;

    using Gorba.Common.Configuration.Core;

    using NLog;

    /// <summary>
    /// Tab control for Cu5 and Protran files merge
    /// </summary>
    public partial class TabControl : UserControl
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ConfigManager<AbuDhabiMergeConfig> configManager = new ConfigManager<AbuDhabiMergeConfig>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TabControl"/> class.
        /// </summary>
        public TabControl()
        {
            this.InitializeComponent();

            this.configManager.FileName = "AbuDhabiMerge.xml";

            try
            {
                this.mergeControlCu.Configure(this.configManager.Config);
                this.mergeControlTopBox.Configure(this.configManager.Config);
            }
            catch (Exception ex)
            {
                // should only happen in designer mode
                Logger.Error(ex,"Could not load config");
            }
        }
    }
}
