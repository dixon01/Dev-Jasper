// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphicalElementDataViewModelBase.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Layout
{
    using System;
    using System.Collections.Generic;

    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Motion.Infomedia.AhdlcRenderer.Engines;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Defines the base properties of all layout element data ViewModels.
    /// </summary>
    public abstract partial class GraphicalElementDataViewModelBase
    {
        private readonly Lazy<IResourceManager> lazyResourceManager = new Lazy<IResourceManager>(GetResourceManager);

        private GraphicalElementGroupDataViewModel group;

        /// <summary>
        /// Gets the parent ViewModel.
        /// </summary>
        public IEditorViewModel Parent
        {
            get
            {
                return this.mediaShell.Editor;
            }
        }

        /// <summary>
        /// Gets the shell.
        /// </summary>
        public IMediaShell Shell
        {
            get
            {
                return this.mediaShell;
            }
        }

        /// <summary>
        /// Gets or sets the group this element belongs to.
        /// </summary>
        public GraphicalElementGroupDataViewModel Group
        {
            get
            {
                return this.group;
            }

            set
            {
                this.SetProperty(ref this.group, value, () => this.Group);
            }
        }

        /// <summary>
        /// Gets or sets the is on canvas.
        /// </summary>
        public DataValue<bool> UseMousePosition { get; set; }

        /// <summary>
        /// Gets or sets the list of edge snap data.
        /// </summary>
        public List<EdgeSnapData> EdgeSnapDataList { get; set; }

        /// <summary>
        /// Gets the resource manager.
        /// </summary>
        protected IResourceManager ResourceManager
        {
            get
            {
                return this.lazyResourceManager.Value;
            }
        }

        /// <summary>
        /// Gets the component to be rendered.
        /// </summary>
        /// <returns>
        /// The <see cref="ComponentBase"/>.
        /// </returns>
        public virtual ComponentBase GetComponent()
        {
            return null;
        }

        private static IResourceManager GetResourceManager()
        {
            return ServiceLocator.Current.GetInstance<IResourceManager>();
        }

        partial void Initialize(GraphicalElementDataModelBase dataModel)
        {
            this.UseMousePosition = new DataValue<bool>(false);
            this.EdgeSnapDataList = new List<EdgeSnapData>();
        }

        partial void Initialize(GraphicalElementDataViewModelBase dataViewModel)
        {
            this.Group = dataViewModel.Group;
            this.UseMousePosition = new DataValue<bool>(false);
            this.EdgeSnapDataList = new List<EdgeSnapData>();
        }

        partial void ConvertNotGeneratedToDataModel(ref GraphicalElementDataModelBase dataModel)
        {
            dataModel.ElementName = this.ElementName.Value;
            if (this.group != null)
            {
                dataModel.Group = new GraphicalElementGroupDataModel
                {
                    GroupName = this.group.GroupName,
                };

                foreach (var groupItem in this.group.Items)
                {
                    var convertedItem =
                        (GraphicalElementDataModelBase)
                        groupItem.GetType()
                                 .InvokeMember(
                                     "ToDataModel", System.Reflection.BindingFlags.InvokeMethod, null, groupItem, null);

                    dataModel.Group.Items.Add(convertedItem);
                }
            }
        }
    }
}
