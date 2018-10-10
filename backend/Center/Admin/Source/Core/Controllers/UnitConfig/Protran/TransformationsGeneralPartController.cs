// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransformationsGeneralPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TransformationsGeneralPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Core.Collections;
    using Gorba.Common.Configuration.HardwareDescription;
    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// The transformations general part controller.
    /// </summary>
    public class TransformationsGeneralPartController : PartControllerBase<NamedListPartViewModel>
    {
        private bool updatingList;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformationsGeneralPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public TransformationsGeneralPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.Transformations.General, parent)
        {
        }

        /// <summary>
        /// Gets the number of currently visible transformations.
        /// </summary>
        public int TransformationsCount
        {
            get
            {
                return this.ViewModel.Editor.Elements.Count;
            }
        }

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
            this.ViewModel.Editor.Elements.Clear();

            var count = partData.GetValue(0);
            for (var i = 0; i < count; i++)
            {
                this.ViewModel.Editor.Elements.Add(new NamedElement());
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
            partData.SetValue(this.TransformationsCount);
        }

        /// <summary>
        /// Creates and initializes the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="MultiEditorPartViewModel"/>.
        /// </returns>
        protected override NamedListPartViewModel CreateViewModel()
        {
            var viewModel = new NamedListPartViewModel();
            viewModel.DisplayName = AdminStrings.UnitConfig_Transformations_General;
            viewModel.Description = AdminStrings.UnitConfig_Transformations_General_Description;

            viewModel.Editor.MaxElementCount = TransformationsCategoryController.MaxTransformationsCount;
            viewModel.Editor.Elements.CollectionChanged += this.ElementsOnCollectionChanged;
            viewModel.Editor.Elements.ItemPropertyChanged += this.ElementsOnItemPropertyChanged;

            return viewModel;
        }

        /// <summary>
        /// Prepares this part controller with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        /// The hardware descriptor.
        /// </param>
        protected override void Prepare(HardwareDescriptor descriptor)
        {
            for (int index = 1; index <= TransformationsCategoryController.MaxTransformationsCount; index++)
            {
                var key = string.Format(UnitConfigKeys.Transformations.TransformationFormat, index);
                var part = this.GetPart<TransformationPartController>(key);
                part.ViewModel.PropertyChanged += this.PartViewModelOnPropertyChanged;
            }
        }

        private void AddChain(int index, string name)
        {
            var chains = Enumerable.Range(index, this.TransformationsCount - index - 1)
                .Select(this.GetTransformationPart)
                .Select(p => new Tuple<Chain, bool>(p.CreateChain(), p.ViewModel.IsDirty)).ToList();

            this.GetTransformationPart(index).LoadChain(new Chain { Id = name });
            for (var i = 0; i < chains.Count; i++)
            {
                var part = this.GetTransformationPart(index + i + 1);
                part.LoadChain(chains[i].Item1);
                if (!chains[i].Item2)
                {
                    part.ViewModel.ClearDirty();
                }
            }

            // workaround: load the chain again to clear any possible temporary errors
            this.GetTransformationPart(index).LoadChain(new Chain { Id = name });
        }

        private void RemoveChain(int index)
        {
            var chains = Enumerable.Range(index + 1, this.TransformationsCount - index + 1)
                .Select(this.GetTransformationPart)
                .Select(p => new Tuple<Chain, bool>(p.CreateChain(), p.ViewModel.IsDirty)).ToList();
            for (var i = chains.Count - 1; i >= 0; i--)
            {
                var part = this.GetTransformationPart(index + i);
                part.LoadChain(chains[i].Item1);
                if (!chains[i].Item2)
                {
                    part.ViewModel.ClearDirty();
                }
            }
        }

        private TransformationPartController GetTransformationPart(int index)
        {
            // indexed from 1
            var key = string.Format(UnitConfigKeys.Transformations.TransformationFormat, index + 1);
            var part = this.GetPart<TransformationPartController>(key);
            return part;
        }

        private void ElementsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.updatingList = true;
            try
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        var element = (NamedElement)e.NewItems[0];
                        element.NavigateToCommand = new RelayCommand<NamedElement>(this.NavigateTo);
                        var name = "Transformation_" + this.TransformationsCount;
                        this.AddChain(e.NewStartingIndex, name);
                        element.Name = name;
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        this.RemoveChain(e.OldStartingIndex);
                        break;
                }

                this.RaiseViewModelUpdated(e);
            }
            finally
            {
                this.updatingList = false;
            }
        }

        private void NavigateTo(NamedElement element)
        {
            var index = this.ViewModel.Editor.Elements.IndexOf(element);
            if (index < 0)
            {
                return;
            }

            this.Parent.Parent.NavigateToPart(this.GetTransformationPart(index).ViewModel);
        }

        private void ElementsOnItemPropertyChanged(object sender, ItemPropertyChangedEventArgs<NamedElement> e)
        {
            if (e.PropertyName != "Name" || this.updatingList)
            {
                return;
            }

            var index = this.ViewModel.Editor.Elements.IndexOf(e.Item);
            if (index < 0)
            {
                return;
            }

            var part = this.GetTransformationPart(index);
            part.ViewModel.ChainId = e.Item.Name;
        }

        private void PartViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "ChainId" || this.updatingList)
            {
                return;
            }

            var viewModel = sender as TransformationPartViewModel;
            if (viewModel == null)
            {
                return;
            }

            var index = viewModel.Index - 1;
            if (this.TransformationsCount <= index)
            {
                return;
            }

            this.ViewModel.Editor.Elements[index].Name = viewModel.ChainId;
        }
    }
}