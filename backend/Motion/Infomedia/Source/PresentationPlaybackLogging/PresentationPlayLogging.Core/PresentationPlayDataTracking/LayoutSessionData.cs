// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutElementsData.cs" company="Luminator LTG">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LayoutSessionData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PresentationPlayLogging.Core.PresentationPlayDataTracking
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.Infomedia.Entities.Screen;

    using Luminator.PresentationPlayLogging.Core.Interfaces;
    using Luminator.PresentationPlayLogging.Core.Models;

    /// <summary>
    /// Tracks and updates resource data between layout session updates, when a presentation is playing.
    /// </summary>
    /// <typeparam name="T">The type of presentation info
    /// </typeparam>
    public class LayoutSessionData<T>
        where T : class, IInfotransitPresentationInfo, new()
    {
        private readonly CurrentEnvironmentInfo currentEnvironment;

        private readonly Dictionary<string, T> infoItemsByUnit = new Dictionary<string, T>();

        /// <summary>
        /// Tracks the screen item by its filename
        /// </summary>
        private readonly Dictionary<string, ScreenItemBase> screenItemByFileName = new Dictionary<string, ScreenItemBase>();

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutSessionData{T}"/> class.
        /// </summary>
        /// <param name="currentEnvironment">
        /// The current environment.
        /// </param>
        public LayoutSessionData(CurrentEnvironmentInfo currentEnvironment)
        {
            this.currentEnvironment = currentEnvironment;
        }

        /// <summary>
        /// Gets the current environment.
        /// </summary>
        public CurrentEnvironmentInfo CurrentEnvironment
        {
            get
            {
                return this.currentEnvironment;
            }
        }

        /// <summary>
        /// Gets or sets the element id we are tracking this data for.
        /// </summary>
        public int ElementId { get; set; }

        /// <summary>
        /// Gets or sets the info items.
        /// </summary>
        public List<T> InfoItems { get; set; } = new List<T>();

        /// <summary>
        /// Gets or sets the unit name.
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// Get the entries for this element, for each unit.
        /// </summary>
        /// <param name="elementId"></param>
        /// <returns></returns>
        public List<T> GetLogItems()
        {
            return this.infoItemsByUnit.Values.ToList();
        }

        /// <summary>
        /// Gets a screen item we are tracking, by its file name.
        /// </summary>
        /// <param name="itemFileName">
        /// The info item file name.
        /// </param>
        /// <returns>
        /// The <see cref="ScreenItemBase"/>.
        /// </returns>
        public ScreenItemBase GetScreenItem(string itemFileName)
        {
            if (this.screenItemByFileName.TryGetValue(itemFileName, out var screenItem))
            {
                return screenItem;
            }

            return null;
        }

        public void InitializeDrawableItemData(DrawableComposerInitMessage message)
        {
            this.infoItemsByUnit.Clear();
            return;
        }

        /// <summary>
        /// Update all items matching the given resource name, with the given properties.
        /// </summary>
        /// <param name="unitName">
        /// The unit Name.
        /// </param>
        /// <param name="resourceName">The resource or file name.
        /// </param>
        /// <param name="resourceId">The resource id
        /// </param>
        /// <param name="messageDuration">The message duration set from iAdmin
        /// </param>
        /// <param name="screenItem">The screen item (image or video)
        /// The screen Item.
        /// </param>
        public void UpdateItems(string unitName, string resourceName, string resourceId, TimeSpan messageDuration, ScreenItemBase screenItem)
        {
            var matchingItems = this.GetLayoutElements(resourceName);

            if (matchingItems.Count == 0)
            {
                // Add a new item.
                T newItem = new T { FileName = resourceName, UnitName = unitName };
                this.InfoItems.Add(newItem);
                this.screenItemByFileName[resourceName] = screenItem;
                matchingItems = this.InfoItems;
            }

            foreach (var infoItem in matchingItems)
            {
                infoItem.ResourceId = resourceId;
                infoItem.Duration = (long)messageDuration.TotalSeconds;
                infoItem.PlayStarted = DateTime.Now;
                infoItem.Route = this.currentEnvironment.Route;
                infoItem.VehicleId = this.currentEnvironment.VehicleId;
                infoItem.StartedLatitude = this.FirstValidString(infoItem.StartedLatitude, this.currentEnvironment.Latitude);
                infoItem.StartedLongitude = this.FirstValidString(infoItem.StartedLongitude, this.currentEnvironment.Longitude);
            }
        }

        /// <summary>
        /// Create a new item to track, or update existing items with the same filename.
        /// </summary>
        /// <param name="presentationInfo">Values to update</param>
        public void UpdateItems(InfotransitPresentationInfo presentationInfo)
        {
            var matchingItems = this.GetLayoutElements(presentationInfo.FileName);
            if (matchingItems.Count == 0)
            {
                T newItem = new T
                                {
                                    FileName = presentationInfo.FileName,
                                    UnitName = presentationInfo.UnitName,
                                    StartedLongitude = this.FirstValidString(presentationInfo.StartedLongitude, this.currentEnvironment.Longitude),
                                    StartedLatitude = this.FirstValidString(presentationInfo.StartedLatitude, this.currentEnvironment.Latitude),
                                    Route = this.FirstValidString(this.currentEnvironment.Route, presentationInfo.Route),
                                    VehicleId = this.FirstValidString(this.currentEnvironment.VehicleId, presentationInfo.VehicleId)
                                };
                this.InfoItems.Add(newItem);
                matchingItems = this.InfoItems;
            }

            foreach (var infoItem in matchingItems)
            {
                infoItem.ResourceId = presentationInfo.ResourceId;
                infoItem.Duration = presentationInfo.Duration;
                infoItem.PlayStarted = presentationInfo.PlayStarted;
            }
        }

        public void UpdateItemUnits(DrawableComposerInitMessage message)
        {
            this.infoItemsByUnit[message.UnitName] = this.CreateNewInfoItem(message);
        }

        /// <summary>
        /// When we get a change in video playback, update the tracked information on the video items we track.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="messagePlaying">
        /// The message playing.
        /// </param>
        /// <param name="currentTime">
        /// The current time.
        /// </param>
        public void UpdateVideoPlayback(string fileName, bool messagePlaying, DateTime currentTime)
        {
            var matchingEntries = this.GetLayoutElements(fileName);

            foreach (var infoEntry in matchingEntries)
            {
                if (messagePlaying)
                {
                    infoEntry.StartedLatitude = this.currentEnvironment.Latitude;
                    infoEntry.StartedLongitude = this.currentEnvironment.Longitude;
                    infoEntry.PlayStarted = currentTime;
                }
                else
                {
                    infoEntry.StoppedLatitude = this.currentEnvironment.Latitude;
                    infoEntry.StoppedLongitude = this.currentEnvironment.Longitude;
                    infoEntry.PlayStopped = currentTime;
                }
            }
        }

        private T CreateNewInfoItem(DrawableComposerInitMessage message)
        {
            T newItem = new T
                            {
                                ResourceId = message.ElementID.ToString(),
                                FileName = Path.GetFileName(message.ElementFileName),
                                UnitName = message.UnitName,
                                StartedLongitude = this.currentEnvironment.Longitude,
                                StartedLatitude = this.currentEnvironment.Latitude,
                                Route = this.currentEnvironment.Route,
                                VehicleId = this.currentEnvironment.VehicleId,
                                PlayStarted = DateTime.Now
                            };

            return newItem;
        }

        /// <summary>
        /// Give multiple string values, return the first that is not null or empty, or return an empty string.
        /// </summary>
        /// <param name="values"></param>
        /// <returns>The result</returns>
        private string FirstValidString(params string[] values)
        {
            foreach (var value in values)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets a list of graphical elements we are tracking for this layout, that have the given file name.
        /// </summary>
        /// <param name="resourceName">The file or resource name.</param>
        /// <returns>The list of matching layout elements</returns>
        private List<T> GetLayoutElements(string resourceName)
        {
            List<T> results = new List<T>();
            foreach (var infoItem in this.InfoItems)
            {
                if (infoItem.FileName == resourceName)
                {
                    results.Add(infoItem);
                }
            }

            return results;
        }
    }
}