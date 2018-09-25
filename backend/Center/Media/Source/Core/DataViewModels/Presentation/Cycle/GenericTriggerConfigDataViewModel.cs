// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericTriggerConfigDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The GenericTriggerConfigDataViewModel.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle
{
    using System.Collections.Specialized;

    using Gorba.Center.Common.Wpf.Views.Components.PropertyGrid;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Models.Presentation.Cycle;

    /// <summary>
    /// The GenericTriggerConfigDataViewModel.
    /// </summary>
    public partial class GenericTriggerConfigDataViewModel : ITriggerProperty
    {
        private const char CoordinateSeperator = ';';

        /// <summary>
        /// produces a textual representation of the generic trigger
        /// </summary>
        /// <returns>a textual representation of the generic trigger</returns>
        public override string ToString()
        {
            var result = string.Empty;

            foreach (var coordinate in this.Coordinates)
            {
                var text = GenericExtensions.ConvertToHumanReadable(
                    this.mediaShell,
                    coordinate.Table,
                    coordinate.Column,
                    coordinate.Row,
                    coordinate.Language);

                result += text + CoordinateSeperator;
            }

            result = "[ " + result.TrimEnd(new[] { CoordinateSeperator, ' ' }) + " ]";

            return result;
        }

        partial void Initialize(GenericTriggerConfigDataModel dataModel = null)
        {
            this.Coordinates.ListenOnChildChanged = true;
            this.Coordinates.ChildChanged += this.MakeAppDirty;
        }

        partial void Initialize(GenericTriggerConfigDataViewModel dataViewModel)
        {
            this.Coordinates.ListenOnChildChanged = true;
            this.Coordinates.ChildChanged += this.MakeAppDirty;
        }

        private void MakeAppDirty()
        {
            this.mediaShell.MediaApplicationState.MakeDirty();
            this.mediaShell.MediaApplicationState.CurrentProject.IsCheckedIn = false;
        }
    }
}