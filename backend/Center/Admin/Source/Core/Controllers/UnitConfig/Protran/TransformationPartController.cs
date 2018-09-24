// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransformationPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TransformationPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran
{
    using System.ComponentModel;
    using System.Linq;

    using Gorba.Center.Admin.Core.DataViewModels.UnitConfig.Transformations;
    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// The part controller to edit a single transformation.
    /// </summary>
    public class TransformationPartController : PartControllerBase<TransformationPartViewModel>
    {
        private readonly int index;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformationPartController"/> class.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public TransformationPartController(int index, CategoryControllerBase parent)
            : base(string.Format(UnitConfigKeys.Transformations.TransformationFormat, index), parent)
        {
            this.index = index;
        }

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
            var chain = partData.GetXmlValue(new Chain { Id = "Transformation_" + this.index });
            this.LoadChain(chain);
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetXmlValue(this.CreateChain());
        }

        /// <summary>
        /// Creates the transformation chain from this editor.
        /// </summary>
        /// <returns>
        /// The <see cref="Chain"/> with the proper id and all transformations.
        /// </returns>
        public Chain CreateChain()
        {
            var chain = new Chain { Id = this.ViewModel.Editor.Id };
            foreach (var transformation in this.ViewModel.Editor.Transformations)
            {
                chain.Transformations.Add(transformation.CreateConfig());
            }

            return chain;
        }

        /// <summary>
        /// Loads the given chain into this editor.
        /// </summary>
        /// <param name="chain">
        /// The chain.
        /// </param>
        public void LoadChain(Chain chain)
        {
            this.ViewModel.Editor.Id = chain.Id;

            this.ViewModel.Editor.Transformations.Clear();
            foreach (var transformation in chain.Transformations)
            {
                this.ViewModel.Editor.Transformations.Add(TransformationDataViewModelBase.Create(transformation));
            }

            this.CheckErrors();
        }

        /// <summary>
        /// Creates and initializes the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="TransformationPartViewModel"/>.
        /// </returns>
        protected override TransformationPartViewModel CreateViewModel()
        {
            var viewModel = new TransformationPartViewModel(this.index);
            viewModel.Description = AdminStrings.UnitConfig_Transformations_Transformation_Description;
            viewModel.Editor.PropertyChanged += this.EditorOnPropertyChanged;

            return viewModel;
        }

        private void EditorOnPropertyChanged(object s, PropertyChangedEventArgs e)
        {
            this.RaiseViewModelUpdated(e);
            this.CheckErrors();
        }

        private void CheckErrors()
        {
            var parent = this.Parent as TransformationsCategoryController;
            if (parent == null)
            {
                return;
            }

            var errorState =
                parent.TransformationPartControllers.Any(
                    c =>
                        c != this && c.ViewModel != null && c.ViewModel.IsVisible
                        && c.ViewModel.Editor.Id == this.ViewModel.Editor.Id)
                    ? ErrorState.Error
                    : ErrorState.Ok;
            this.ViewModel.Editor.SetError("Id", errorState, AdminStrings.Errors_DuplicateName);
        }
    }
}