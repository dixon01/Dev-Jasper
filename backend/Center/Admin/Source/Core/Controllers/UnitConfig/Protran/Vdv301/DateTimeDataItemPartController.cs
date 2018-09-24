// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeDataItemPartController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DateTimeDataItemPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Vdv301
{
    using System;

    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.Protran.VDV301;

    /// <summary>
    /// The part controller for a <see cref="DateTimeDataItemConfig"/>.
    /// </summary>
    public class DateTimeDataItemPartController : DataItemPartControllerBase<DateTimeDataItemConfig>
    {
        private TextEditorViewModel dateTimeFormat;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeDataItemPartController"/> class.
        /// </summary>
        /// <param name="path">
        /// The path to the data item.
        /// </param>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public DateTimeDataItemPartController(string[] path, CategoryControllerBase parent)
            : base(path, parent)
        {
        }

        /// <summary>
        /// Creates the data item config for this part.
        /// </summary>
        /// <returns>
        /// The <see cref="DateTimeDataItemConfig"/>.
        /// </returns>
        protected override DateTimeDataItemConfig CreateDataItemConfig()
        {
            var config = base.CreateDataItemConfig();
            config.DateTimeFormat = this.dateTimeFormat.Text;
            return config;
        }

        /// <summary>
        /// Loads the data from the given config into the editors.
        /// </summary>
        /// <param name="dataItemConfig">
        /// The data item config.
        /// </param>
        protected override void LoadFrom(DateTimeDataItemConfig dataItemConfig)
        {
            base.LoadFrom(dataItemConfig);
            this.dateTimeFormat.Text = dataItemConfig.DateTimeFormat;
            this.UpdateErrors();
        }

        /// <summary>
        /// Creates and initializes the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="MultiEditorPartViewModel"/>.
        /// </returns>
        protected override MultiEditorPartViewModel CreateViewModel()
        {
            var viewModel = base.CreateViewModel();

            this.dateTimeFormat = new TextEditorViewModel();
            this.dateTimeFormat.Label = AdminStrings.UnitConfig_Vdv301_DataItem_DateTimeFormat;
            viewModel.Editors.Add(this.dateTimeFormat);

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

        private void UpdateErrors()
        {
            var errorState = string.IsNullOrWhiteSpace(this.dateTimeFormat.Text)
                                 ? (this.ViewModel.WasVisited ? ErrorState.Error : ErrorState.Missing)
                                 : ErrorState.Ok;
            this.dateTimeFormat.SetError("Text", errorState, AdminStrings.Errors_TextNotWhitespace);
        }
    }
}