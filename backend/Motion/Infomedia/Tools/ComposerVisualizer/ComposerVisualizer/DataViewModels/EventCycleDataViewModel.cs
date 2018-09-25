// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventCycleDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Evaluator;

    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    /// <summary>
    /// Event cycle properties data view model
    /// </summary>
    [DisplayName(@"Event Cycle Properties")]
    public class EventCycleDataViewModel : CycleDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventCycleDataViewModel"/> class.
        /// </summary>
        public EventCycleDataViewModel()
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
