// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvaluatorBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EvaluatorBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Evaluation
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Eval;

    /// <summary>
    /// Base class for evaluations.
    /// Its subclasses can query <see cref="IPresentationContext"/> to be updated
    /// about generic data changes.
    /// Important: subclasses must make sure their <see cref="Value"/>
    /// property is set correctly after returning from the constructor.
    /// </summary>
    public abstract class EvaluatorBase : IDisposable
    {
        private object value;

        // ReSharper disable UnusedParameter.Local

        /// <summary>
        /// Initializes a new instance of the <see cref="EvaluatorBase"/> class.
        /// </summary>
        /// <param name="eval">
        /// The evaluation configuration.
        /// </param>
        /// <param name="context">
        /// The presentation context.
        /// </param>
        protected EvaluatorBase(EvalBase eval, IPresentationContext context)
        {
            this.Context = context;
        }

        // ReSharper restore UnusedParameter.Local

        /// <summary>
        /// Event that is fired every time <see cref="Value"/> changes.
        /// </summary>
        public event EventHandler ValueChanged;

        /// <summary>
        /// Gets or sets the value of this evaluator.
        /// </summary>
        public object Value
        {
            get
            {
                return this.value;
            }

            protected set
            {
                if (object.Equals(this.value, value))
                {
                    return;
                }

                this.value = value;

                var handler = this.ValueChanged;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the value is true.
        /// This tries to convert the <see cref="Value"/> to a boolean,
        /// either directly or converting it to an integer first.
        /// </summary>
        public virtual bool BoolValue
        {
            get
            {
                try
                {
                    try
                    {
                        return Convert.ToBoolean(this.Value);
                    }
                    catch (FormatException)
                    {
                        return Convert.ToBoolean(Convert.ToInt32(this.Value));
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the integer value or 0 if the <see cref="Value"/> can't be converted.
        /// </summary>
        public virtual int IntValue
        {
            get
            {
                try
                {
                    try
                    {
                        return Convert.ToInt32(this.Value);
                    }
                    catch (FormatException)
                    {
                        return Convert.ToInt32(Convert.ToBoolean(this.Value));
                    }
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets the string representation of the <see cref="Value"/>.
        /// </summary>
        public virtual string StringValue
        {
            get
            {
                return Convert.ToString(this.Value);
            }
        }

        /// <summary>
        /// Gets the presentation context.
        /// </summary>
        protected IPresentationContext Context { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing,
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// Initializes the <see cref="Value"/> from the constructor of subclasses.
        /// </summary>
        protected virtual void InitializeValue()
        {
        }
    }
}