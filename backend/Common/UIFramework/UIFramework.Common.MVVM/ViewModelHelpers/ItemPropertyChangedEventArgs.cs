// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemPropertyChangedEventArgs.cs" company="">
//   Copyright (c) 2013
//   Luminator Technology Group
//   All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.UIFramework.Common.MVVM.ViewModelHelpers
{
    using System.ComponentModel;

    /// <summary>
    /// The item property changed event args.
    /// </summary>
    public class ItemPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        public int Index { get; set; }

        public ItemPropertyChangedEventArgs(int index, string propertyName)
            : base(propertyName)
        {
            this.Index = index;
        }
    }
}