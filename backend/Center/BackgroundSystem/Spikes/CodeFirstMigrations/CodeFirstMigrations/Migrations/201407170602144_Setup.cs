namespace CodeFirstMigrations.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    /// <summary>
    /// Initial migration to created the database with Lessons table.
    /// </summary>
    public partial class Setup : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Lessons",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Lessons");
        }
    }
}
