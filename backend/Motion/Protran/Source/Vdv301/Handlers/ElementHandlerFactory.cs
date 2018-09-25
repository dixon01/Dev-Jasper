// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementHandlerFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ElementHandlerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Vdv301.Handlers
{
    using Gorba.Common.Configuration.Protran.VDV301;

    /// <summary>
    /// Standard implementation of <see cref="IElementHandlerFactory"/>.
    /// </summary>
    public class ElementHandlerFactory : IElementHandlerFactory
    {
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
            return new ElementHandler(config, context);
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
            return new ArrayElementHandler(config, context);
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
            return new TranslatedElementHandler(config, context);
        }
    }
}
