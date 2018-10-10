// --------------------------------------------------------------------------------------------------------------------
// <copyright file="201503230739245_XmlDataMoveData.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the XmlDataMoveData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    /// <summary>
    /// Manual migration that moves XML Data to the newly created table.
    /// </summary>
    public partial class XmlDataMoveData : DbMigration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            using (var context = DataContextFactory.Current.Create())
            {
                MoveXmlData(context.Database, "ProductTypes", "HardwareDescriptor");
                MoveXmlData(context.Database, "UpdateParts", "Structure");
                MoveXmlData(context.Database, "UpdateCommands", "Command");
                MoveXmlData(context.Database, "UpdateFeedbacks", "Feedback");
                MoveXmlData(context.Database, "DocumentVersions", "Content");
                MoveXmlData(context.Database, "PackageVersions", "Structure");
                MoveXmlData(context.Database, "SystemConfigs", "Settings");

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            throw new NotSupportedException("Can't revert data migration for now");
        }

        private static void MoveXmlData(Database database, string table, string column)
        {
            var productTypeIds = database.SqlQuery<int>(string.Format("SELECT [Id] FROM [{0}]", table)).ToList();
            foreach (var productTypeId in productTypeIds)
            {
                var xmlDataId =
                    database.SqlQuery<int>(
                        string.Format(
                            "{2} OUTPUT Inserted.Id SELECT [{1}Xml], [{1}XmlType] FROM [{0}] WHERE [Id] = @p0",
                            table,
                            column,
                            "INSERT INTO [XmlDatas] ([Xml], [Type])"),
                        productTypeId).Single();
                database.ExecuteSqlCommand(
                    string.Format("UPDATE [{0}] SET [{1}_Id] = @p0 WHERE [Id] = @p1", table, column),
                    xmlDataId,
                    productTypeId);
            }
        }
    }
}
