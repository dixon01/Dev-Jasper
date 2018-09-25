// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemTimeOutput.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemTimeOutput type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Common
{
    using System;

    using Gorba.Common.Gioom.Core;
    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Medi.Core;

    /// <summary>
    /// GIOoM port for setting the system time.
    /// </summary>
    public partial class SystemTimeOutput : SimplePort
    {
        /// <summary>
        /// The date / time value defined to be zero.
        /// </summary>
        public static readonly DateTime Zero = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private readonly bool broadcastTimeChanges;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemTimeOutput"/> class.
        /// </summary>
        /// <param name="broadcastTimeChanges">
        /// A flag indicating if the application should broadcast any changes done
        /// to this output to all other hardware managers and if so, it will
        /// also listen to broadcast messages arriving.
        /// </param>
        public SystemTimeOutput(bool broadcastTimeChanges)
            : base("SystemTime", false, true, new IntegerValues(0, int.MaxValue), 0)
        {
            this.broadcastTimeChanges = broadcastTimeChanges;

            if (broadcastTimeChanges)
            {
                MessageDispatcher.Instance.Subscribe<SystemTimeUpdate>(this.HandleSystemTimeUpdate);
            }
        }

        /// <summary>
        /// Gets or sets the current I/O value of this port.
        /// Setting the value might not immediately change the value
        /// returned by the getter. Especially if the port is on a remote
        /// Medi node, it might take some time until the value actually changes.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// if the port value is read since it is not readable.
        /// </exception>
        public override IOValue Value
        {
            get
            {
                throw new NotSupportedException("Can't read system time");
            }

            set
            {
                var dateTime = Zero + TimeSpan.FromSeconds(value.Value);

                this.SetSystemTime(dateTime);

                if (this.broadcastTimeChanges)
                {
                    MessageDispatcher.Instance.Send(
                        new MediAddress("*", MessageDispatcher.Instance.LocalAddress.Application),
                        new SystemTimeUpdate { SystemTimeUtc = dateTime });
                }
            }
        }

        private void SetSystemTime(DateTime dateTime)
        {
            var time = new NativeMethods.SystemTime
                           {
                               Year = (ushort)dateTime.Year,
                               Month = (ushort)dateTime.Month,
                               Day = (ushort)dateTime.Day,
                               Hour = (ushort)dateTime.Hour,
                               Minute = (ushort)dateTime.Minute,
                               Second = (ushort)dateTime.Second
                           };
            if (!NativeMethods.SetSystemTime(ref time))
            {
                throw new InvalidOperationException(
                    "Couldn't update system time (do you have the necessary access rights?)");
            }
        }

        private void HandleSystemTimeUpdate(object sender, MessageEventArgs<SystemTimeUpdate> e)
        {
            if (e.Source.Equals(MessageDispatcher.Instance.LocalAddress))
            {
                // prevent an infinite loop
                return;
            }

            this.SetSystemTime(e.Message.SystemTimeUtc);
        }

        private static partial class NativeMethods
        {
            public struct SystemTime
            {
                // ReSharper disable NotAccessedField.Local
                // ReSharper disable UnusedMember.Local
                public ushort Year;
                public ushort Month;
                public ushort DayOfWeek;
                public ushort Day;
                public ushort Hour;
                public ushort Minute;
                public ushort Second;
                public ushort Millisecond;

                // ReSharper restore UnusedMember.Local
                // ReSharper restore NotAccessedField.Local
            }
        }
    }
}
