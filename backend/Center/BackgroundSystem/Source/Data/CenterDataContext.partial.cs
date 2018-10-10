// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CenterDataContext.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CenterDataContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Data
{
    using System.Data.Entity;

    using Gorba.Center.BackgroundSystem.Data.Model;

    /// <summary>
    /// Defines the center data context.
    /// </summary>
    public partial class CenterDataContext
    {
        static CenterDataContext()
        {
            // This is needed to ensure that EntityFramework.SqlServer.dll is copied to the output folder.
            // This fixes "Provider not loaded" exception on TFS build server.
            // See also http://robsneuron.blogspot.nl/2013/11/entity-framework-upgrade-to-6.html
            // ReSharper disable once UnusedVariable
            var ensureDllIsCopied = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CenterDataContext"/> class.
        /// </summary>
        /// <param name="nameOrConnectionString">
        /// The name or connection string.
        /// </param>
        public CenterDataContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CenterDataContext"/> class.
        /// </summary>
        public CenterDataContext()
        {
        }

        /// <summary>
        /// Gets or sets the XML data <see cref="IDbSet{TEntity}"/>.
        /// This property is not auto-generated since it's not used outside the data layer.
        /// </summary>
        public IDbSet<XmlData> XmlData { get; set; }

        /// <summary>
        /// The set values.
        /// </summary>
        /// <param name="original">
        /// The original to be updated.
        /// </param>
        /// <param name="entity">
        /// The entity containing new values.
        /// </param>
        /// <typeparam name="TEntity">
        /// The entity
        /// </typeparam>
        public virtual void SetValues<TEntity>(TEntity original, TEntity entity) where TEntity : class
        {
            var entry = this.Entry(original);
            entry.CurrentValues.SetValues(entity);
        }

        /// <summary>
        /// Loads the reference property of the <see cref="original"/> entry.
        /// </summary>
        /// <param name="original">
        /// The original entity entry.
        /// </param>
        /// <param name="referenceName">
        /// The name of the reference to be loaded.
        /// </param>
        /// <typeparam name="TEntity">
        /// The entity.
        /// </typeparam>
        public virtual void LoadReference<TEntity>(TEntity original, string referenceName) where TEntity : class
        {
            this.Entry(original).Reference(referenceName).Load();
        }

        /// <summary>
        /// Sets the current value of a reference property.
        /// </summary>
        /// <param name="original">
        /// The original entity entry.
        /// </param>
        /// <param name="referenceName">
        /// The name of the reference property.
        /// </param>
        /// <param name="newValue">
        /// The new value.
        /// </param>
        /// <typeparam name="TEntity">
        /// The entity.
        /// </typeparam>
        public virtual void SetReference<TEntity>(TEntity original, string referenceName, object newValue)
            where TEntity : class
        {
            this.Entry(original).Reference(referenceName).CurrentValue = newValue;
        }

        /// <summary>
        /// This method is called when the model for a derived context has been initialized, but
        ///             before the model has been locked down and used to initialize the context.  The default
        ///             implementation of this method does nothing, but it can be overridden in a derived class
        ///             such that the model can be further configured before it is locked down.
        /// </summary>
        /// <remarks>
        /// Typically, this method is called only once when the first instance of a derived context
        /// is created.  The model for that context is then cached and is for all further instances of
        /// the context in the app domain.  This caching can be disabled by setting the ModelCaching
        /// property on the given ModelBuilder, but note that this can seriously degrade performance.
        /// More control over caching is provided through use of the <c>DbModelBuilder</c> and <c>DbContextFactory</c>
        /// classes directly.
        /// </remarks>
        /// <param name="modelBuilder">The builder that defines the model for the context being created. </param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            this.DisableReferenceCascadeDelete(modelBuilder);
        }
    }
}