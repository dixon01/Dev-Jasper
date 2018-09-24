// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AlarmScreen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AlarmScreen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Screens
{
    using System.Collections.Generic;

    using Gorba.Motion.Edi.Core;
    using Gorba.Motion.Obc.Terminal.Control.Alarm;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The alarm list screen.
    /// </summary>
    internal class AlarmScreen : SimpleListScreen
    {
        private readonly GenericAlarms alarms;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlarmScreen"/> class.
        /// </summary>
        /// <param name="mainField">
        /// The main field.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public AlarmScreen(IList mainField, IContext context)
            : base(mainField, context)
        {
            this.alarms = new GenericAlarms();
        }

        /// <summary>
        /// Gets the list that will be shown to the user. The user will/may select an item from this list.
        /// </summary>
        protected override List<string> List
        {
            get
            {
                return this.alarms.GetAllAlarmNames();
            }
        }

        /// <summary>
        /// Gets the caption from the screen
        /// </summary>
        protected override string Caption
        {
            get
            {
                return ml.ml_string(46, "Alarm");
            }
        }

        /// <summary>
        /// Shows this screen.
        /// </summary>
        public override void Show()
        {
            base.Show();
            this.SendAlarm(GenericAlarms.GenericAlarm);
        }

        /// <summary>
        ///   This method will be called when the user has selected an entry.
        ///   Implement your action here. The index is the selected item from the GetList() method
        /// </summary>
        /// <param name = "index">
        /// The selected index.
        /// </param>
        protected override void ItemSelected(int index)
        {
            this.SendAlarm(this.alarms.GetAlarmId(index));
            this.Context.ShowRootScreen();
        }

        /// <summary>
        /// Handles the escape key.
        /// </summary>
        protected override void HandleEscapeKey()
        {
            // Not allowed. When user entered in Alarm screen, he has to handle it
        }

        private void SendAlarm(int alarmId)
        {
            // Maybe in icenter.Motion the user allready pressed button end alarm. in this case
            // the alarm state has to be changed again to reported
            var state = this.Context.AlarmHandler.AlarmState;
            if (state == AlarmState.Inactive)
            {
                state = AlarmState.Reported;
            }

            this.Context.AlarmHandler.SetAlarmState(state, alarmId);
        }
    }
}