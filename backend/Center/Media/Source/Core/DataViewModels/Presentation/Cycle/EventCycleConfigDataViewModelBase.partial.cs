// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventCycleConfigDataViewModelBase.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EventCycleConfigDataViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle
{
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// Defines the configuration for an event cycle.
    /// </summary>
    public partial class EventCycleConfigDataViewModelBase
    {
        partial void Initialize(Models.Presentation.Cycle.EventCycleConfigDataModelBase dataModel)
        {
            if (dataModel == null || dataModel.Trigger.Coordinates.Count == 0)
            {
                this.Trigger.Coordinates.Add(new GenericEvalDataViewModel(this.mediaShell));
            }
        }

        partial void Initialize(EventCycleConfigDataViewModelBase dataViewModel)
        {
            if (dataViewModel == null || dataViewModel.Trigger.Coordinates.Count == 0)
            {
                this.Trigger.Coordinates.Add(new GenericEvalDataViewModel(this.mediaShell));
            }
        }
    }
}