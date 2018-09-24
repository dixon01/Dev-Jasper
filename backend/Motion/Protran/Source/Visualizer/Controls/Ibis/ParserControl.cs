// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParserControl.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ParserControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.Ibis
{
    using System;
    using System.Windows.Forms;

    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Motion.Protran.Visualizer.Data.Ibis;

    /// <summary>
    /// The control showing parser information (binary data, telegram, answer).
    /// </summary>
    public partial class ParserControl : UserControl, IIbisVisualizationControl
    {
        private IbisConfig config;
        private SideTab sideTab;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserControl"/> class.
        /// </summary>
        public ParserControl()
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
            this.config = ctrl.Config;
            ctrl.TelegramParsing += this.OnTelegramParsing;
            ctrl.TelegramParsed += this.OnTelegramParsed;
        }

        /// <summary>
        /// Assigns a <see cref="SideTab"/> to this control.
        /// This can be used to keep a reference to the tab
        /// and update it when events arrive.
        /// </summary>
        /// <param name="tab">
        /// The side tab.
        /// </param>
        public void SetSideTab(SideTab tab)
        {
            this.sideTab = tab;
        }

        /// <summary>
        /// Populates this control with the given event args.
        /// This is used to fill the control when shown from the log view.
        /// </summary>
        /// <param name="cfg">
        /// The IBIS configuration.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        public void Populate(IbisConfig cfg, TelegramParsedEventArgs e)
        {
            this.config = cfg;
            this.OnTelegramParsing(this, EventArgs.Empty);
            this.OnTelegramParsed(this, e);
        }

        private void OnTelegramParsing(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler(this.OnTelegramParsing));
                return;
            }

            this.textBoxData.Clear();
            this.propertyGridParser.SelectedObject = null;
            this.propertyGridTelegram.SelectedObject = null;
            this.propertyGridAnswer.SelectedObject = null;

            if (this.sideTab != null)
            {
                this.sideTab.Description = string.Empty;
            }
        }

        private void OnTelegramParsed(object sender, TelegramParsedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler<TelegramParsedEventArgs>(this.OnTelegramParsed), sender, e);
                return;
            }

            this.textBoxData.Text = TelegramFormatter.ToTelegramString(e.Data, this.config.Behaviour.ByteType);
            this.propertyGridParser.SelectedObject = e.Parser;
            this.propertyGridTelegram.SelectedObject = e.Telegram;
            this.propertyGridAnswer.SelectedObject = e.Answer;

            if (this.sideTab != null)
            {
                this.sideTab.Description = e.Telegram != null ? e.Telegram.GetType().Name : "<Unknown Telegram>";
            }
        }
    }
}
