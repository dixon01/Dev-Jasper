// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectWrapper.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ObjectWrapper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Tests.Transcoder.Bec.Utils
{
    using System;

    /// <summary>
    /// Wrapper around an object that was created using <see cref="ClassWrapper"/>.
    /// </summary>
    public class ObjectWrapper : MarshalByRefObject
    {
        private readonly object obj;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectWrapper"/> class.
        /// </summary>
        /// <param name="obj">
        /// The object to be wrapped.
        /// </param>
        internal ObjectWrapper(object obj)
        {
            this.obj = obj;
        }

        /// <summary>
        /// Gets the full class name.
        /// </summary>
        public string FullName
        {
            get
            {
                return this.obj.GetType().FullName;
            }
        }

        /// <summary>
        /// Gets the value of a given property.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <returns>
        /// The value of the property. Can be null.
        /// </returns>
        public object GetPropertyValue(string propertyName)
        {
            return this.obj.GetType().GetProperty(propertyName).GetValue(this.obj, null);
        }

        /// <summary>
        /// Gets the value of a given property wrapped inside an <see cref="ObjectWrapper"/>.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <returns>
        /// A wrapper around the value of the property. Can be null.
        /// </returns>
        public ObjectWrapper GetPropertyWrapperValue(string propertyName)
        {
            var value = this.GetPropertyValue(propertyName);
            return value == null ? null : new ObjectWrapper(value);
        }

        /// <summary>
        /// Sets the value of the given property.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// This can be an <see cref="ObjectWrapper"/> which will then be unwrapped.
        /// </param>
        public void SetPropertyValue(string propertyName, object value)
        {
            value = Unwrap(value);

            this.obj.GetType().GetProperty(propertyName).SetValue(this.obj, value, null);
        }

        /// <summary>
        /// Invoke a method on the wrapped object.
        /// IMPORTANT: don't use this method if the object returned by the method
        /// should be wrapped - use <see cref="InvokeMethodAndWrap"/> instead.
        /// </summary>
        /// <param name="methodName">
        /// The method name.
        /// </param>
        /// <param name="args">
        /// The method parameters.
        /// The parameters can be <see cref="ObjectWrapper"/>s which will then be unwrapped.
        /// </param>
        /// <returns>
        /// The return value of the method or null if the method doesn't have a return value.
        /// </returns>
        public object InvokeMethod(string methodName, params object[] args)
        {
            return this.obj.GetType().GetMethod(methodName).Invoke(this.obj, Array.ConvertAll(args, Unwrap));
        }

        /// <summary>
        /// Invoke a method on the wrapped object and wrap the result.
        /// </summary>
        /// <param name="methodName">
        /// The method name.
        /// </param>
        /// <param name="args">
        /// The method parameters.
        /// The parameters can be <see cref="ObjectWrapper"/>s which will then be unwrapped.
        /// </param>
        /// <returns>
        /// The return value of the method wrapped in a <see cref="ObjectWrapper"/> or
        /// null if the method doesn't have a return value or returned null.
        /// </returns>
        public ObjectWrapper InvokeMethodAndWrap(string methodName, params object[] args)
        {
            var result = this.InvokeMethod(methodName, args);
            return result == null ? null : new ObjectWrapper(result);
        }

        /// <summary>
        /// Gets the wrapped object.
        /// This method should only be used within the same AppDomain that created the wrapper.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        internal object GetObject()
        {
            return this.obj;
        }

        private static object Unwrap(object value)
        {
            var wrapper = value as ObjectWrapper;
            return wrapper != null ? wrapper.GetObject() : value;
        }
    }
}