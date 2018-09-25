// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddListElementHistoryEntry{T}.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The AddListElementHistoryEntry.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.History
{
    using System.IO;

    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    /// <summary>
    /// The AddListElementHistoryEntry.
    /// </summary>
    /// <typeparam name="T">the type of the element</typeparam>
    public class AddListElementHistoryEntry<T> : ListElementHistoryEntryBase
    {
        private readonly T element;

        private readonly ExtendedObservableCollection<T> targetCollection;

        private FontConfigDataViewModel fontDataViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddListElementHistoryEntry{T}"/> class.
        /// </summary>
        /// <param name="mediaShell">
        /// The media Shell.
        /// </param>
        /// <param name="element">
        /// resource info data view model
        /// </param>
        /// <param name="targetCollection">
        /// target collection
        /// </param>
        /// <param name="displayText">
        /// display text
        /// </param>
        public AddListElementHistoryEntry(
            IMediaShell mediaShell,
            T element,
            ExtendedObservableCollection<T> targetCollection,
            string displayText)
            : base(mediaShell, displayText)
        {
            this.element = element;
            this.targetCollection = targetCollection;
        }

        /// <summary>
        /// the do method
        /// </summary>
        public override void Do()
        {
            this.targetCollection.Add(this.element);
            var resource = this.element as ResourceInfoDataViewModel;
            if (resource == null || resource.Type != ResourceType.Font)
            {
                return;
            }

            this.AddToAvailableFonts(resource);

            var name = Path.GetFileName(resource.Filename);
            if (name != null)
            {
                var infomediaFontPath = Path.Combine(Settings.Default.RelativeFontsFolderPath, name);
                this.fontDataViewModel = new FontConfigDataViewModel(this.MediaShell)
                    {
                        Path = { Value = infomediaFontPath },
                        ScreenType = { Value = resource.IsLedFont ? PhysicalScreenType.LED : PhysicalScreenType.TFT }
                    };
                this.MediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.Fonts.Add(this.fontDataViewModel);
                InteractionManager<UpdateLayoutDetailsPrompt>.Current.Raise(new UpdateLayoutDetailsPrompt());
            }
        }

        /// <summary>
        /// the undo method
        /// </summary>
        public override void Undo()
        {
            this.targetCollection.Remove(this.element);
            var resource = this.element as ResourceInfoDataViewModel;
            if (resource == null || resource.Type != ResourceType.Font || this.fontDataViewModel == null)
            {
                return;
            }

            this.RemoveFromAvailableFonts(resource);

            this.MediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.Fonts.Remove(this.fontDataViewModel);
            InteractionManager<UpdateLayoutDetailsPrompt>.Current.Raise(new UpdateLayoutDetailsPrompt());
        }
    }
}