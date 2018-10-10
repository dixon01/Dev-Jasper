// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITransformationSink{T}.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Transformations
{
    /// <summary>
    /// Interface for all classes that can be a receiver in a
    /// transformation chain.
    /// </summary>
    /// <typeparam name="T">
    /// The input type of the sink. This is the type received in
    /// the <see cref="Transform(T)"/> method.
    /// </typeparam>
    public interface ITransformationSink<T> : ITransformationSink
    {
        /// <summary>
        /// Transforms the given value. The handling of the result 
        /// of the transformation depends on the implementation.
        /// </summary>
        /// <param name="value">the value to be transformed.</param>
        void Transform(T value);
    }
}
