// <auto-generated />
namespace Gorba.Center.BackgroundSystem.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenamedUserDefinedPropertiesKeyToName : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.UserDefinedProperties", new[] { "Key" });
            this.RenameColumn("dbo.UserDefinedProperties", "Key", "Name");
        }
        
        public override void Down()
        {
            this.RenameColumn("dbo.UserDefinedProperties", "Name", "Key");
            CreateIndex("dbo.UserDefinedProperties", "Key", unique: true);
        }
    }
}
