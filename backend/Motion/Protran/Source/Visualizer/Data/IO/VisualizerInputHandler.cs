// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualizerInputHandler.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Data.IO
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Protran.IO;
    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Motion.Protran.Core.Transformations;
    using Gorba.Motion.Protran.Core.Utils;
    using Gorba.Motion.Protran.IO.Inputs;

    /// <summary>
    /// Handler for input from the IO form
    /// </summary>
    public class VisualizerInputHandler : IInputHandler
    {
        private readonly InputHandlingConfig config;

        private readonly GenericUsageHandler usage;

        private readonly List<TransformationInfo> transformationInfos = new List<TransformationInfo>();

        private TransformationChain chain;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualizerInputHandler"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="chain">
        /// The chain.
        /// </param>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        public VisualizerInputHandler(InputHandlingConfig config, TransformationChain chain, Dictionary dictionary)
        {
            this.config = config;
            this.chain = chain;
            this.usage = new GenericUsageHandler(this.config.UsedFor, dictionary);

            var sink = (ITransformer)this.chain.First;
            while (sink != null)
            {
                var loggerType = typeof(TransformationLogger<>).MakeGenericType(sink.OutputType);
                var logger = (ITransformationLogger)Activator.CreateInstance(loggerType);
                logger.Transformed += (s, e) => this.transformationInfos.Add(e.Info);
                logger.Next = sink.Next;
                sink.Next = logger;
                sink = logger.Next as ITransformer;
            }
        }

        /// <summary>
        /// The started.
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Event that is fired if this handler created some data.
        /// </summary>
        public event EventHandler<XimpleEventArgs> XimpleCreated;

        /// <summary>
        /// The io input transforming.
        /// </summary>
        public event EventHandler IoInputTransforming;

        /// <summary>
        /// Event that is fired when a telegram successfully passed transformation.
        /// </summary>
        public event EventHandler<TransformationChainEventArgs> IoInputTransformed;

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get
            {
                return this.config.Name;
            }
        }

        /// <summary>
        /// Gets or sets the chain id.
        /// </summary>
        public string ChainId { get; set; }

        /// <summary>
        /// The start.
        /// </summary>
        public void Start()
        {
            var handler = this.Started;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
        }

        /// <summary>
        /// The pin state changed.
        /// </summary>
        /// <param name="pinState">
        /// The pin state.
        /// </param>
        public void PinStateChanged(bool pinState)
        {
            var ximple = new Ximple();
            this.RaiseIoInputTransforming(EventArgs.Empty);

            this.transformationInfos.Clear();
            var value = this.chain.Transform(this.ConvertToIoValue(pinState));
            this.RaiseIoInputTransformed(new TransformationChainEventArgs(
                this.ChainId, this.transformationInfos.ToArray()));

            this.usage.AddCell(ximple, value);
            this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
        }

        /// <summary>
        /// Raises the <see cref="XimpleCreated"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseXimpleCreated(XimpleEventArgs e)
        {
            var handler = this.XimpleCreated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private IOValue ConvertToIoValue(bool pinState)
        {
            return pinState ? FlagValues.True : FlagValues.False;
        }

        private void RaiseIoInputTransforming(EventArgs args)
        {
            var handler = this.IoInputTransforming;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private void RaiseIoInputTransformed(TransformationChainEventArgs args)
        {
            var handler = this.IoInputTransformed;
            if (handler != null)
            {
                handler(this, args);
            }
        }
    }
}
