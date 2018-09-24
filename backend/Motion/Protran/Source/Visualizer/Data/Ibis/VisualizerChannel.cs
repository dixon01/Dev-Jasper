// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualizerChannel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VisualizerChannel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Data.Ibis
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Protran.Transformations;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Motion.Protran.Core.Buffers;
    using Gorba.Motion.Protran.Core.Transformations;
    using Gorba.Motion.Protran.Ibis;
    using Gorba.Motion.Protran.Ibis.Channels;
    using Gorba.Motion.Protran.Ibis.Parsers;
    using Gorba.Motion.Protran.Ibis.Transformations;

    /// <summary>
    /// Special channel that intercepts events and allows them to be
    /// reported in the visualizer using some events.
    /// </summary>
    public class VisualizerChannel : IbisChannel
    {
        private readonly List<TransformationChain> enhancedChains = new List<TransformationChain>();
        private readonly List<TransformationInfo> transformationInfos = new List<TransformationInfo>();

        private readonly Dictionary<string, TransformationChain> answerChains =
            new Dictionary<string, TransformationChain>();

        private readonly TelegramParserFactory answerParserFactory;

        private readonly ByteInfo byteInfo;

        private Telegram currentAnswer;
        private byte[] currentData;
        private ITelegramParser currentParser;
        private TransformationChain currentChain;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualizerChannel"/> class.
        /// </summary>
        /// <param name="configContext">
        /// The config context.
        /// </param>
        public VisualizerChannel(IIbisConfigContext configContext)
            : base(configContext)
        {
            this.answerChains.Add(
                "DS120",
                new TransformationChain(
                    new Chain { Transformations = { new Integer() } }, configContext.Config.Behaviour.ByteType, null));
            this.answerChains.Add(
                "DS130",
                new TransformationChain(
                    new Chain { Transformations = { new Integer() } }, configContext.Config.Behaviour.ByteType, null));

            this.RemoteComputer = new VisualizerRemoteComputer();
            this.answerParserFactory = new TelegramParserFactory();

            this.byteInfo = ByteInfo.For(configContext.Config.Behaviour.ByteType);
        }

        /// <summary>
        /// Event that is fired when this channel is opened.
        /// </summary>
        public event EventHandler Opened;

        /// <summary>
        /// Event that is fired every time a new telegram is being enqueued.
        /// </summary>
        /// <seealso cref="EnqueueTelegram"/>
        public event EventHandler TelegramParsing;

        /// <summary>
        /// Event that is fired when a telegram was successfully parsed,
        /// i.e. a <see cref="Telegram"/> was created from the binary data.
        /// </summary>
        public event EventHandler<TelegramParsedEventArgs> TelegramParsed;

        /// <summary>
        /// Event that is fired when a telegram successfully passed transformation.
        /// </summary>
        public event EventHandler<TransformationChainEventArgs> TelegramTransformed;

        /// <summary>
        /// Enqueue a new telegram into this channel.
        /// </summary>
        /// <param name="telegram">
        /// The telegram data.
        /// </param>
        /// <param name="ignoreUnknown">
        /// If set to true, unknown telegrams (where no parser is found) will not be enqueued
        /// and false will be returned.
        /// </param>
        /// <returns>
        /// true if the telegram was enqueued successfully.
        /// </returns>
        public bool EnqueueTelegram(byte[] telegram, bool ignoreUnknown)
        {
            var parser = this.GetTelegramParser(telegram);
            if (ignoreUnknown && parser == null)
            {
                return false;
            }

            this.Enqueue(telegram, parser);
            return true;
        }

        /// <summary>
        /// Opens this channel and start the recorder if it was set.
        /// </summary>
        protected override void DoOpen()
        {
            base.DoOpen();

            var handler = this.Opened;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
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
        protected override void SendAnswer(byte[] bytes, int offset, int length)
        {
        }

        /// <summary>
        /// Handles received data on the parser thread.
        /// </summary>
        /// <param name="data">
        /// The telegram data.
        /// </param>
        /// <param name="parser">
        /// The handler responsible for this type of telegram.
        /// </param>
        /// <returns>
        /// True if the handle has succeeded, else false.
        /// </returns>
        protected override bool HandleData(byte[] data, ITelegramParser parser)
        {
            this.RaiseTelegramParsing(EventArgs.Empty);

            if (parser != null && parser.RequiresAnswer(data))
            {
                // the telegram just received requires an answer
                this.currentAnswer = this.CreateAnswer(data, parser);
            }
            else
            {
                this.currentAnswer = null;
            }

            this.currentData = data;
            this.currentParser = parser;
            return base.HandleData(data, parser);
        }

        /// <summary>
        /// Gets the transformation chain for a given telegram.
        /// </summary>
        /// <param name="telegram">
        ///     The telegram.
        /// </param>
        /// <returns>
        /// the transformation chain or null if no chain was found.
        /// </returns>
        protected override TransformationChain GetTransformationChain(string telegram)
        {
            this.currentChain = base.GetTransformationChain(telegram);
            if (this.currentChain == null || this.enhancedChains.Contains(this.currentChain))
            {
                return this.currentChain;
            }

            ITransformer sink = this.currentChain.First;
            while (sink != null)
            {
                var loggerType = typeof(TransformationLogger<>).MakeGenericType(sink.OutputType);
                var logger = (ITransformationLogger)Activator.CreateInstance(loggerType);
                logger.Transformed += (s, e) => this.transformationInfos.Add(e.Info);
                logger.Next = sink.Next;
                sink.Next = logger;
                sink = logger.Next as ITransformer;
            }

            this.enhancedChains.Add(this.currentChain);
            return this.currentChain;
        }

        /// <summary>
        /// Transforms the telegram's payload to its data field
        /// using the transformation chain referenced from the
        /// telegram's configuration.
        /// </summary>
        /// <param name="telegram">the telegram to be transformed</param>
        /// <param name="transfRef">the name of the transformation</param>
        protected override void Transform(Telegram telegram, string transfRef)
        {
            this.RaiseTelegramParsed(new TelegramParsedEventArgs(
                this.currentData, this.currentParser, telegram, this.currentAnswer));

            this.transformationInfos.Clear();
            base.Transform(telegram, transfRef);

            if (this.transformationInfos.Count > 0)
            {
                this.RaiseTelegramTransformed(
                    new TransformationChainEventArgs(this.currentChain.Config.Id, this.transformationInfos.ToArray()));
            }
        }

        private Telegram CreateAnswer(byte[] data, ITelegramParser parser)
        {
            byte[] answer = parser.CreateAnswer(data, this.CurrentStatus);
            this.Parser.UpdateChecksum(answer);
            this.Logger.Trace(() => string.Format("Created answer: {0}", BufferUtils.FromByteArrayToHexString(answer)));

            var answerParser = this.answerParserFactory.CreateParser(this.byteInfo, parser.Config.Answer.Telegram);
            var telegram = answerParser.Parse(answer);
            TransformationChain chain;
            if (this.answerChains.TryGetValue(parser.Config.Name, out chain))
            {
                chain.Transform(telegram);
            }

            return telegram;
        }

        private void RaiseTelegramParsing(EventArgs args)
        {
            var handler = this.TelegramParsing;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private void RaiseTelegramParsed(TelegramParsedEventArgs args)
        {
            var handler = this.TelegramParsed;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private void RaiseTelegramTransformed(TransformationChainEventArgs args)
        {
            var handler = this.TelegramTransformed;
            if (handler != null)
            {
                handler(this, args);
            }
        }
    }
}
