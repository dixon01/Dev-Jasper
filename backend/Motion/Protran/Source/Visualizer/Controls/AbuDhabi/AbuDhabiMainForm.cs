// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbuDhabiMainForm.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AbuDhabiMainForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.AbuDhabi
{
    using System.Windows.Forms;

    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Win32.Wrapper;
    using Gorba.Motion.Protran.AbuDhabi;
    using Gorba.Motion.Protran.Visualizer.Controls.Ibis;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Main window of Abu Dhabi Protran Visualizer
    /// </summary>
    public partial class AbuDhabiMainForm : MdiChildBase
    {
        private IbisMainForm ibisMainForm;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbuDhabiMainForm"/> class.
        /// </summary>
        public AbuDhabiMainForm()
        {
            this.InitializeComponent();
            this.Icon = ShellFileInfo.GetFileIcon(ApplicationHelper.GetEntryAssemblyLocation(), false, false);
        }

        /// <summary>
        /// The create ibis form.
        /// </summary>
        /// <returns>
        /// The <see cref="Form"/>.
        /// </returns>
        public Form CreateIbisForm()
        {
            if (this.ibisMainForm != null)
            {
                return null;
            }

            var protocol = ServiceLocator.Current.GetInstance<AbuDhabiProtocol>();
            if (!protocol.Config.Ibis.Enabled)
            {
                return null;
            }

            this.ibisMainForm = new IbisMainForm();
            return this.ibisMainForm;
        }
    }
}
