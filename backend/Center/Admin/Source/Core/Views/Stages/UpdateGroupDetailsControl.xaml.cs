// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateGroupDetailsControl.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for UpdateGroupDetailsControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Views.Stages
{
    using Gorba.Center.Admin.Core.DataViewModels.Update;

    using Telerik.Windows.Controls;

    /// <summary>
    /// Interaction logic for UpdateGroupDetailsControl.xaml
    /// </summary>
    public partial class UpdateGroupDetailsControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateGroupDetailsControl"/> class.
        /// </summary>
        public UpdateGroupDetailsControl()
        {
            this.InitializeComponent();
        }

        private void TimelineOnSelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            var part = this.Timeline.SelectedItem as UpdatePartsTimelineViewModel.Part;
            if (part == null)
            {
                return;
            }

            var viewModel = this.DataContext as UpdateGroupReadOnlyDataViewModel;
            if (viewModel == null)
            {
                return;
            }

            viewModel.UpdateParts.NavigateToEntityCommand.Execute(part.UpdatePart);
            this.Timeline.SelectedItem = null;
        }
    }
}
