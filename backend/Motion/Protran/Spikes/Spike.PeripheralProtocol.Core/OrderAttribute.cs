// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderAttribute.cs" company="Luminator Technology Group">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <summary>
//   Defines the OrderAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PeripheralProtocol.Core
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class OrderAttribute : Attribute
    {
        private readonly int order;

        private readonly int size;

        public OrderAttribute([CallerLineNumber]int order = 0, int size = 0)
        {
            this.order = order;
            this.size = size;
        }

        public int Order { get { return this.order; } }

        public int Size { get { return this.size; } }
    }
}