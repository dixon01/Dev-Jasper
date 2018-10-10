namespace Gorba.Center.Common.Wpf.Core.Collections
{
    using System.ComponentModel;

    public class ItemPropertyChangedEventArgs<T> : PropertyChangedEventArgs
    {
        public ItemPropertyChangedEventArgs(T item, string propertyName)
            : base(propertyName)
        {
            this.Item = item;
        }

        public T Item { get; private set; }
    }
}