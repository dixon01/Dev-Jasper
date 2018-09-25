// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vdv301LogsControl.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Vdv301LogsControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.Vdv301
{
    using System;
    using System.IO;
    using System.Windows.Forms;
    using System.Xml.Serialization;

    using Gorba.Common.Protocols.Vdv301.Services;
    using Gorba.Motion.Protran.Visualizer.Data;
    using Gorba.Motion.Protran.Visualizer.Data.Vdv301;

    /// <summary>
    /// The logs control for VDV 301.
    /// </summary>
    public partial class Vdv301LogsControl : LogsControl, IVdv301VisualizationControl
    {
        private readonly XmlEditorControl xmlEditor;
        private readonly Vdv301TransformationControl transformationControl;

        private IVdv301VisualizationService visualizationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vdv301LogsControl"/> class.
        /// </summary>
        public Vdv301LogsControl()
        {
            this.InitializeComponent();

            this.xmlEditor = new XmlEditorControl { IsReadOnly = true };
            this.transformationControl = new Vdv301TransformationControl();
        }

        /// <summary>
        /// Configure the control with the given service.
        /// </summary>
        /// <param name="service">
        /// The service to which you can for example attach event handlers
        /// </param>
        public void Configure(IVdv301VisualizationService service)
        {
            this.visualizationService = service;
            this.visualizationService.DataUpdated += this.VisualizationServiceOnDataUpdated;
            this.visualizationService.ElementTransformed += this.VisualizationServiceOnElementTransformed;
            this.visualizationService.XimpleCreated += (s, e) => this.AddXimpleEvent(e);
        }

        /// <summary>
        /// Subclasses have to provide a (reusable) control that can be shown
        /// when an item in the log list was double-clicked.
        /// The control has to be filled with the provided event information.
        /// Subclasses should always call the base method.
        /// </summary>
        /// <param name="timestamp">The timestamp of the event to represent.</param>
        /// <param name="type">The type of event to represent.</param>
        /// <param name="description">The description.</param>
        /// <param name="e">The event arguments.</param>
        /// <returns>a control (this method should not create a new control for each request) or null if the
        /// event can't be shown in a detail control.</returns>
        protected override Control GetDetailControl(DateTime timestamp, EventType type, string description, EventArgs e)
        {
            switch (type)
            {
                case EventType.TelegramIn:
                    this.xmlEditor.XmlText = ToXmlString(((DataUpdateEventArgs<object>)e).Value);
                    return this.xmlEditor;
                case EventType.Transform:
                    this.transformationControl.Populate((TransformationChainEventArgs)e);
                    return this.transformationControl;
                default:
                    return base.GetDetailControl(timestamp, type, description, e);
            }
        }

        private static string ToXmlString(object value)
        {
            var serializer = new XmlSerializer(value.GetType());
            var writer = new StringWriter();
            serializer.Serialize(writer, value);
            return writer.ToString();
        }

        private void VisualizationServiceOnDataUpdated(object sender, DataUpdateEventArgs<object> e)
        {
            var dataType = e.Value.GetType();
            var description = dataType.Name;
            var attrs = dataType.GetCustomAttributes(typeof(XmlTypeAttribute), false);
            if (attrs.Length > 0)
            {
                description = ((XmlTypeAttribute)attrs[0]).TypeName;
            }

            this.AddEvent(EventType.TelegramIn, description, e);
        }

        private void VisualizationServiceOnElementTransformed(object sender, TransformationChainEventArgs e)
        {
            this.AddEvent(EventType.Transform, e.ChainName, e);
        }
    }
}
