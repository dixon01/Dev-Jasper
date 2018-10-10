// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TranslatedElementHandler.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TranslatedElementHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Vdv301.Handlers
{
    using Gorba.Common.Configuration.Protran.VDV301;
    using Gorba.Common.Protocols.Vdv301.Messages;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Motion.Common.IbisIP;

    /// <summary>
    /// Element handler that handles an <see cref="InternationalTextType"/> input
    /// and puts it into one or more Ximple cells.
    /// </summary>
    public class TranslatedElementHandler
    {
        private readonly TranslationGenericUsageHandler genericUsageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslatedElementHandler"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="context">
        /// The handler config context.
        /// </param>
        public TranslatedElementHandler(DataItemConfig config, IHandlerConfigContext context)
        {
            this.genericUsageHandler = new TranslationGenericUsageHandler(
                config, context.Dictionary, context.Config.Languages);
            this.TransformationChain = new TransformationChain<string>(config.TransfRef, context);
        }

        /// <summary>
        /// Gets or sets the transformation chain used by this handler.
        /// </summary>
        protected TransformationChain<string> TransformationChain { get; set; }

        /// <summary>
        /// Handles the given <paramref name="values"/> by first translating it and
        /// then puts it into the correct generic cells of the provided <paramref name="ximple"/>.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <param name="name">
        /// The name of the element being handled (only used for error reporting).
        /// </param>
        /// <param name="ximple">
        /// The ximple to fill.
        /// </param>
        /// <param name="rowIndex">
        /// The row index.
        /// </param>
        /// <exception cref="IbisIPException">
        /// If any of the values has an error associated to it.
        /// </exception>
        public void Handle(InternationalTextType[] values, string name, Ximple ximple, int rowIndex)
        {
            var translated = new TranslatedText[values.Length];
            for (var i = 0; i < values.Length; i++)
            {
                if (values[i].ErrorCodeSpecified)
                {
                    throw new IbisIPException(name + "[" + i + "] is " + values[i].ErrorCode);
                }

                translated[i] = new TranslatedText(
                    values[i].Language, this.TransformationChain.Transform(values[i].Value));
            }

            this.genericUsageHandler.AddCells(ximple, translated, rowIndex);
        }
    }
}