// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementHandler.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ElementHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Vdv301.Handlers
{
    using System;
    using System.Globalization;

    using Gorba.Common.Configuration.Protran.VDV301;
    using Gorba.Common.Protocols.Vdv301.Messages;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Motion.Common.IbisIP;
    using Gorba.Motion.Protran.Core.Utils;

    /// <summary>
    /// Element handler that handles a single data input and puts it into a single Ximple cell.
    /// </summary>
    public class ElementHandler
    {
        private readonly GenericUsageHandler genericUsageHandler;

        private readonly DateTimeDataItemConfig dateTimeConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementHandler"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="context">
        /// The handler config context.
        /// </param>
        public ElementHandler(DataItemConfig config, IHandlerConfigContext context)
        {
            this.dateTimeConfig = config as DateTimeDataItemConfig;
            this.genericUsageHandler = new GenericUsageHandler(config, context.Dictionary);
            this.TransformationChain = new TransformationChain<string>(config.TransfRef, context);
        }

        /// <summary>
        /// Gets or sets the transformation chain used by this handler.
        /// </summary>
        protected TransformationChain<string> TransformationChain { get; set; }

        /// <summary>
        /// Handles the given <paramref name="value"/> by first translating it and
        /// then puts it into the correct generic cell of the provided <paramref name="ximple"/>.
        /// </summary>
        /// <param name="value">
        /// The value.
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
        /// If the value has an error associated to it.
        /// </exception>
        public void Handle(IBISIPstring value, string name, Ximple ximple, int rowIndex)
        {
            if (value.ErrorCodeSpecified)
            {
                throw new IbisIPException(name + " is " + value.ErrorCode);
            }

            this.Handle(value.Value, ximple, rowIndex);
        }

        /// <summary>
        /// Handles the given <paramref name="value"/> by first translating it and
        /// then puts it into the correct generic cell of the provided <paramref name="ximple"/>.
        /// </summary>
        /// <param name="value">
        /// The value.
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
        /// If the value has an error associated to it.
        /// </exception>
        public void Handle(IBISIPNMTOKEN value, string name, Ximple ximple, int rowIndex)
        {
            if (value.ErrorCodeSpecified)
            {
                throw new IbisIPException(name + " is " + value.ErrorCode);
            }

            this.Handle(value.Value, ximple, rowIndex);
        }

        /// <summary>
        /// Handles the given <paramref name="value"/> by first translating it and
        /// then puts it into the correct generic cell of the provided <paramref name="ximple"/>.
        /// </summary>
        /// <param name="value">
        /// The value.
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
        /// If the value has an error associated to it.
        /// </exception>
        public void Handle(IBISIPlanguage value, string name, Ximple ximple, int rowIndex)
        {
            if (value.ErrorCodeSpecified)
            {
                throw new IbisIPException(name + " is " + value.ErrorCode);
            }

            this.Handle(value.Value, ximple, rowIndex);
        }

        /// <summary>
        /// Handles the given <paramref name="value"/> by first translating it and
        /// then puts it into the correct generic cell of the provided <paramref name="ximple"/>.
        /// </summary>
        /// <param name="value">
        /// The value.
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
        /// If the value has an error associated to it.
        /// </exception>
        public void Handle(IBISIPint value, string name, Ximple ximple, int rowIndex)
        {
            if (value.ErrorCodeSpecified)
            {
                throw new IbisIPException(name + " is " + value.ErrorCode);
            }

            this.Handle(value.Value.ToString(CultureInfo.InvariantCulture), ximple, rowIndex);
        }

        /// <summary>
        /// Handles the given <paramref name="value"/> by first translating it and
        /// then puts it into the correct generic cell of the provided <paramref name="ximple"/>.
        /// </summary>
        /// <param name="value">
        /// The value.
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
        /// If the value has an error associated to it.
        /// </exception>
        public void Handle(IBISIPnonNegativeInteger value, string name, Ximple ximple, int rowIndex)
        {
            if (value.ErrorCodeSpecified)
            {
                throw new IbisIPException(name + " is " + value.ErrorCode);
            }

            this.Handle(value.Value.ToString(CultureInfo.InvariantCulture), ximple, rowIndex);
        }

        /// <summary>
        /// Handles the given <paramref name="value"/> by first translating it and
        /// then puts it into the correct generic cell of the provided <paramref name="ximple"/>.
        /// </summary>
        /// <param name="value">
        /// The value.
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
        /// If the value has an error associated to it.
        /// </exception>
        public void Handle(IBISIPdouble value, string name, Ximple ximple, int rowIndex)
        {
            if (value.ErrorCodeSpecified)
            {
                throw new IbisIPException(name + " is " + value.ErrorCode);
            }

            this.Handle(value.Value.ToString(CultureInfo.InvariantCulture), ximple, rowIndex);
        }

        /// <summary>
        /// Handles the given <paramref name="value"/> by first translating it and
        /// then puts it into the correct generic cell of the provided <paramref name="ximple"/>.
        /// </summary>
        /// <param name="value">
        /// The value.
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
        /// If the value has an error associated to it.
        /// </exception>
        public void Handle(IBISIPboolean value, string name, Ximple ximple, int rowIndex)
        {
            if (value.ErrorCodeSpecified)
            {
                throw new IbisIPException(name + " is " + value.ErrorCode);
            }

            this.Handle(value.Value.ToString(CultureInfo.InvariantCulture), ximple, rowIndex);
        }

        /// <summary>
        /// Handles the given enum <paramref name="value"/> by first translating it and
        /// then puts it into the correct generic cell of the provided <paramref name="ximple"/>.
        /// </summary>
        /// <param name="value">
        /// The value.
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
        public void Handle(Enum value, string name, Ximple ximple, int rowIndex)
        {
            this.Handle(value.ToString(), ximple, rowIndex);
        }

        /// <summary>
        /// Handles the given <paramref name="value"/> by first translating it and
        /// then puts it into the correct generic cell of the provided <paramref name="ximple"/>.
        /// </summary>
        /// <param name="value">
        /// The value.
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
        /// If the value has an error associated to it.
        /// </exception>
        public void Handle(IBISIPduration value, string name, Ximple ximple, int rowIndex)
        {
            if (value.ErrorCodeSpecified)
            {
                throw new IbisIPException(name + " is " + value.ErrorCode);
            }

            this.Handle(value.Value, ximple, rowIndex);
        }

        /// <summary>
        /// Handles the given <paramref name="value"/> by first translating it and
        /// then puts it into the correct generic cell of the provided <paramref name="ximple"/>.
        /// </summary>
        /// <param name="value">
        /// The value.
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
        /// If the value has an error associated to it.
        /// </exception>
        public void Handle(IBISIPdateTime value, string name, Ximple ximple, int rowIndex)
        {
            if (value.ErrorCodeSpecified)
            {
                throw new IbisIPException(name + " is " + value.ErrorCode);
            }

            this.Handle(value.Value.ToString(this.dateTimeConfig.DateTimeFormat), ximple, rowIndex);
        }

        private void Handle(string value, Ximple ximple, int rowIndex)
        {
            var transformed = this.TransformationChain.Transform(value);
            this.genericUsageHandler.AddCell(ximple, transformed, rowIndex);
        }
    }
}
