// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataItemSelectionPartController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataItemSelectionPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Vdv301
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// The data item selection part controller.
    /// </summary>
    public class DataItemSelectionPartController
        : PartControllerBase<CheckableTreePartViewModel>, IFilteredPartController
    {
        private readonly Vdv301ProtocolCategoryController parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataItemSelectionPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public DataItemSelectionPartController(Vdv301ProtocolCategoryController parent)
            : base(UnitConfigKeys.Vdv301Protocol.TelegramSelection, parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
            foreach (var node in this.ViewModel.Editor.Root.Children)
            {
                this.Load(node, new[] { ((PropertyInfo)node.Value).Name }, partData);
            }
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            foreach (var node in this.ViewModel.Editor.Root.Children)
            {
                this.Save(node, new[] { ((PropertyInfo)node.Value).Name }, partData);
            }
        }

        /// <summary>
        /// Update the visibility of this part.
        /// </summary>
        /// <param name="visible">
        /// A flag indicating if the view model of this part should be visible.
        /// </param>
        public void UpdateVisibility(bool visible)
        {
            this.ViewModel.IsVisible = visible;
        }

        /// <summary>
        /// Prepares this part controller with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        /// The hardware descriptor.
        /// </param>
        protected override void Prepare(HardwareDescriptor descriptor)
        {
            foreach (var node in this.parent.DataItemsRoot.Children)
            {
                this.ViewModel.Editor.Root.Children.Add(node);
            }
        }

        /// <summary>
        /// Creates and initializes the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="MultiSelectPartViewModel"/>.
        /// </returns>
        protected override CheckableTreePartViewModel CreateViewModel()
        {
            var viewModel = new CheckableTreePartViewModel();
            viewModel.DisplayName = AdminStrings.UnitConfig_Vdv301_DataItemSelection;
            viewModel.Description = AdminStrings.UnitConfig_Vdv301_DataItemSelection_Description;

            viewModel.Editor.Root.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == "IsChecked")
                    {
                        this.UpdateErrors();
                    }
                };

            return viewModel;
        }

        /// <summary>
        /// Raises the <see cref="PartControllerBase.ViewModelUpdated"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void RaiseViewModelUpdated(EventArgs e)
        {
            base.RaiseViewModelUpdated(e);
            this.UpdateErrors();
        }

        private void Load(CheckableTreeNodeViewModel node, string[] path, UnitConfigPart partData)
        {
            if (node.Children.Count == 0)
            {
                node.IsChecked = partData.GetValue(false, string.Join(".", path));
                return;
            }

            foreach (var child in node.Children)
            {
                this.Load(child, path.Concat(new[] { ((PropertyInfo)child.Value).Name }).ToArray(), partData);
            }
        }

        private void Save(CheckableTreeNodeViewModel node, string[] path, UnitConfigPart partData)
        {
            if (node.Children.Count == 0)
            {
                partData.SetValue(node.IsChecked, string.Join(".", path));
                return;
            }

            foreach (var child in node.Children)
            {
                this.Save(child, path.Concat(new[] { ((PropertyInfo)child.Value).Name }).ToArray(), partData);
            }
        }

        private void UpdateErrors()
        {
            var root = this.ViewModel.Editor.Root;
            var errorState = !root.IsChecked.HasValue || root.IsChecked.Value
                                 ? ErrorState.Ok
                                 : (this.ViewModel.WasVisited ? ErrorState.Error : ErrorState.Missing);
            this.ViewModel.Editor.SetError("Root", errorState, AdminStrings.Errors_SelectOneAtLeast);
        }
    }
}