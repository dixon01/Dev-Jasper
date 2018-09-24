// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostInstallationActionPartController.cs" company="Gorba AG">
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
    public class PostInstallationActionPartController : InstallationActionPartControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostInstallationActionPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public PostInstallationActionPartController(CategoryControllerBase parent)
            : base(
            parent,
            "PostInstallationActionPart",
            AdminStrings.UnitConfig_Conclusion_PostInstallationAction,
            AdminStrings.UnitConfig_Conclusion_PostInstallationAction_Description)
        {
        }
    }
}