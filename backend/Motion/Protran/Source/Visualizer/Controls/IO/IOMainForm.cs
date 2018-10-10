// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOMainForm.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The io main form.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.IO
{
    using System;

    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Win32.Wrapper;
    using Gorba.Motion.Protran.Visualizer.Data.IO;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The main window of IO in Protran Visualizer.
    /// </summary>
    public partial class IOMainForm : MdiChildBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IOMainForm"/> class.
        /// </summary>
        public IOMainForm()
        {
            this.InitializeComponent();
            this.Icon = ShellFileInfo.GetFileIcon(ApplicationHelper.GetEntryAssemblyLocation(), false, false);
        }

        /// <summary>
        /// The on load.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.ioTabControl1.Configure(ServiceLocator.Current.GetInstance<IIOVisualizationService>());
        }
    }
}
