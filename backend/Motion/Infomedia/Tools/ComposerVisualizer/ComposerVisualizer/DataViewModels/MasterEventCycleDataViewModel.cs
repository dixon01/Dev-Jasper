// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MasterEventCycleDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MasterEventCycleDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Evaluator;

    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    /// <summary>
    /// The master event cycle data view model.
    /// </summary>
    [DisplayName(@"Master Event Cycle Properties")]
    public class MasterEventCycleDataViewModel : MasterCycleDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterEventCycleDataViewModel"/> class.
        /// </summary>
        public MasterEventCycleDataViewModel()
        {
            this.Trigger = new ObservableCollection<EvaluatorBaseDataViewModel>();
        }

        /// <summary>
        /// Gets or sets the trigger.
        /// </summary>
        [Category("Configuration")]
        [ReadOnly(true)]
        [ExpandableObject]
        [DisplayName(@"Trigger*")]
        public ObservableCollection<EvaluatorBaseDataViewModel> Trigger { get; set; }
    }
}