// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisLogsControl.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisLogsControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.Ibis
{
    using System;
    using System.Windows.Forms;

    using Gorba.Motion.Protran.Visualizer.Data;
    using Gorba.Motion.Protran.Visualizer.Data.Ibis;

    /// <summary>
    /// Logs control for IBIS protocol.
    /// </summary>
    public partial class IbisLogsControl : LogsControl, IIbisVisualizationControl
    {
        private readonly ParserControl parserControl = new ParserControl();
        private readonly IbisTransformationControl transformationControl = new IbisTransformationControl();

        private IIbisVisualizationService controller;

        /// <summary>
        /// Initializes a new instance of the <see cref="IbisLogsControl"/> class.
        /// </summary>
        public IbisLogsControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Configure the control with the given controller.
        /// </summary>
        /// <param name="ctrl">
        ///   The controller to which you can for example attach event handlers
        /// </param>
        public void Configure(IIbisVisualizationService ctrl)
        {
            this.controller = ctrl;
            ctrl.TelegramParsed += this.OnTelegramParsed;
            ctrl.TelegramTransformed += this.OnTelegramTransformed;
            ctrl.HandlingTelegram += this.OnHandlingTelegram;
            ctrl.XimpleCreated += (s, e) => this.AddXimpleEvent(e);
        }

        /// <summary>
        /// Provides a (reusable) control that can be shown
        /// when an item in the log list was double-clicked.
        /// The control has to be filled with the provided event information.
        /// </summary>
        /// <param name="timestamp">The timestamp of the event to represent.</param>
        /// <param name="type">The type of event to represent.</param>
        /// <param name="description">The description.</param>
        /// <param name="e">The event arguments.</param>
        /// <returns>a control or null if the event can't be shown in a detail control.</returns>
        protected override Control GetDetailControl(DateTime timestamp, EventType type, string description, EventArgs e)
        {
            switch (type)
            {
                case EventType.TelegramIn:
                case EventType.TelegramOut:
                case EventType.Parse:
                    this.parserControl.Populate(this.controller.Config, (TelegramParsedEventArgs)e);
                    return this.parserControl;
                case EventType.Transform:
                    this.transformationControl.Populate((TransformationChainEventArgs)e);
                    return this.transformationControl;
                case EventType.Handle:
                    return null;
                default:
                    return base.GetDetailControl(timestamp, type, description, e);
            }
        }

        private void OnTelegramParsed(object sender, TelegramParsedEventArgs e)
        {
            this.AddEvent(
                EventType.TelegramIn,
                TelegramFormatter.ToTelegramString(e.Data, this.controller.Config.Behaviour.ByteType),
                e);

            if (e.Parser != null)
            {
                var description = e.Telegram != null
                                      ? string.Format(
                                          "{0} -> {1}",
                                          TypePropertyGrid.GetTypeName(e.Parser.GetType()),
                                          e.Telegram.GetType().Name)
                                      : e.Parser.GetType().Name;
                this.AddEvent(EventType.Parse, description, e);
            }

            if (e.Answer != null)
            {
                this.AddEvent(EventType.TelegramOut, e.Answer.GetType().Name, e);
            }
        }

        private void OnTelegramTransformed(object sender, TransformationChainEventArgs e)
        {
            this.AddEvent(EventType.Transform, e.ChainName, e);
        }

        private void OnHandlingTelegram(object sender, TelegramHandlerEventArgs e)
        {
            this.AddEvent(EventType.Handle, TypePropertyGrid.GetTypeName(e.Handler.GetType()), e);
        }
    }
}
