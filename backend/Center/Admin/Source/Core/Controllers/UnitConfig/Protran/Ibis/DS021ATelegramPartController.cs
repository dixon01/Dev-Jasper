// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS021ATelegramPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS021ATelegramPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Ibis
{
    using System.Linq;

    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;

    /// <summary>
    /// The DS021a telegram part controller.
    /// </summary>
    public class DS021ATelegramPartController : DS021TelegramPartControllerBase<DS021AConfig>
    {
        private SelectionEditorViewModel firstStopIndexValue;

        private SelectionEditorViewModel endingStopIndexValue;

        private CheckableEditorViewModel deleteRoute;

        private CheckableEditorViewModel connectionsEnabled;

        private TextEditorViewModel absoluteTimeFormat;

        private GenericUsageDS021BaseEditorViewModel usedForRelativeTime;

        private GenericUsageDS021BaseEditorViewModel usedForAbsoluteTime;

        private GenericUsageEditorViewModel usedForDestinationRelativeTime;

        private GenericUsageEditorViewModel usedForDestinationAbsoluteTime;

        private GenericUsageEditorViewModel usedForText;

        /// <summary>
        /// Initializes a new instance of the <see cref="DS021ATelegramPartController"/> class.
        /// </summary>
        /// <param name="telegramName">
        /// The telegram name.
        /// </param>
        /// <param name="defaultUsage">
        /// The default generic usage of the telegram.
        /// </param>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public DS021ATelegramPartController(
            string telegramName, GenericUsage defaultUsage, CategoryControllerBase parent)
            : base(telegramName, defaultUsage, parent)
        {
        }

        /// <summary>
        /// Gets a value indicating whether handling of connections is enabled.
        /// </summary>
        public bool ConnectionsEnabled
        {
            get
            {
                return this.connectionsEnabled != null && this.connectionsEnabled.IsChecked.HasValue
                       && this.connectionsEnabled.IsChecked.Value;
            }
        }

        /// <summary>
        /// Prepares the given telegram with the data configured in this part.
        /// </summary>
        /// <param name="telegram">
        /// The telegram to be filled with data.
        /// </param>
        protected override void PrepareTelegram(DS021AConfig telegram)
        {
            base.PrepareTelegram(telegram);

            // ReSharper disable PossibleInvalidOperationException
            telegram.FirstStopIndexValue = (int)this.firstStopIndexValue.SelectedValue;
            telegram.EndingStopValue = (int)this.endingStopIndexValue.SelectedValue;
            telegram.DeleteRouteIndexValue = this.deleteRoute.IsChecked.Value ? 0 : -1;
            telegram.AbsoluteTimeFormat = this.absoluteTimeFormat.Text;

            // ReSharper restore PossibleInvalidOperationException
            var connections = this.GetPart<DS021AConnectionsPartController>();
            telegram.Connection = connections.GetConfig();

            telegram.UsedForRelativeTime = this.usedForRelativeTime.GenericUsageDS021Base;
            telegram.UsedForAbsoluteTime = this.usedForAbsoluteTime.GenericUsageDS021Base;
            telegram.UsedForDestinationRelativeTime = this.usedForDestinationRelativeTime.GenericUsage;
            telegram.UsedForDestinationAbsoluteTime = this.usedForDestinationAbsoluteTime.GenericUsage;
            telegram.UsedForText = this.usedForText.GenericUsage;
        }

        /// <summary>
        /// Loads the configuration from the given telegram into this part's editors.
        /// </summary>
        /// <param name="telegram">
        /// The telegram to load the data from.
        /// </param>
        protected override void LoadFrom(DS021AConfig telegram)
        {
            base.LoadFrom(telegram);

            this.firstStopIndexValue.SelectValue(telegram.FirstStopIndexValue);
            this.endingStopIndexValue.SelectValue(telegram.EndingStopValue);
            this.deleteRoute.IsChecked = telegram.DeleteRouteIndexValue == 0;
            this.connectionsEnabled.IsChecked = telegram.Connection.Enabled;
            this.absoluteTimeFormat.Text = telegram.AbsoluteTimeFormat;

            this.usedForRelativeTime.GenericUsageDS021Base = telegram.UsedForRelativeTime;
            this.usedForAbsoluteTime.GenericUsageDS021Base = telegram.UsedForAbsoluteTime;
            this.usedForDestinationRelativeTime.GenericUsage = telegram.UsedForDestinationRelativeTime;
            this.usedForDestinationAbsoluteTime.GenericUsage = telegram.UsedForDestinationAbsoluteTime;
            this.usedForText.GenericUsage = telegram.UsedForText;
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

            this.connectionsEnabled = new CheckableEditorViewModel();
            this.connectionsEnabled.Label = AdminStrings.UnitConfig_Ibis_DS021a_ConnectionsEnabled;
            this.connectionsEnabled.IsThreeState = false;
            viewModel.Editors.Add(this.connectionsEnabled);

            this.firstStopIndexValue = new SelectionEditorViewModel();
            this.firstStopIndexValue.Label = AdminStrings.UnitConfig_Ibis_DS021c_FirstStopIndexValue;
            this.firstStopIndexValue.Options.Add(new SelectionOptionViewModel("0", 0));
            this.firstStopIndexValue.Options.Add(new SelectionOptionViewModel("1", 1));
            viewModel.Editors.Add(this.firstStopIndexValue);

            this.endingStopIndexValue = new SelectionEditorViewModel();
            this.endingStopIndexValue.Label = AdminStrings.UnitConfig_Ibis_DS021a_EndingStopIndexValue;
            this.endingStopIndexValue.Options.Add(new SelectionOptionViewModel("99", 99));
            this.endingStopIndexValue.Options.Add(new SelectionOptionViewModel("999", 999));
            viewModel.Editors.Add(this.endingStopIndexValue);

            this.deleteRoute = new CheckableEditorViewModel();
            this.deleteRoute.Label = AdminStrings.UnitConfig_Ibis_DS021a_DeleteRoute;
            this.deleteRoute.IsThreeState = false;
            viewModel.Editors.Add(this.deleteRoute);

            this.absoluteTimeFormat = new TextEditorViewModel();
            this.absoluteTimeFormat.Label = AdminStrings.UnitConfig_Ibis_DS021a_AbsoluteTimeFormat;
            viewModel.Editors.Add(this.absoluteTimeFormat);

            var index =
                viewModel.Editors.IndexOf(
                    viewModel.Editors.OfType<GenericUsageDS021BaseEditorViewModel>().LastOrDefault());

            this.usedForRelativeTime = new GenericUsageDS021BaseEditorViewModel();
            this.usedForRelativeTime.Label = AdminStrings.UnitConfig_Ibis_DS021a_UsedForRelativeTime;
            this.usedForRelativeTime.ShouldShowRow = false;
            this.usedForRelativeTime.IsNullable = true;
            viewModel.Editors.Insert(++index, this.usedForRelativeTime);

            this.usedForAbsoluteTime = new GenericUsageDS021BaseEditorViewModel();
            this.usedForAbsoluteTime.Label = AdminStrings.UnitConfig_Ibis_DS021a_UsedForAbsoluteTime;
            this.usedForAbsoluteTime.ShouldShowRow = false;
            this.usedForAbsoluteTime.IsNullable = true;
            viewModel.Editors.Insert(++index, this.usedForAbsoluteTime);

            index =
                viewModel.Editors.IndexOf(viewModel.Editors.OfType<GenericUsageEditorViewModel>().LastOrDefault());

            this.usedForDestinationRelativeTime = new GenericUsageEditorViewModel();
            this.usedForDestinationRelativeTime.Label =
                AdminStrings.UnitConfig_Ibis_DS021a_UsedForDestinationRelativeTime;
            this.usedForDestinationRelativeTime.ShouldShowRow = true;
            this.usedForDestinationRelativeTime.IsNullable = true;
            viewModel.Editors.Insert(++index, this.usedForDestinationRelativeTime);

            this.usedForDestinationAbsoluteTime = new GenericUsageEditorViewModel();
            this.usedForDestinationAbsoluteTime.Label =
                AdminStrings.UnitConfig_Ibis_DS021a_UsedForDestinationAbsoluteTime;
            this.usedForDestinationAbsoluteTime.ShouldShowRow = true;
            this.usedForDestinationAbsoluteTime.IsNullable = true;
            viewModel.Editors.Insert(++index, this.usedForDestinationAbsoluteTime);

            this.usedForText = new GenericUsageEditorViewModel();
            this.usedForText.Label = AdminStrings.UnitConfig_Ibis_DS021a_UsedForText;
            this.usedForText.ShouldShowRow = false;
            this.usedForText.IsNullable = true;
            viewModel.Editors.Insert(++index, this.usedForText);

            return viewModel;
        }
    }
}