// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisChannel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisChannel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Channels
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.Core.Buffers;
    using Gorba.Motion.Protran.Ibis.Parsers;
    using Gorba.Motion.Protran.Ibis.Recording;
    using Gorba.Motion.Protran.Ibis.Remote;
    using Gorba.Motion.Protran.Ibis.Transformations;

    /// <summary>
    /// Base class for IBIS channels that read and write from
    /// a real IBIS master.
    /// </summary>
    public abstract class IbisChannel : Channel
    {
        /// <summary>
        /// Queue tasked to store the telegrams received from the remote computer
        /// in a FIFO fashion.
        /// </summary>
        private readonly ProducerConsumerQueue<TelegramInfo> readQueue;

        /// <summary>
        /// Dictionary of all transformations accessed with their ID.
        /// </summary>
        private readonly Dictionary<string, TransformationChain> transformationChains;

        /// <summary>
        /// Instance of the class tasked to record all the
        /// telegram received from the IBIS master.
        /// </summary>
        private IRecorder recorder;

        /// <summary>
        /// Variable that tells how many answer are been sent to
        /// the IBIS master.
        /// </summary>
        private int answersSentCounter;

        private int telegramsCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="IbisChannel"/> class.
        /// </summary>
        /// <param name="configContext">
        /// The config context.
        /// </param>
        protected IbisChannel(IIbisConfigContext configContext)
            : base(configContext)
        {
            this.readQueue = new ProducerConsumerQueue<TelegramInfo>(t => this.HandleData(t.Telegram, t.Parser), 100);

            this.transformationChains = new Dictionary<string, TransformationChain>();

            var config = configContext.Config;
            foreach (var chain in config.Transformations)
            {
                this.transformationChains.Add(
                    chain.Id,
                    new TransformationChain(chain, config.Behaviour.ByteType, config.Transformations));
            }
        }

        /// <summary>
        /// Gets or sets the Recorder for this channel.
        /// </summary>
        public IRecorder Recorder
        {
            get
            {
                return this.recorder;
            }

            set
            {
                if (this.recorder == value)
                {
                    return;
                }

                if (this.recorder != null)
                {
                    // I ensure that the old
                    // is stopped, before to set it
                    // with the new one.
                    this.recorder.Stop();
                }

                // now I can really set my recorder.
                this.recorder = value;
            }
        }

        /// <summary>
        /// Gets or sets the instance of the remote computer
        /// (if any) associated to this channel.
        /// </summary>
        public RemoteComputer RemoteComputer { get; protected set; }

        /// <summary>
        /// Gets or sets the current channel's status to send
        /// within an answer. The default value is "Ok".
        /// </summary>
        public virtual Status CurrentStatus { get; set; }

        /// <summary>
        /// Opens this channel and start the recorder if it was set.
        /// </summary>
        protected override void DoOpen()
        {
            this.readQueue.StartConsumer();

            try
            {
                if (this.recorder != null)
                {
                    this.recorder.Start();
                }
            }
            catch (Exception ex)
            {
                // an error was occured on starting the record.
                this.Close();
                this.Logger.Error(ex,"Error on starting the IBIS recorder");
            }
        }

        /// <summary>
        /// Closes this channel and stops the recorder if it was set.
        /// </summary>
        protected override void DoClose()
        {
            this.readQueue.StopConsumer();

            if (this.recorder != null)
            {
                this.recorder.Stop();
                this.recorder = null;
            }
        }

        /// <summary>
        /// Subclasses implement this method to send an answer to the IBIS master.
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
        protected abstract void SendAnswer(byte[] bytes, int offset, int length);

        /// <summary>
        /// Handles received data on the parser thread.
        /// </summary>
        /// <param name="data">
        ///   The telegram data.
        /// </param>
        /// <param name="parser">
        ///   The handler responsible for this type of telegram.
        /// </param>
        /// <returns>
        /// True if the handle has succeeded, else false.
        /// </returns>
        protected virtual bool HandleData(byte[] data, ITelegramParser parser)
        {
            // now, if the recording feauture is enabled
            // I record the telegram just recognized.
            if (this.recorder != null)
            {
                this.recorder.Write(data);
            }

            if (parser == null)
            {
                // ignore, this is just used for recording the telegram
                return false;
            }

            this.Logger.Trace(() => string.Format("Received data: {0}", BufferUtils.FromByteArrayToHexString(data)));

            Telegram telegram = null;
            try
            {
                telegram = parser.Parse(data);
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, 
                    string.Format(
                        "Exception while parsing telegram with {0}: {1}",
                        parser.GetType().Name,
                        BitConverter.ToString(data)),
                    ex);
            }

            if (telegram == null)
            {
                // invalid telegram.
                // I cannot do nothing with it.
                this.Logger.Info("Invalid telegram extracted from the Channel's queue.");
                return false;
            }

            this.Logger.Trace("Parsed telegram: {0}", parser.Config.Name);

            try
            {
                this.Transform(telegram, parser.Config.TransfRef);
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Could not transform telegram: " + ex.Message, ex);
                return false;
            }

            // ok, the Telegram was created with success.
            // it's the time to fire in event in order to notify
            // that some info were received from the remote computer.
            this.RaiseTelegramReceived(new TelegramReceivedEventArgs(telegram, parser.Config));
            return true;
        }

        /// <summary>
        /// Checks the telegram's CRC (if enabled) and then searches a
        /// telegram parser that matches the given telegram. If that
        /// parser is found it is queried to check if the telegram is
        /// for the local address.
        /// </summary>
        /// <param name="telegram">
        /// The telegram.
        /// </param>
        /// <returns>
        /// The parser for the telegram if all the mentioned checks
        /// are fulfilled.
        /// </returns>
        protected ITelegramParser GetTelegramParser(byte[] telegram)
        {
            if (!this.Parser.IsChecksumCorrect(telegram))
            {
                // the CRC is wrong. I discard this telegram is corrupted.
                this.Logger.Warn("Invalid CRC in telegram: " + BufferUtils.FromByteArrayToHexString(telegram));
                return null;
            }

            var parser = this.Parser.GetTelegramParser(telegram);
            if (parser == null)
            {
                this.Logger.Info("Telegram skipped (no configuration enabled).");
                return null;
            }

            foreach (var ibisAddress in this.Config.Behaviour.IbisAddresses)
            {
                if (parser.IsForAddress(telegram, ibisAddress))
                {
                    return parser;
                }
            }

            // the telegram just received is not for me.
            // it was sent from the IBIS master to an IBIS slave
            // with an IBIS address different from the mine.
            this.Logger.Info("Telegram skipped (not our IBIS address).");
            return null;
        }

        /// <summary>
        /// Transforms the telegram's payload to its data field
        /// using the transformation chain referenced from the
        /// telegram's configuration.
        /// </summary>
        /// <param name="telegram">the telegram to be transformed</param>
        /// <param name="transfRef">the name of the transformation</param>
        protected virtual void Transform(Telegram telegram, string transfRef)
        {
            if (telegram is EmptyTelegram)
            {
                // empty telegrams don't need a transformation
                return;
            }

            var chain = this.GetTransformationChain(transfRef);
            if (chain == null)
            {
                return;
            }

            chain.Transform(telegram);
            this.Logger.Trace("Transformed telegram: {0}", telegram.GetType().Name);
        }

        /// <summary>
        /// Gets the transformation chain for a given telegram.
        /// </summary>
        /// <param name="transfRef">
        /// The name of the transformation.
        /// </param>
        /// <returns>
        /// the transformation chain or null if no chain was found.
        /// </returns>
        protected virtual TransformationChain GetTransformationChain(string transfRef)
        {
            TransformationChain chain;
            if (transfRef == null || !this.transformationChains.TryGetValue(transfRef, out chain))
            {
                // todo: what is, if we have no transformation referenced?
                // The data property will not be set and thus the telegram is not usable.
                this.Logger.Warn(
                    "Transformation chain not found: {0}", transfRef);
                return null;
            }

            return chain;
        }

        /// <summary>
        /// Enqueues a telegram inside the queue
        /// (the max size is 100 telegrams).
        /// </summary>
        /// <param name="telegram">The telegram to be enqueued.</param>
        /// <param name="parser">The handler responsible for this type of telegram.</param>
        /// <exception cref="ArgumentException">If the telegram to be enqueued is null.</exception>
        /// <exception cref="Exception">If the queue is full.</exception>
        protected void Enqueue(byte[] telegram, ITelegramParser parser)
        {
            this.readQueue.Enqueue(new TelegramInfo(telegram, parser));
        }

        /// <summary>
        /// Deals with all the steps required whenever a valid IBIS telegram is received:
        /// - parse it
        /// - create an eventual response
        /// - send the eventual response
        /// - sends the telegram to the telegram's producer-consumer queue.
        /// </summary>
        /// <param name="telegram">
        /// The telegram to be parsed.
        /// </param>
        protected void ManageTelegram(byte[] telegram)
        {
            var parser = this.GetTelegramParser(telegram);
            if (parser == null)
            {
                // invalid parser.
                // I can't do nothing with it.
                return;
            }

            this.ManageAnswer(telegram, parser);

            try
            {
                // ok, the IBIS telegram is complete.
                // I can store it into the queue. Somebodyelse will study it.
                this.Enqueue(telegram, parser);
                if (this.Logger.IsTraceEnabled)
                {
                    this.Logger.Trace("IBIS telegram enqueued ({0})", ++this.telegramsCounter);
                }
            }
            catch (Exception ex)
            {
                // an error was occured enqueuing the telegram
                // (probably the queue is full).
                this.Logger.Error(ex,"Error on enqueuing a telegram");
            }
        }

        /// <summary>
        /// Deals with all the steps required to send an answer to the IBIS master
        /// </summary>
        /// <param name="telegram">
        /// The telegram from which understand the answer.
        /// </param>
        /// <param name="parser">
        /// The parser to be used to compose the answer. If null,
        /// the parser will be retrieved analyzing the telegram itself.
        /// </param>
        protected void ManageAnswer(byte[] telegram, ITelegramParser parser)
        {
            parser = parser ?? this.GetTelegramParser(telegram);
            if (parser == null || !parser.RequiresAnswer(telegram))
            {
                // invalid parser or answer not required.
                return;
            }

            try
            {
                // the telegram just received requires an answer.
                // without losing any time, I send it to the
                // IBIS master right now.
                byte[] answer = parser.CreateAnswer(telegram, this.CurrentStatus);
                this.Parser.UpdateChecksum(answer);

                this.SendAnswer(answer, 0, answer.Length);
                this.Logger.Info("Answer sent to the IBIS master: {0}", ++this.answersSentCounter);
                this.Logger.Trace(() => BufferUtils.FromByteArrayToHexString(answer));
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex,"Could not send answer telegram");
            }
        }

        /// <summary>
        /// Just a container of the couple {telegram;parser}
        /// </summary>
        private class TelegramInfo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TelegramInfo"/> class.
            /// </summary>
            /// <param name="telegram">
            /// The original telegram received from the remote computer.
            /// </param>
            /// <param name="parser">
            /// The parser tasked to parse the telegram.
            /// </param>
            public TelegramInfo(byte[] telegram, ITelegramParser parser)
            {
                this.Telegram = telegram;
                this.Parser = parser;
            }

            /// <summary>
            /// Gets the telegram received from the remote computer.
            /// </summary>
            public byte[] Telegram { get; private set; }

            /// <summary>
            /// Gets the parser tasked to parse the telegram.
            /// </summary>
            public ITelegramParser Parser { get; private set; }
        }
    }
}
