// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonIbisChannel.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Channels
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Motion.Common.Mgi.AtmelControl;
    using Gorba.Motion.Protran.Ibis.Remote;

    /// <summary>
    /// IBIS channel that manages communication over JSON interface
    /// </summary>
    public partial class JsonIbisChannel : IbisChannel, IManageableObject
    {
        private readonly JsonConfig jsonConfig;

        private readonly int ibisAddress;

        private readonly byte[] fakeStatusRequest;

        private AtmelControlClient jsonClient;

        private bool isIbisAddressSet;

        private int currentReplyValue = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonIbisChannel"/> class.
        /// </summary>
        /// <param name="configContext">
        /// The config context.
        /// </param>
        public JsonIbisChannel(IIbisConfigContext configContext)
            : base(configContext)
        {
            var config = configContext.Config;
            this.ibisAddress = config.Behaviour.IbisAddresses[0];
            this.jsonConfig = config.Sources.Json;
            this.jsonClient = new AtmelControlClient();

            this.RemoteComputer = new IbisMaster(this.Config.Behaviour.ConnectionTimeOut);

            if (!config.Behaviour.CheckCrc)
            {
                this.Logger.Warn(
                    "The CheckCrc tag in ibis.xml was configured incorrectly, JSON always verifies the CRC");
            }

            if (config.Behaviour.ByteType != ByteType.Ascii7)
            {
                config.Behaviour.ByteType = ByteType.Ascii7;
                this.Logger.Warn("The byte type for JSON was configured incorrectly. Must be set to Ascii7");
            }

            if (config.Behaviour.IbisAddresses.Count > 1)
            {
                this.Logger.Warn("JSON only supports one IBIS address, using {0}", this.ibisAddress);
            }

            this.fakeStatusRequest = Encoding.ASCII.GetBytes(string.Format("a{0}\r0", (char)('0' + this.ibisAddress)));
            this.Parser.UpdateChecksum(this.fakeStatusRequest);
        }

        /// <summary>
        /// Gets or sets the current channel's status to send
        /// within an answer. The default value is "Ok".
        /// </summary>
        public override Status CurrentStatus
        {
            get
            {
                return base.CurrentStatus;
            }

            set
            {
                if (base.CurrentStatus == value)
                {
                    return;
                }

                this.Logger.Debug("Current status has changed to {0}", value);
                base.CurrentStatus = value;
                this.UpdateReplyValue();
            }
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield break;
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            yield return new ManagementProperty<bool>("JSON Channel Connected", this.jsonClient.Connected, true);
            yield return new ManagementProperty<int>("IBIS address", this.ibisAddress, true);
        }

        /// <summary>
        /// Opens the channel and the JSON port
        /// </summary>
        protected override void DoOpen()
        {
            this.jsonClient.ConnectedChanged += this.OnJsonConnectionChanged;
            this.jsonClient.Connect(this.jsonConfig.IpAddress, this.jsonConfig.Port);
            base.DoOpen();
        }

        /// <summary>
        /// Closes the channel and the JSON port
        /// </summary>
        protected override void DoClose()
        {
            if (this.jsonClient != null)
            {
                this.jsonClient.Dispose();
                this.jsonClient = null;
            }

            base.DoClose();
            this.Logger.Info("JSON IBIS channel closed");
        }

        /// <summary>
        /// This method is not implemented in this channel
        /// since answers are created automatically by Atmel Control.
        /// </summary>
        /// <param name="bytes">
        /// The buffer to send.
        /// </param>
        /// <param name="offset">
        /// The offset inside the buffer.
        /// </param>
        /// <param name="length">
        /// The number of bytes to send starting from <see cref="offset"/>.
        /// </param>
        protected override void SendAnswer(byte[] bytes, int offset, int length)
        {
            if (length != 4 || bytes[offset] != 'a')
            {
                return;
            }

            var answer = bytes[offset + 1] - '0';

            if (!this.jsonClient.Connected)
            {
                this.Logger.Debug("Can't set IBIS reply value ({0}) because we are not connected to JSON", answer);
                return;
            }

            if (this.currentReplyValue == answer)
            {
                return;
            }

            this.Logger.Info("Setting IBIS reply value to {0}", answer);
            this.currentReplyValue = answer;
            this.jsonClient.SetIbisReplyValue(answer);
        }

        private void UpdateReplyValue()
        {
            // send a fake status request, this will call SendAnswer() above which will the set the value
            // on the JSON client (if necessary)
            this.ManageAnswer(this.fakeStatusRequest, null);
        }

        private void OnJsonConnectionChanged(object sender, EventArgs args)
        {
            if (this.jsonClient.Connected)
            {
                this.Logger.Info("JSON IBIS channel connected");
                this.jsonClient.RegisterObject<InfovisionInputState>(this.ReadInputState);
            }
            else
            {
                this.Logger.Info("JSON IBIS channel disconnected");
            }
        }

        private void ReadInputState(InfovisionInputState state)
        {
            if (!this.isIbisAddressSet)
            {
                if (this.ibisAddress != state.Address)
                {
                    this.Logger.Info("Setting IBIS address to {0}", this.ibisAddress);
                    this.jsonClient.SetIbisAddress(this.ibisAddress);
                }

                if (this.currentReplyValue >= 0)
                {
                    this.Logger.Info("Setting IBIS reply value to {0}", this.currentReplyValue);
                    this.jsonClient.SetIbisReplyValue(this.currentReplyValue);
                }

                this.isIbisAddressSet = true;
            }

            this.jsonClient.RegisterObject<IbisStream>(this.ReadTelegrams);
        }

        private void ReadTelegrams(IbisStream dataStream)
        {
            if (dataStream.Data.Count > 0)
            {
                this.RemoteComputer.HasSentData = true;
            }

            foreach (var data in dataStream.Data)
            {
                var telegram = new byte[data.Length + 2];
                Encoding.ASCII.GetBytes(data, 0, data.Length, telegram, 0);
                telegram[telegram.Length - 2] = 0x0D;
                this.Parser.UpdateChecksum(telegram);

                this.Logger.Trace("Received telegram: {0}", data);

                this.ManageTelegram(telegram);
            }
        }
    }
}
