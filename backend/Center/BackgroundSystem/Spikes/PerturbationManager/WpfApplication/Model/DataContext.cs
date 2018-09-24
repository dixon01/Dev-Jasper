// -----------------------------------------------------------------------
// <copyright file="DataContext.cs" company="HP">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace WpfApplication.Model
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DataContext : DbContext
    {
        public IDbSet<Unit> Units { get; set; }
    }
}
