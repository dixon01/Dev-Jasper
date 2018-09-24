// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreInstallationActionPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InstallationActionPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Conclusion
{
    using Gorba.Center.Admin.Core.Resources;

    /// <summary>
    /// The software versions part controller.
    /// </summary>
    public class PreInstallationActionPartController : InstallationActionPartControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreInstallationActionPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public PreInstallationActionPartController(CategoryControllerBase parent)
            : base(
            parent,
            "PreInstallationActionPart",
            AdminStrings.UnitConfig_Conclusion_PreInstallationAction,
            AdminStrings.UnitConfig_Conclusion_PreInstallationAction_Description)
        {
        }
    }
}