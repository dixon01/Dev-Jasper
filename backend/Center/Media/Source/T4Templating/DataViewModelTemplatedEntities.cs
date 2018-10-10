// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataViewModelTemplatedEntities.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataViewModelTemplatedEntities type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.T4Templating
{
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the root class for entities.
    /// </summary>
    [XmlRoot("Entities")]
    public class DataViewModelTemplatedEntities
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataViewModelTemplatedEntities"/> class.
        /// </summary>
        public DataViewModelTemplatedEntities()
        {
            this.NamespaceEntityDescriptors =
                new ChildItemCollection<DataViewModelTemplatedEntities, NamespaceEntityDescriptor>(this);
        }

        /// <summary>
        /// Gets or sets the filters.
        /// </summary>
        /// <value>
        /// The filters.
        /// </value>
        public Filters Filters { get; set; }

        /// <summary>
        /// Gets the namespace entity descriptors.
        /// </summary>
        [XmlElement("Namespace")]
        public ChildItemCollection<DataViewModelTemplatedEntities, NamespaceEntityDescriptor> NamespaceEntityDescriptors
        {
            get; private set;
        }

        /// <summary>
        /// Loads the specified entities file path.
        /// </summary>
        /// <param name="entitiesFilePath">The entities file path.</param>
        /// <param name="dataViewModelEntitiesFilePath">The data view model entities file path.</param>
        /// <returns>The loaded entities.</returns>
        public static DataViewModelTemplatedEntities Load(string entitiesFilePath, string dataViewModelEntitiesFilePath)
        {
            Filters filters;
            using (
                var stream = new FileStream(
                    dataViewModelEntitiesFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var xmlSerializer = new XmlSerializer(typeof(Filters));
                {
                    filters = (Filters)xmlSerializer.Deserialize(stream);
                }
            }

            DataViewModelTemplatedEntities dataViewModelEntities;
            using (var stream = new FileStream(entitiesFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var xmlSerializer = new XmlSerializer(typeof(DataViewModelTemplatedEntities));
                {
                    dataViewModelEntities = (DataViewModelTemplatedEntities)xmlSerializer.Deserialize(stream);
                    dataViewModelEntities.Filters = filters;
                }
            }

            return dataViewModelEntities;
        }
    }
}