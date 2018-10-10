// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisProtocolCategoryController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisProtocolCategoryController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Ibis
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Software;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Common.Configuration.HardwareDescription;
    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;

    /// <summary>
    /// The IBIS protocol category controller.
    /// </summary>
    public class IbisProtocolCategoryController : CategoryControllerBase
    {
        private List<PartControllerBase> controllers;

        private IncomingPartController incoming;

        private IbisGeneralPartController general;

        /// <summary>
        /// Initializes a new instance of the <see cref="IbisProtocolCategoryController"/> class.
        /// </summary>
        public IbisProtocolCategoryController()
            : base(UnitConfigKeys.IbisProtocol.Category)
        {
        }

        /// <summary>
        /// Gets all telegram part controllers in this category.
        /// </summary>
        /// <returns>
        /// The list of all telegram part controllers.
        /// </returns>
        public IEnumerable<IbisTelegramPartControllerBase> GetTelegramControllers()
        {
            return this.controllers.OfType<IbisTelegramPartControllerBase>();
        }

        /// <summary>
        /// Asynchronously prepares this category controller and all its children with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        /// The hardware descriptor.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to wait on.
        /// </returns>
        public override Task PrepareAsync(HardwareDescriptor descriptor)
        {
            this.incoming = this.Parent.GetPart<IncomingPartController>();
            this.incoming.ViewModelUpdated += (s, e) => this.UpdateVisibility();
            this.UpdateVisibility();

            return base.PrepareAsync(descriptor);
        }

        /// <summary>
        /// Creates all part controllers.
        /// </summary>
        /// <returns>
        /// An enumeration of the part controllers of this category.
        /// </returns>
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Listing of all telegrams, don't want to split it into multiple methods")]
        protected override IEnumerable<PartControllerBase> CreatePartControllers()
        {
            this.general = new IbisGeneralPartController(this);
            this.general.ViewModelUpdated += (s, e) => this.UpdateVisibility();
            this.controllers = new List<PartControllerBase>
                                   {
                                       this.general,
                                       new IbisSimulationPartController(this),
                                       new IbisUdpServerPartController(this),
                                       new IbisTelegramSelectionPartController(this),

                                       // Telegrams
                                       new SimpleTelegramPartController<SimpleTelegramConfig>(
                                           "DS001", new GenericUsage("Route", "Line"), this),
                                       new SimpleTelegramPartController<SimpleTelegramConfig>(
                                           "DS001a", new GenericUsage("Route", "SpecialLine"), this),
                                       new SimpleTelegramPartController<SimpleTelegramConfig>(
                                           "DS002", new GenericUsage("Route", "Run"), this)
                                           {
                                               TelegramType = TelegramType.Integer
                                           },
                                       new SimpleTelegramPartController<SimpleTelegramConfig>(
                                           "DS003", new GenericUsage("Destination", "DestinationInfo"), this)
                                           {
                                               TelegramType = TelegramType.Integer
                                           },
                                       new SimpleTelegramPartController<SimpleTelegramConfig>(
                                           "DS003a", new GenericUsage("Destination", "DestinationName"), this),
                                       new SimpleTelegramPartController<SimpleTelegramConfig>(
                                           "DS003c", new GenericUsage("Route", "Region"), this),

                                       // we skip DS005, DS0006 and DS006a since they are defined by time sync
                                       new SimpleTelegramPartController<SimpleTelegramConfig>(
                                           "DS008", new GenericUsage("Route", "Region"), this),
                                       new OptionalDataTelegramPartController<SimpleTelegramConfig>(
                                           "DS009", new GenericUsage("Stops", "StopName"), this),
                                       new OptionalDataTelegramPartController<SimpleTelegramConfig>(
                                           "DS010", new GenericUsage("Route", "CurrentStopIndex"), this),
                                       new OptionalDataTelegramPartController<SimpleTelegramConfig>(
                                           "DS010b", new GenericUsage("Route", "CurrentStopIndex"), this)
                                           {
                                               TelegramType = TelegramType.Integer
                                           },
                                       new OptionalDataTelegramPartController<SimpleTelegramConfig>(
                                           "DS010j", new GenericUsage("Route", "CurrentStopIndex"), this)
                                           {
                                               TelegramType = TelegramType.Integer
                                           },
                                       new DS020TelegramPartController("DS020", this)
                                           {
                                               TelegramType = TelegramType.Empty
                                           },
                                       new AnswerWithDS120TelegramPartController<SimpleTelegramConfig>(
                                           "DS021", new GenericUsage("Destination", "DestinationName"), this),
                                       new DS021ATelegramPartController(
                                           "DS021a", new GenericUsage("Stops", "StopName"), this),
                                       new DS021AConnectionsPartController(this),
                                       new DS021CTelegramPartController(
                                           "DS021c", new GenericUsage("Stops", "StopName"), this),
                                       new AnswerWithDS130TelegramPartController<DS036Config>(
                                           "DS036", new GenericUsage("Route", "AnnouncementIndex"), this),
                                       new DS080TelegramPartController(
                                           "DS080", new GenericUsage("SystemStatus", "DoorStatus"), this)
                                           {
                                               TelegramType = TelegramType.Empty
                                           },
                                       new DS081TelegramPartController(
                                           "DS081", new GenericUsage("SystemStatus", "DoorStatus"), this)
                                           {
                                               TelegramType = TelegramType.Empty
                                           },
                                       new SimpleTelegramPartController<SimpleTelegramConfig>(
                                           "GO001", new GenericUsage("Route", "ApproachingStop"), this)
                                           {
                                               TelegramType = TelegramType.Integer
                                           },

                                       // TODO: add GO002 if needed (big effort)
                                       ////new GO002TelegramPartController(
                                       ////    "GO002",
                                       ////    new GenericUsage("Connections", "ConnectionDestinationName"), this),
                                       new GO003TelegramPartController(
                                           "GO003", new GenericUsage("Stops", "StopName"), this),
                                       new GO004TelegramPartController(
                                           "GO004", new GenericUsage("PassengerMessages", "MessageText"), this),
                                       new GO005TelegramPartController(
                                           "GO005", new GenericUsage("Stops", "StopName"), this),
                                       new SimpleTelegramPartController<SimpleTelegramConfig>(
                                           "GO006", new GenericUsage("Route", "Line"), this),
                                       new GO007TelegramPartController(
                                           "GO007", new GenericUsage("Stops", "StopName"), this),
                                       new HPW074TelegramPartController(
                                           "HPW074", new GenericUsage("PassengerMessages", "MessageText"), this),
                                   };
            return this.controllers;
        }

        /// <summary>
        /// Prepares the category view model (e.g. setting its name, ...).
        /// </summary>
        protected override void PrepareViewModel()
        {
            this.ViewModel.DisplayName = AdminStrings.UnitConfig_Ibis;
        }

        private void UpdateVisibility()
        {
            if (this.incoming == null || this.general == null)
            {
                return;
            }

            var visible = this.incoming.HasSelected(IncomingData.Ibis);
            this.general.ViewModel.IsVisible = visible;

            foreach (var part in this.controllers.OfType<IFilteredPartController>())
            {
                part.UpdateVisibility(visible);
            }
        }
    }
}
