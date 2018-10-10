// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteLogInfoPartViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemoteLogInfoPartViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.App
{
    using Gorba.Center.Diag.Core.Resources;

    /// <summary>
    /// The view model for the application part that shows the log of the given application.
    /// </summary>
    public class RemoteLogInfoPartViewModel : AppInfoPartViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteLogInfoPartViewModel"/> class.
        /// </summary>
        /// <param name="application">
        /// The application.
        /// </param>
        public RemoteLogInfoPartViewModel(RemoteAppViewModel application)
            : base(application)
        {
            this.Name = DiagStrings.AppInfoPart_RemoteLog;
        }
    }
}