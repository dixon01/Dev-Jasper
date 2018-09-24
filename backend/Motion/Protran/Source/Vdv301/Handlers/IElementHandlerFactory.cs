// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IElementHandlerFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IElementHandlerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Vdv301.Handlers
{
    using Gorba.Common.Configuration.Protran.VDV301;

    /// <summary>
    /// Interface for a factory for *ElementHandler objects.
    /// </summary>
    public interface IElementHandlerFactory
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
        ElementHandler CreateElementHandler(DataItemConfig config, IHandlerConfigContext context);

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
        ArrayElementHandler CreateArrayElementHandler(DataItemConfig config, IHandlerConfigContext context);

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
        TranslatedElementHandler CreateTranslatedElementHandler(DataItemConfig config, IHandlerConfigContext context);
    }
}