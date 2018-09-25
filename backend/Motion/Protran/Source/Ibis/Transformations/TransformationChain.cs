// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransformationChain.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Transformations
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.Configuration.Protran.Transformations;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Motion.Protran.Core.Transformations;

    /// <summary>
    /// Transformation chain that takes a telegram and converts its payload to the data field
    /// using the configured transformations.
    /// </summary>
    public class TransformationChain
    {
        private readonly TelegramSink sink;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformationChain"/> class.
        /// </summary>
        /// <param name="config">
        /// The configuration of this chain containing all transformation configurations.
        /// </param>
        /// <param name="byteType">
        /// An enum telling if this chain has to handle 7, 8 or 16 bit data from the telegram.
        /// </param>
        /// <param name="allChains">
        /// The list of all chains configured in this application.
        /// </param>
        public TransformationChain(Chain config, ByteType byteType, ICollection<Chain> allChains)
        {
            this.Config = config;
            this.First = new TelegramSource();

            ITransformer encodingTransformer;

            switch (byteType)
            {
                case ByteType.Ascii7:
                    encodingTransformer = new AsciiTransformer();
                    break;
                case ByteType.Hengartner8:
                    encodingTransformer = new HengartnerTransformer();
                    break;
                case ByteType.UnicodeBigEndian:
                    encodingTransformer = new UnicodeTransformer();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("byteType");
            }

            ITransformer current = this.First;

            foreach (var transcoderConfig in config.ResolveReferences(allChains))
            {
                var previous = current;
                current = TransformerFactory.CreateTransformer(transcoderConfig, previous.OutputType);

                if (encodingTransformer != null)
                {
                    if (current.InputType != typeof(byte[]))
                    {
                        // insert the default encoding transformer if we don't have one at the beginning of our chain
                        previous.Next = encodingTransformer;
                        previous = encodingTransformer;
                    }

                    encodingTransformer = null;
                }

                previous.Next = current;
            }

            if (encodingTransformer != null)
            {
                // insert the default encoding transformer
                current.Next = encodingTransformer;
                current = encodingTransformer;
            }

            this.sink = CreateTelegramSink(current.OutputType);
            current.Next = this.sink;
        }

        /// <summary>
        /// Gets the chain configuration.
        /// </summary>
        public Chain Config { get; private set; }

        /// <summary>
        /// Gets the first element of the chain, this just extracts the payload from the telegram.
        /// </summary>
        public TelegramSource First { get; private set; }

        /// <summary>
        /// Transforms the telegram's Payload to its Data
        /// using the configured transformations.
        /// </summary>
        /// <param name="telegram">the telegram to transform.</param>
        public void Transform(Telegram telegram)
        {
            this.sink.Telegram = telegram;
            this.First.Transform(telegram);
        }

        /// <summary>
        /// Creates the final transformation sink from the given output of the second to last sink.
        /// </summary>
        /// <param name="outputType">the telegram.</param>
        /// <returns>a sink able to handle the given telegram.</returns>
        private static TelegramSink CreateTelegramSink(Type outputType)
        {
            if (outputType == typeof(string))
            {
                return new StringTelegramSink();
            }

            if (outputType == typeof(string[]))
            {
                return new StringArrayTelegramSink();
            }

            if (outputType == typeof(int))
            {
                return new IntegerTelegramSink();
            }

            // todo: implement other types of telegrams
            throw new NotSupportedException("Can't handle output type of " + outputType.Name);
        }
    }
}
