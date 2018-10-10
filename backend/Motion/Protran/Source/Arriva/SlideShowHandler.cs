// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SlideShowHandler.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SlideShowHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Arriva
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.Protocols.Ximple.Utils;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.Core.Buffers;

    using NLog;

    /// <summary>
    /// The slide show handler.
    /// </summary>
    public class SlideShowHandler : IManageableTable, IManageableObject
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary dictionary;

        private readonly List<SlideShowInfo> messages = new List<SlideShowInfo>();

        private readonly ITimer timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SlideShowHandler"/> class.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        public SlideShowHandler(Dictionary dictionary)
        {
            this.dictionary = dictionary;
            this.timer = TimerFactory.Current.CreateTimer("ArrivaSlideshow");
            this.timer.AutoReset = false;
            this.timer.Elapsed += this.TimerOnElapsed;
        }

        /// <summary>
        /// The message valid.
        /// </summary>
        public event EventHandler<XimpleEventArgs> XimpleCreated;

        /// <summary>
        /// The validate message.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public void AddSlideShowMessage(SlideShowMessage data)
        {
            var info = new SlideShowInfo(data);
            lock (this.messages)
            {
                this.messages.Add(info);
                this.messages.Sort();

                this.timer.Enabled = false;
                var time = TimeProvider.Current.Now;
                this.SendUpdates(time);
                this.RestartTimer(time);
            }
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield break;
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            yield return new ManagementProperty<int>("Number of Slideshow messages", this.messages.Count, true);
        }

        IEnumerable<List<ManagementProperty>> IManageableTable.GetRows()
        {
            foreach (var slideShowInfo in this.messages)
            {
                yield return new List<ManagementProperty>
                                 {
                                     new ManagementProperty<bool>(
                                         "Slideshow message showing", slideShowInfo.Showing, true),
                                     new ManagementProperty<string>(
                                         "Slideshow message header", slideShowInfo.Message.HeaderText.ToString(), true),
                                     new ManagementProperty<string>(
                                         "Slideshow message", slideShowInfo.Message.MsgText.ToString(), true),
                                     new ManagementProperty<string>(
                                         "Start validity",
                                         slideShowInfo.StartValidity.ToString(CultureInfo.InvariantCulture),
                                         true),
                                     new ManagementProperty<string>(
                                         "End validity",
                                         slideShowInfo.EndValidity.ToString(CultureInfo.InvariantCulture),
                                         true),
                                 };
            }
        }

        private void RestartTimer(DateTime time)
        {
            var nextValid = this.messages.Find(i => i.StartValidity >= time);
            var allEndValid = this.messages.FindAll(i => i.EndValidity >= time);
            allEndValid.Sort((a, b) => a.EndValidity.CompareTo(b.EndValidity));
            var nextEndValid = allEndValid.Count > 0 ? allEndValid[0] : null;

            if (nextValid == null && nextEndValid == null)
            {
                return;
            }

            var nextCheck = nextEndValid == null
                || (nextValid != null && nextValid.StartValidity < nextEndValid.EndValidity)
                                ? nextValid.StartValidity
                                : nextEndValid.EndValidity;
            this.timer.Interval = nextCheck - time;
            this.timer.Enabled = true;
        }

        private void SendUpdates(DateTime time)
        {
            var ximple = new Ximple(Constants.Version2);
            var remove = new List<SlideShowInfo>();

            foreach (var info in this.messages)
            {
                if (info.EndValidity <= time)
                {
                    this.RemoveMessage(ximple, info.Message);
                    remove.Add(info);
                }
                else if (info.StartValidity <= time && !info.Showing)
                {
                    info.Showing = true;
                    this.AddMessage(ximple, info.Message);
                }
            }

            foreach (var info in remove)
            {
                this.messages.Remove(info);
            }

            if (ximple.Cells.Count > 0)
            {
                Logger.Info("Sent SlideShow XIMPLE");
                Logger.Trace(() => "SlideShow XIMPLE is: " + ximple.ToXmlString());
                this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
            }
        }

        private void AddMessage(Ximple ximple, SlideShowMessage message)
        {
            var type = message.Id.ToString(CultureInfo.InvariantCulture);
            var title = BufferUtils.FromBytesToString(message.HeaderText);
            var text = BufferUtils.FromBytesToString(message.MsgText);
            this.AddXimpleCells(ximple, message.Id, type, title, text);
        }

        private void RemoveMessage(Ximple ximple, SlideShowMessage message)
        {
            this.AddXimpleCells(ximple, message.Id, string.Empty, string.Empty, string.Empty);
        }

        private void AddXimpleCells(
            Ximple ximple, int row, string type, string title, string text)
        {
            var table = this.dictionary.GetTableForNameOrNumber("PassengerMessages");
            if (table == null)
            {
                return;
            }

            this.AddXimpleCell(table, "MessageType", row, type, ximple);
            this.AddXimpleCell(table, "MessageTitle", row, title, ximple);
            this.AddXimpleCell(table, "MessageText", row, text, ximple);
        }

        private void AddXimpleCell(Table table, string columnSpeakingName, int row, string value, Ximple ximple)
        {
            var column = table.GetColumnForNameOrNumber(columnSpeakingName);
            if (column == null)
            {
                // no column found.
                // I cannot translate the current infomation to a generic view's cell.
                return;
            }

            var genViewCell = new XimpleCell
            {
                RowNumber = row,
                ColumnNumber = column.Index,
                TableNumber = table.Index,
                LanguageNumber = 1,
                Value = value
            };

            ximple.Cells.Add(genViewCell);
        }

        private void TimerOnElapsed(object sender, EventArgs eventArgs)
        {
            lock (this.messages)
            {
                var time = TimeProvider.Current.Now;
                this.SendUpdates(time);
                this.RestartTimer(time);
            }
        }

        private void RaiseXimpleCreated(XimpleEventArgs args)
        {
            var handler = this.XimpleCreated;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private class SlideShowInfo : IComparable<SlideShowInfo>
        {
            public SlideShowInfo(SlideShowMessage message)
            {
                this.Message = message;
                this.StartValidity = BufferUtils.FromBytesToUnix64BitTimestamp(message.StartOfValidity);
                this.EndValidity = BufferUtils.FromBytesToUnix64BitTimestamp(message.EndOfValidity);
            }

            public SlideShowMessage Message { get; private set; }

            public DateTime StartValidity { get; private set; }

            public DateTime EndValidity { get; private set; }

            public bool Showing { get; set; }

            public int CompareTo(SlideShowInfo other)
            {
                return this.StartValidity.CompareTo(other.StartValidity);
            }
        }
    }
}