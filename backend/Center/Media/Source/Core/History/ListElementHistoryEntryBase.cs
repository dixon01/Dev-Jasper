// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListElementHistoryEntryBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The list element history entry base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.History
{
    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// The list element history entry base.
    /// </summary>
    public abstract class ListElementHistoryEntryBase : HistoryEntryBase
    {
        /// <summary>
        /// The media shell.
        /// </summary>
        protected readonly IMediaShell MediaShell;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListElementHistoryEntryBase"/> class.
        /// </summary>
        /// <param name="mediaShell">
        /// The media shell.
        /// </param>
        /// <param name="displayText">
        /// The display text.
        /// </param>
        protected ListElementHistoryEntryBase(IMediaShell mediaShell, string displayText)
            : base(displayText)
        {
            this.MediaShell = mediaShell;
        }

        /// <summary>
        /// The add to font list.
        /// </summary>
        /// <param name="fontResource">
        /// The font resource.
        /// </param>
        protected void AddToAvailableFonts(ResourceInfoDataViewModel fontResource)
        {
            if (fontResource.IsLedFont)
            {
                if (!this.MediaShell.MediaApplicationState.CurrentProject.AvailableLedFonts.Contains(fontResource.Facename))
                {
                    this.MediaShell.MediaApplicationState.CurrentProject.AvailableLedFonts.Add(fontResource.Facename);
                }
            }
            else
            {
                if (!this.MediaShell.MediaApplicationState.CurrentProject.AvailableFonts.Contains(fontResource.Facename))
                {
                    this.MediaShell.MediaApplicationState.CurrentProject.AvailableFonts.Add(fontResource.Facename);
                }
            }
        }

        /// <summary>
        /// The remove from font list.
        /// </summary>
        /// <param name="fontResource">
        /// The font resource.
        /// </param>
        protected void RemoveFromAvailableFonts(ResourceInfoDataViewModel fontResource)
        {
            if (fontResource.IsLedFont)
            {
                if (this.MediaShell.MediaApplicationState.CurrentProject.AvailableLedFonts.Contains(fontResource.Facename))
                {
                    this.MediaShell.MediaApplicationState.CurrentProject.AvailableLedFonts.Remove(fontResource.Facename);
                }
            }
            else
            {
                if (this.MediaShell.MediaApplicationState.CurrentProject.AvailableFonts.Contains(fontResource.Facename))
                {
                    this.MediaShell.MediaApplicationState.CurrentProject.AvailableFonts.Remove(fontResource.Facename);
                }
            }
        }
    }
}