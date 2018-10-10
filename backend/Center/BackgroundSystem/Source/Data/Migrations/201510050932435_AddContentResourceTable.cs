// <auto-generated />
namespace Gorba.Center.BackgroundSystem.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddContentResourceTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ContentResources",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastModifiedOn = c.DateTime(precision: 7, storeType: "datetime2"),
                        Version = c.Int(nullable: false),
                        OriginalFilename = c.String(),
                        Description = c.String(),
                        ThumbnailHash = c.String(),
                        Hash = c.String(maxLength: 100),
                        HashAlgorithmType = c.Int(nullable: false),
                        MimeType = c.String(),
                        Length = c.Long(nullable: false),
                        UploadingUser_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UploadingUser_Id)
                .Index(t => new { t.Hash, t.HashAlgorithmType }, unique: true, name: "IX_HashAndHashType")
                .Index(t => t.UploadingUser_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ContentResources", "UploadingUser_Id", "dbo.Users");
            DropIndex("dbo.ContentResources", new[] { "UploadingUser_Id" });
            DropIndex("dbo.ContentResources", "IX_HashAndHashType");
            DropTable("dbo.ContentResources");
        }
    }
}
