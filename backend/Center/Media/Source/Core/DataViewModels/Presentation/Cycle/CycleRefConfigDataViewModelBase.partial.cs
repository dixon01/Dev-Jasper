// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CycleRefConfigDataViewModelBase.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle
{
    using System.Collections.Generic;

    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ViewModels;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// The cycle config data view model base.
    /// </summary>
    public partial class CycleRefConfigDataViewModelBase : IReusableEntity
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private bool isInEditMode;

        private int referencesCount;

        private bool isExpanded;

        private bool isChildItemSelected;

        /// <summary>
        /// Gets or sets a value indicating whether is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return this.isExpanded;
            }

            set
            {
                this.SetProperty(ref this.isExpanded, value, () => this.IsExpanded);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether one of the child items is selected.
        /// </summary>
        public bool IsChildItemSelected
        {
            get
            {
                return this.isChildItemSelected;
            }

            set
            {
                this.SetProperty(ref this.isChildItemSelected, value, () => this.IsChildItemSelected);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the is in edit mode
        /// </summary>
        public bool IsInEditMode
        {
            get
            {
                return this.isInEditMode;
            }

            set
            {
                this.SetProperty(ref this.isInEditMode, value, () => this.IsInEditMode);
            }
        }

        /// <summary>
        /// Gets or sets the name of the referenced object.
        /// </summary>
        public DataValue<string> Name
        {
            get
            {
                return this.Reference.Name;
            }

            set
            {
                this.Reference.Name = value;
            }
        }

        /// <summary>
        /// Gets the number of sections where this layout is used.
        /// </summary>
        public int ReferencesCount
        {
            get
            {
                return this.referencesCount;
            }

            private set
            {
                this.SetProperty(ref this.referencesCount, value, () => this.ReferencesCount);
            }
        }

        /// <summary>
        /// Gets the is used tooltip
        /// </summary>
        public string IsUsedToolTip { get; private set; }

        /// <summary>
        /// Gets the name
        /// </summary>
        /// <returns>the name</returns>
        public string GetName()
        {
            return this.Reference.Name.Value;
        }

        /// <summary>
        /// Sets the name of the referenced object.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public void SetName(string name)
        {
            this.Reference.Name.Value = name;
        }

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        protected override System.Collections.Generic.IEnumerable<string> Validate(string propertyName)
        {
            return Reference.ValidateAsReference(propertyName);
        }

        private CycleConfigDataViewModelBase FindReference()
        {
            var applicationState = ServiceLocator.Current.GetInstance<IMediaApplicationState>();
            if (applicationState.CurrentProject == null || applicationState.CurrentProject.InfomediaConfig == null)
            {
                return null;
            }

            foreach (var cycle in applicationState.CurrentProject.InfomediaConfig.Cycles.StandardCycles)
            {
                if (cycle.Name.Value == this.ReferenceName)
                {
                    return cycle;
                }
            }

            foreach (var eventCycle in applicationState.CurrentProject.InfomediaConfig.Cycles.EventCycles)
            {
                if (eventCycle.Name.Value == this.ReferenceName)
                {
                    return eventCycle;
                }
            }

            Logger.Trace("Cycle reference with name {0} not found in Cycles.", this.ReferenceName);
            return null;
        }
    }
}
