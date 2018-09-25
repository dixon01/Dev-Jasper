// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrayElementHandler.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ArrayElementHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Vdv301.Handlers
{
    using Gorba.Common.Configuration.Protran.VDV301;
    using Gorba.Common.Protocols.Vdv301.Messages;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Motion.Common.IbisIP;
    using Gorba.Motion.Protran.Core.Utils;

    /// <summary>
    /// Element handler that handles array data input and puts it into a single Ximple cell.
    /// </summary>
    public class ArrayElementHandler
    {
        private readonly GenericUsageHandler genericUsageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayElementHandler"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="context">
        /// The handler config context.
        /// </param>
        public ArrayElementHandler(DataItemConfig config, IHandlerConfigContext context)
        {
            this.genericUsageHandler = new GenericUsageHandler(config, context.Dictionary);
            this.TransformationChain = new TransformationChain<string[]>(config.TransfRef, context);
        }

        /// <summary>
        /// Gets or sets the transformation chain used by this handler.
        /// </summary>
        protected TransformationChain<string[]> TransformationChain { get; set; }

        /// <summary>
        /// Handles the given <paramref name="values"/> by first translating them and
        /// then puts it into the correct generic cell of the provided <paramref name="ximple"/>.
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
        public void Handle(IBISIPNMTOKEN[] values, string name, Ximple ximple, int rowIndex)
        {
            var stringValue = new string[values.Length];
            for (var i = 0; i < values.Length; i++)
            {
                if (values[i].ErrorCodeSpecified)
                {
                    throw new IbisIPException(name + "[" + i + "] is " + values[i].ErrorCode);
                }

                stringValue[i] = values[i].Value;
            }

            var transformed = this.TransformationChain.Transform(stringValue);
            this.genericUsageHandler.AddCell(ximple, transformed, rowIndex);
        }
    }
}