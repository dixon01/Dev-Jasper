// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CycleManager.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.StateMachineCycles
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Gorba.Common.Gioom.Core.Utility;
    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Motion.Protran.AbuDhabi.Config;
    using Gorba.Motion.Protran.AbuDhabi.Ibis;
    using Gorba.Motion.Protran.Core.Utils;

    using NLog;

    /// <summary>
    /// Class to handle cycle management.
    /// </summary>
    public class CycleManager : IStateContext, IManageableObject
    {
        /// <summary>
        /// The logger used by this whole protocol.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly object locker = new object();

        private State currentState;

        private State newState;

        private State.CycleStateValue currentStateValue;

        private string previousStopName;

        private string previousStopApproachValue;

        private GenericUsageHandler cycleUsage;

        private PortListener stopRequestListener;

        /// <summary>
        /// Initializes a new instance of the <see cref="CycleManager"/> class.
        /// </summary>
        public CycleManager()
        {
            this.currentState = new MainCycleState();
            this.newState = new MainCycleState();
            this.previousStopName = string.Empty;
            this.previousStopApproachValue = string.Empty;
        }

        /// <summary>
        /// Event that is fired whenever the this object creates
        /// a new ximple object.
        /// </summary>
        public event EventHandler<XimpleEventArgs> XimpleCreated;

        /// <summary>
        /// Gets Dictionary.
        /// </summary>
        public Dictionary Dictionary { get; private set; }

        /// <summary>
        /// Gets Config.
        /// </summary>
        public AbuDhabiConfig Config { get; private set; }

        /// <summary>
        /// Gets or sets CurrentProtocol.
        /// </summary>
        public IbisProtocolHost CurrentProtocol { get; set; }

        /// <summary>
        /// Configures this manager with the given config and dictionary.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        public void Configure(AbuDhabiConfig config, Dictionary dictionary)
        {
            this.Dictionary = dictionary;
            this.Config = config;
            this.cycleUsage = new GenericUsageHandler(config.Behaviour.UsedForCycle, dictionary);

            this.stopRequestListener = new PortListener(MessageDispatcher.Instance.LocalAddress, "StopRequest");
            this.stopRequestListener.ValueChanged += this.StopRequestListenerOnValueChanged;
            this.stopRequestListener.Start(TimeSpan.FromSeconds(10));
        }

        /// <summary>
        /// Extract Data from Ximple
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public void ExtractDatafromXimple(Ximple data)
        {
            lock (this.locker)
            {
                foreach (var cell in data.Cells)
                {
                    var table = this.Dictionary.GetTableForNameOrNumber("Route");
                    var column = table.GetColumnForNameOrNumber("ApproachingStop");
                    if (cell.TableNumber == table.Index && cell.ColumnNumber == column.Index && cell.RowNumber == 0)
                    {
                        Logger.Debug("Received stop approaching cell with value: {0}", cell.Value);
                        this.ProcessStopApproach(cell.Value);
                        this.previousStopApproachValue = cell.Value;
                    }

                    table = this.Dictionary.GetTableForNameOrNumber("Stops");
                    column = table.GetColumnForNameOrNumber("StopName");
                    if (cell.TableNumber == table.Index && cell.ColumnNumber == column.Index && cell.RowNumber == 1)
                    {
                        Logger.Debug("Received next stop name: {0}", cell.Value);
                        this.ProcessNextStop(cell.Value);
                        this.previousStopName = cell.Value;
                    }

                    this.currentState = this.newState;
                    Logger.Trace("Current state of the cycle is: {0}", this.currentState);
                }
            }
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield break;
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            yield return new ManagementProperty<string>("Current cycle state", this.currentState.ToString(), true);
        }

        private void StopRequestListenerOnValueChanged(object sender, EventArgs eventArgs)
        {
            lock (this.locker)
            {
                this.ProcessStopRequest(this.stopRequestListener.Value);

                this.currentState = this.newState;
                Logger.Trace("Current state of the cycle is: {0}", this.currentState);
            }
        }

        /// <summary>
        /// Stop request event
        /// </summary>
        private void EventStopRequest()
        {
            // this.newState = this.currentState.EventStopRequest(this);
            var value = this.currentState.EventStopRequest();
            this.SendStateChangeValue(value);
        }

        /// <summary>
        /// Removed stop request
        /// </summary>
        private void EventRemovedStopRequest()
        {
            var value = this.currentState.EventRemovedStopRequest();
            this.SendStateChangeValue(value);
        }

        /// <summary>
        /// Stop approach event
        /// </summary>
        private void EventStopApproach()
        {
            var value = this.currentState.EventStopApproach();
            this.SendStateChangeValue(value);
        }

        /// <summary>
        /// Removed Stop approach event
        /// </summary>
        private void EventRemovedStopApproach()
        {
            var value = this.currentState.EventRemovedStopApproach();
            this.SendStateChangeValue(value);
        }

        /// <summary>
        /// Next stop event
        /// </summary>
        private void EventNextStop()
        {
            var value = this.currentState.EventNextStop();
            this.SendStateChangeValue(value);
        }

        private void ProcessNextStop(string cellValue)
        {
            if (!string.IsNullOrEmpty(cellValue) && this.previousStopName == cellValue)
            {
                return;
            }

            this.previousStopName = cellValue;
            this.EventNextStop();
        }

        private void ProcessStopRequest(IOValue value)
        {
            if (value == null)
            {
                return;
            }

            if (value.Equals(FlagValues.True))
            {
                this.EventStopRequest();
            }
            else
            {
                this.EventRemovedStopRequest();
            }
        }

        private void ProcessStopApproach(string cellValue)
        {
            if (string.IsNullOrEmpty(cellValue) || this.previousStopApproachValue == cellValue)
            {
                return;
            }

            if (cellValue == "1")
            {
                this.EventStopApproach();
            }
            else
            {
                this.EventRemovedStopApproach();
            }
        }

        private void SendStateChangeValue(State.CycleStateValue nextStateValue)
        {
            if (nextStateValue == this.currentStateValue)
            {
                return;
            }

            var ximple = new Ximple();
            this.cycleUsage.AddCell(ximple, ((int)nextStateValue).ToString(CultureInfo.InvariantCulture));
            this.RaiseXimpleCreated(new XimpleEventArgs(ximple));

            this.UpdateNewState(nextStateValue);
        }

        private void UpdateNewState(State.CycleStateValue nextStateValue)
        {
            this.currentStateValue = nextStateValue;
            switch (nextStateValue)
            {
                case State.CycleStateValue.MainCycleValue:
                    {
                        this.newState = new MainCycleState();
                        break;
                    }

                case State.CycleStateValue.StopRequestCycleValue:
                    {
                        this.newState = new StopRequestCycleState();
                        break;
                    }

                case State.CycleStateValue.StopApproachingCycleValue:
                    {
                        this.newState = new StopApproachingCycleState();
                        break;
                    }

                case State.CycleStateValue.StopReqStopApprCycleValue:
                    {
                        this.newState = new StopReqStopApprCycleState();
                        break;
                    }
            }
        }

        private void RaiseXimpleCreated(XimpleEventArgs e)
        {
            var handler = this.XimpleCreated;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
