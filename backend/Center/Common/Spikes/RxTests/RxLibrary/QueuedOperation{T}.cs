// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueuedOperation{T}.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the QueuedOperation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RxLibrary
{
    /// <summary>
    /// Defines an operation that can be queued, marked with a name and containing a value.
    /// </summary>
    /// <typeparam name="T">The type of the value used for the operation.</typeparam>
    public sealed class QueuedOperation<T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueuedOperation{T}"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public QueuedOperation(string name, T value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueuedOperation{T}"/> class.
        /// </summary>
        public QueuedOperation()
        {
        }

        /// <summary>
        /// Gets or sets the name of the queued operation.
        /// </summary>
        /// <value>A name representing the operation like: Insert, Update, Delete, Clear, etc.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value to be used for the operation.
        /// </summary>
        public T Value { get; set; }
    }
}
