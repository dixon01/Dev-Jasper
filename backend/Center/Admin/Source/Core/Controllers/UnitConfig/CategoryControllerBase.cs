// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CategoryControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CategoryControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// The base class for all controllers that represent categories.
    /// </summary>
    public abstract class CategoryControllerBase
    {
        private Dictionary<string, PartControllerBase> partControllers;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryControllerBase"/> class.
        /// </summary>
        /// <param name="key">
        /// The key to uniquely identify this category.
        /// </param>
        protected CategoryControllerBase(string key)
        {
            this.Key = key;
        }

        /// <summary>
        /// Gets the key to uniquely identify this category.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Gets the parent controller.
        /// </summary>
        public UnitConfiguratorController Parent { get; private set; }

        /// <summary>
        /// Gets the view model for the category.
        /// </summary>
        public CategoryViewModel ViewModel { get; private set; }

        /// <summary>
        /// Initializes this category controller.
        /// This must be called before using the controller in any other way.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public void Initialize(UnitConfiguratorController parent)
        {
            this.Parent = parent;
            this.ViewModel = new CategoryViewModel(parent.CommandRegistry);
            this.PrepareViewModel();
            this.partControllers = this.CreatePartControllers().ToDictionary(c => c.Key);
            this.partControllers.Values.ForEach(c => this.ViewModel.Parts.Add(c.Initialize()));
        }

        /// <summary>
        /// Asynchronously prepares this category controller and all its children with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        /// The hardware descriptor.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to wait on.
        /// </returns>
        public virtual async Task PrepareAsync(HardwareDescriptor descriptor)
        {
            await Task.WhenAll(this.partControllers.Values.Select(c => c.PrepareAsync(descriptor)));
        }

        /// <summary>
        /// Loads the unit config data into this category controller and all its children.
        /// </summary>
        /// <param name="data">
        /// The configuration data for this category.
        /// </param>
        public virtual void Load(UnitConfigCategory data)
        {
            this.DoWithControllers(data, (c, p) => c.Load(p));
        }

        /// <summary>
        /// Saves the unit config data from this category controller and all its children to the given object.
        /// </summary>
        /// <param name="data">
        /// The configuration data for this category.
        /// </param>
        public virtual void Save(UnitConfigCategory data)
        {
            this.DoWithControllers(data, (c, p) => c.Save(p));
        }

        /// <summary>
        /// Tries to get a part with the given <paramref name="key"/>.
        /// </summary>
        /// <param name="key">
        /// The unique key to identify the part.
        /// If this is null, the first part of the given type is returned.
        /// </param>
        /// <param name="part">
        /// The part or null if not found.
        /// </param>
        /// <typeparam name="T">
        /// The type of part expected.
        /// </typeparam>
        /// <returns>
        /// True if the part was found.
        /// </returns>
        public bool TryGetPart<T>(string key, out T part) where T : PartControllerBase
        {
            if (key == null)
            {
                part = this.partControllers.Values.OfType<T>().FirstOrDefault();
                return part != null;
            }

            PartControllerBase controller;
            if (!this.partControllers.TryGetValue(key, out controller))
            {
                part = null;
                return false;
            }

            part = controller as T;
            return part != null;
        }

        /// <summary>
        /// Creates all part controllers.
        /// </summary>
        /// <returns>
        /// An enumeration of the part controllers of this category.
        /// </returns>
        protected abstract IEnumerable<PartControllerBase> CreatePartControllers();

        /// <summary>
        /// Prepares the category view model (e.g. setting its name, ...).
        /// </summary>
        protected abstract void PrepareViewModel();

        private void DoWithControllers(UnitConfigCategory data, Action<PartControllerBase, UnitConfigPart> action)
        {
            foreach (var controller in this.partControllers.Values)
            {
                var partData = data.Parts.FirstOrDefault(p => p.Key == controller.Key);
                if (partData == null)
                {
                    partData = new UnitConfigPart { Key = controller.Key };
                    data.Parts.Add(partData);
                }

                action(controller, partData);
            }
        }
    }
}