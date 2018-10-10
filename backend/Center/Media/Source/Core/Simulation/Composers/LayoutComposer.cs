// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutComposer.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The composer that handles a single resolution of a single layout and sends out updates for the screen.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Simulation.Composers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Medi.Core;
    using Gorba.Motion.Infomedia.Entities;
    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.Infomedia.Entities.Screen;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The composer that handles a single resolution of a single layout and sends out updates for the screen.
    /// </summary>
    public class LayoutComposer : IDisposable, IComposerContext
    {
        private readonly List<IComposer> composers = new List<IComposer>();
        private readonly List<ScreenChange> currentScreenChanges = new List<ScreenChange>();
        private readonly List<ScreenUpdate> currentScreenUpdates = new List<ScreenUpdate>();

        private readonly SynchronizationContext synchronizationContext;

        private ResolutionConfigDataViewModel resolution;

        private PhysicalScreenType screenType;

        private RootItem rootItem;

        private bool shouldUpdate;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutComposer"/> class.
        /// </summary>
        public LayoutComposer()
        {
            this.synchronizationContext = SynchronizationContext.Current ?? new SynchronizationContext();
        }

        /// <summary>
        /// Gets the application state.
        /// </summary>
        public IMediaApplicationState ApplicationState
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IMediaApplicationState>();
            }
        }

        /// <summary>
        /// Loads the given <paramref name="resolutionConfig"/>.
        /// </summary>
        /// <param name="resolutionConfig">
        /// The resolution configuration.
        /// </param>
        /// <param name="physicalScreenType">
        /// The physical screen type.
        /// </param>
        public void Load(ResolutionConfigDataViewModel resolutionConfig, PhysicalScreenType physicalScreenType)
        {
            this.ClearResolution();

            this.resolution = resolutionConfig;
            this.resolution.Elements.CollectionChanged += this.ElementsOnCollectionChanged;

            this.screenType = physicalScreenType;
            this.ReloadElements();

            MessageDispatcher.Instance.Subscribe<ScreenRequests>(this.HandleScreenRequests);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            MessageDispatcher.Instance.Unsubscribe<ScreenRequests>(this.HandleScreenRequests);
            this.ClearResolution();
        }

        string IComposerContext.GetImageFileName(string hash)
        {
            return "#" + hash;
        }

        /// <summary>
        /// The get resource uri by filename.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetResourceUriByFilename(string filename)
        {
            var resource =
               this.ApplicationState.CurrentProject.Resources.FirstOrDefault(
                   r => r.Filename.EndsWith(filename));
            if (resource == null)
            {
                return string.Empty;
            }

            var resourceManager = ServiceLocator.Current.GetInstance<IResourceManager>();
            return resourceManager.GetResourcePath(resource.Hash);
        }

        string IComposerContext.GetVideoUri(string hash)
        {
            var resource =
                this.ApplicationState.CurrentProject.Resources.FirstOrDefault(
                    r => r.Hash == hash);
            if (resource == null)
            {
                return string.Empty;
            }

            var resourceManager = ServiceLocator.Current.GetInstance<IResourceManager>();
            return resourceManager.GetResourcePath(resource.Hash);
        }

        private void ClearResolution()
        {
            if (this.resolution == null)
            {
                return;
            }

            this.resolution.Elements.CollectionChanged -= this.ElementsOnCollectionChanged;
            this.resolution = null;

            this.ClearComposers();
        }

        private void ClearComposers()
        {
            foreach (var composer in this.composers)
            {
                composer.Dispose();
            }

            this.composers.Clear();
        }

        private void ReloadElements()
        {
            this.currentScreenChanges.Clear();
            this.currentScreenUpdates.Clear();
            this.ClearComposers();

            foreach (var element in this.resolution.Elements)
            {
                this.composers.Add(
                    ComposerFactory.CreateComposer(this, null, (GraphicalElementDataViewModelBase)element));
            }

            // create a special view model for the full screen (if needed)
            var fullScreenFrame = new FrameElementDataViewModel(null);
            fullScreenFrame.FrameId.Value = 0;
            fullScreenFrame.X.Value = 0;
            fullScreenFrame.Y.Value = 0;
            fullScreenFrame.Width.Value = this.resolution.Width.Value;
            fullScreenFrame.Height.Value = this.resolution.Height.Value;
            fullScreenFrame.ZIndex.Value = int.MaxValue;

            this.composers.Add(new FrameComposer(this, null, fullScreenFrame));

            this.rootItem = new RootItem { Width = this.resolution.Width.Value, Height = this.resolution.Height.Value };
            foreach (var composer in this.composers.OfType<IPresentableComposer>())
            {
                composer.Item.PropertyValueChanged += this.ItemOnPropertyValueChanged;
                this.RegisterToIncludeItemChanges(composer.Item as IncludeItem);
                this.rootItem.Items.Add(composer.Item);
            }

            var screenChange = this.CreateScreenChange();
            this.currentScreenChanges.Add(screenChange);

            this.UpdateLater();
        }

        private ScreenChange CreateScreenChange()
        {
            var resourceBasePath = Path.GetTempPath();

            var fontFiles =
                (from resource in this.ApplicationState.CurrentProject.Resources.Where(r => r.Type == ResourceType.Font)
                    where this.ApplicationState.CurrentProject.AvailableFonts.Contains(resource.Facename)
                    select Path.GetFileName(resource.Filename)
                    into resourceFileName
                    where resourceFileName != null
                    select Path.Combine(resourceBasePath, resourceFileName)).ToList();
            var screenChange = new ScreenChange
                                   {
                                       Screen = new ScreenId { Type = this.screenType },
                                       ScreenRoot = new ScreenRoot { Visible = true, Root = this.rootItem },
                                       FontFiles = fontFiles
                                   };

            return screenChange;
        }

        private void UpdateLater()
        {
            this.shouldUpdate = true;
            this.synchronizationContext.Post(s => this.SendUpdates(), null);
        }

        private void SendUpdates()
        {
            if (!this.shouldUpdate)
            {
                return;
            }

            var screenChanges = new ScreenChanges();
            if (this.currentScreenChanges.Count > 0)
            {
                // don't send updates for items if we are anyways changing the screen
                screenChanges.Changes.Add(this.currentScreenChanges.Last());
            }
            else
            {
                screenChanges.Updates.AddRange(this.currentScreenUpdates);
            }

            this.shouldUpdate = false;
            this.currentScreenChanges.Clear();
            this.currentScreenUpdates.Clear();

            MessageDispatcher.Instance.Send(MessageDispatcher.Instance.LocalAddress, screenChanges);
        }

        private void RegisterToIncludeItemChanges(IncludeItem includeItem)
        {
            if (includeItem == null || includeItem.Include == null)
            {
                return;
            }

            foreach (var item in includeItem.Include.Items)
            {
                item.PropertyValueChanged += this.ItemOnPropertyValueChanged;
            }
        }

        private void HandleScreenRequests(object sender, MessageEventArgs<ScreenRequests> e)
        {
            if (!e.Message.Screens.Any(
                r => r.Width == this.resolution.Width.Value && r.Height == this.resolution.Height.Value) ||
                this.rootItem == null)
            {
                return;
            }

            var screenChange = this.CreateScreenChange();
            var screenChanges = new ScreenChanges { Changes = { screenChange } };
            MessageDispatcher.Instance.Send(MessageDispatcher.Instance.LocalAddress, screenChanges);
        }

        private void ElementsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.ReloadElements();
        }

        private void ItemOnPropertyValueChanged(object sender, AnimatedPropertyChangedEventArgs e)
        {
            var item = sender as ScreenItemBase;
            if (item == null)
            {
                return;
            }

            if (sender is IncludeItem && e.PropertyName == "Include")
            {
                this.RegisterToIncludeItemChanges(item as IncludeItem);
            }

            int rootId;

            if (this.rootItem.Items.Contains(item))
            {
                rootId = this.rootItem.Id;
            }
            else
            {
                var include =
                    this.rootItem.Items.OfType<IncludeItem>().FirstOrDefault(i => i.Include.Items.Contains(item));
                if (include == null)
                {
                    return;
                }

                rootId = include.Include.Id;
            }

            var screenUpdate = this.currentScreenUpdates.FirstOrDefault(u => u.RootId == rootId);
            if (screenUpdate == null)
            {
                screenUpdate = new ScreenUpdate { RootId = rootId };
                this.currentScreenUpdates.Add(screenUpdate);
            }

            screenUpdate.Updates.Add(
                new ItemUpdate
                    {
                        Property = e.PropertyName,
                        Animation = e.Animation,
                        ScreenItemId = item.Id,
                        Value = e.Value
                    });
            this.UpdateLater();
        }
    }
}
