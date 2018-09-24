// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPhysicalScreenController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The PhysicalScreenController interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers
{
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    /// <summary>
    /// The PhysicalScreenController interface.
    /// </summary>
    public interface IPhysicalScreenController
    {
        /// <summary>
        /// Gets or sets the media shell.
        /// </summary>
        IMediaShell MediaShell { get; set; }

        /// <summary>
        /// Gets or sets the parent controller.
        /// </summary>
        IMediaShellController Parent { get; set; }

        /// <summary>
        /// Generates a unique physical screen name in the format:
        /// {resolution} ({DuplicateIndex})
        /// </summary>
        /// <param name="resolution">
        /// The resolution of the screen.
        /// </param>
        /// <returns>
        /// The unique name.
        /// </returns>
        string GeneratePhysicalScreenName(string resolution);

        /// <summary>
        /// Generates a unique layout name in the format:
        /// "Layout {index} ({width}x{height}".
        /// </summary>
        /// <param name="resolutionWidth">
        /// The resolution width.
        /// </param>
        /// <param name="resolutionHeight">
        /// The resolution height.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="screenType">
        /// The physical screen type.
        /// </param>
        /// <param name="isEventLayout">
        /// Indicates a value whether the layout is linked to an event cycle (only to be used with screen type Audio).
        /// </param>
        /// <returns>
        /// The unique name.
        /// </returns>
        string GenerateLayoutName(
            int resolutionWidth,
            int resolutionHeight,
            int index,
            PhysicalScreenType screenType,
            bool isEventLayout = false);

        /// <summary>
        /// Generates a unique cycle package name in the format:
        /// "CyclePackage {identifier} ({DuplicateIndex})".
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        /// <returns>
        /// The unique name.
        /// </returns>
        string GenerateCyclePackageName(string identifier);

        /// <summary>
        /// Generates a unique virtual display name in the format:
        /// "VirtualDisplay {identifier} ({DuplicateIndex})".
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        /// <returns>
        /// The unique name.
        /// </returns>
        string GenerateVirtualDisplayName(string identifier);

        /// <summary>
        /// Generates a unique identifier for physical screens.
        /// </summary>
        /// <returns>
        /// The unique identifier.
        /// </returns>
        string GenerateIdentifier();
    }
}
