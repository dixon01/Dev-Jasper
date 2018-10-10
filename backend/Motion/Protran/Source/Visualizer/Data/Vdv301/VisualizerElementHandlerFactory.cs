// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualizerElementHandlerFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VisualizerElementHandlerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Data.Vdv301
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Protran.VDV301;
    using Gorba.Motion.Protran.Core.Transformations;
    using Gorba.Motion.Protran.Vdv301.Handlers;

    /// <summary>
    /// Implementation of <see cref="IElementHandlerFactory"/> for visualizer.
    /// </summary>
    internal class VisualizerElementHandlerFactory : IElementHandlerFactory
    {
        private readonly List<TransformationInfo> transformationInfos = new List<TransformationInfo>();

        /// <summary>
        /// Event that is fired whenever one of the created element handlers transforms an element.
        /// </summary>
        public event EventHandler<TransformationChainEventArgs> ElementTransformed;

        /// <summary>
        /// Creates a standard element handler.
        /// </summary>
        /// <param name="config">
        /// The data item configuration.
        /// </param>
        /// <param name="context">
        /// The handler configuration context.
        /// </param>
        /// <returns>
        /// The newly created <see cref="ElementHandler"/>.
        /// </returns>
        public ElementHandler CreateElementHandler(DataItemConfig config, IHandlerConfigContext context)
        {
            return new VisualizerElementHandler(config, context, this);
        }

        /// <summary>
        /// Creates an array element handler.
        /// </summary>
        /// <param name="config">
        /// The data item configuration.
        /// </param>
        /// <param name="context">
        /// The handler configuration context.
        /// </param>
        /// <returns>
        /// The newly created <see cref="ArrayElementHandler"/>.
        /// </returns>
        public ArrayElementHandler CreateArrayElementHandler(DataItemConfig config, IHandlerConfigContext context)
        {
            return new VisualizerArrayElementHandler(config, context, this);
        }

        /// <summary>
        /// Creates an element handler for translated texts.
        /// </summary>
        /// <param name="config">
        /// The data item configuration.
        /// </param>
        /// <param name="context">
        /// The handler configuration context.
        /// </param>
        /// <returns>
        /// The newly created <see cref="TranslatedElementHandler"/>.
        /// </returns>
        public TranslatedElementHandler CreateTranslatedElementHandler(
            DataItemConfig config, IHandlerConfigContext context)
        {
            return new VisualizerTranslatedElementHandler(config, context, this);
        }

        /// <summary>
        /// Raises the <see cref="ElementTransformed"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseTelegramTransformed(TransformationChainEventArgs e)
        {
            var handler = this.ElementTransformed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void PrepareTransformationChain<T>(TransformationChain<T> transformationChain)
        {
            var sink = (ITransformer)transformationChain.First;
            ITransformationLogger logger = null;
            while (sink != null)
            {
                var loggerType = typeof(TransformationLogger<>).MakeGenericType(sink.OutputType);
                logger = (ITransformationLogger)Activator.CreateInstance(loggerType);
                if (sink == transformationChain.First)
                {
                    logger.Transformed += (s, e) => this.transformationInfos.Clear();
                }

                logger.Transformed += (s, e) => this.transformationInfos.Add(e.Info);
                logger.Next = sink.Next;
                sink.Next = logger;
                sink = logger.Next as ITransformer;
            }

            if (logger != null)
            {
                logger.Transformed +=
                    (s, e) =>
                    this.RaiseTelegramTransformed(
                        new TransformationChainEventArgs(transformationChain.Name, this.transformationInfos.ToArray()));
            }
        }

        private class VisualizerElementHandler : ElementHandler
        {
            public VisualizerElementHandler(
                DataItemConfig config, IHandlerConfigContext context, VisualizerElementHandlerFactory owner)
                : base(config, context)
            {
                owner.PrepareTransformationChain(this.TransformationChain);
            }
        }

        private class VisualizerArrayElementHandler : ArrayElementHandler
        {
            public VisualizerArrayElementHandler(
                DataItemConfig config, IHandlerConfigContext context, VisualizerElementHandlerFactory owner)
                : base(config, context)
            {
                owner.PrepareTransformationChain(this.TransformationChain);
            }
        }

        private class VisualizerTranslatedElementHandler : TranslatedElementHandler
        {
            public VisualizerTranslatedElementHandler(
                DataItemConfig config, IHandlerConfigContext context, VisualizerElementHandlerFactory owner)
                : base(config, context)
            {
                owner.PrepareTransformationChain(this.TransformationChain);
            }
        }
    }
}