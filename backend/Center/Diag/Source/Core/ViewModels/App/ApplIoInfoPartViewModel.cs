// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplIoInfoPartViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplIoInfoPartViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.App
{
    using Gorba.Center.Diag.Core.Resources;

    /// <summary>
    /// The view model for the application part that shows all I/O's of a given application.
    /// </summary>
    public class ApplIoInfoPartViewModel : AppInfoPartViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplIoInfoPartViewModel"/> class.
        /// </summary>
        /// <param name="application">
        /// The application.
        /// </param>
        public ApplIoInfoPartViewModel(RemoteAppViewModel application)
            : base(application)
        {
            this.Name = DiagStrings.AppInfoPart_ApplicationIO;
        }
    }
}