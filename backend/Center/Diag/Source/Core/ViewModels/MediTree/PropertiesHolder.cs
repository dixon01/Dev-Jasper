// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertiesHolder.cs" company="Gorba AG">
//   Copyright � 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PropertiesHolder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.MediTree
{
    using System.Collections.Generic;
    using System.Dynamic;

    /// <summary>
    /// A dynamic object that contains all (read-only) properties of a <see cref="ObjectNodeInfoViewModel"/>.
    /// </summary>
    public class PropertiesHolder : DynamicObject
    {
        private readonly IDictionary<string, object> properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertiesHolder"/> class.
        /// </summary>
        /// <param name="properties">
        /// The properties.
        /// </param>
        public PropertiesHolder(IDictionary<string, object> properties)
        {
            this.properties = properties;
        }

        /// <summary>
        /// Returns the enumeration of all dynamic member names.
        /// </summary>
        /// <returns>
        /// A sequence that contains dynamic member names.
        /// </returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return this.properties.Keys;
        }

        /// <summary>
        /// Provides the implementation for operations that get member values.
        /// Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override
        /// this method to specify dynamic behavior for operations such as getting a value for a property.
        /// </summary>
        /// <param name="binder">
        /// Provides information about the object that called the dynamic operation.
        /// The binder.Name property provides the name of the member on which the dynamic operation is performed.
        /// For example, for the Console.WriteLine(sampleObject.SampleProperty) statement,
        /// where sampleObject is an instance of the class derived from
        /// the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty".
        /// The binder.IgnoreCase property specifies whether the member name is case-sensitive.
        /// </param>
        /// <param name="result">
        /// The result of the get operation.
        /// For example, if the method is called for a property, you can assign the property value to
        /// <paramref name="result"/>.
        /// </param>
        /// <returns>
        /// true if the operation is successful; otherwise, false.
        /// If this method returns false, the run-time binder of the language determines the behavior.
        /// (In most cases, a run-time exception is thrown.)
        /// </returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return this.properties.TryGetValue(binder.Name, out result);
        }

        /// <summary>
        /// Provides the implementation for operations that set member values.
        /// Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override
        /// this method to specify dynamic behavior for operations such as setting a value for a property.
        /// </summary>
        /// <param name="binder">
        /// Provides information about the object that called the dynamic operation.
        /// The binder.Name property provides the name of the member to which the value is being assigned.
        /// For example, for the statement sampleObject.SampleProperty = "Test", where sampleObject is an instance
        /// of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class,
        /// binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether
        /// the member name is case-sensitive.</param><param name="value">The value to set to the member.
        /// For example, for sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class
        /// derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, the <paramref name="value"/> is "Test".
        /// </param>
        /// <returns>
        /// true if the operation is successful; otherwise, false.
        /// If this method returns false, the run-time binder of the language determines the behavior.
        /// (In most cases, a language-specific run-time exception is thrown.)
        /// </returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return false;
        }
    }
}