namespace DynamicQuery
{
    using System.Linq.Expressions;

    internal class DynamicOrdering
    {
        public Expression Selector;

        public bool Ascending;
    }
}