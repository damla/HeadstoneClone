namespace Headstone.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DBInit : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        CommentId = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(),
                        UserId = c.Int(nullable: false),
                        OrganizationId = c.Int(),
                        RelatedDataEntityType = c.String(),
                        RelatedDataEntityId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        Rating = c.Short(),
                        Body = c.String(),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(),
                        Status = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.CommentId)
                .ForeignKey("dbo.Comments", t => t.ParentId)
                .Index(t => t.ParentId);
            
            CreateTable(
                "dbo.CommentProperties",
                c => new
                    {
                        PropertyId = c.Int(nullable: false, identity: true),
                        CommentId = c.Int(nullable: false),
                        Key = c.String(),
                        Culture = c.String(),
                        Value = c.String(),
                        Extra = c.String(),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(),
                        Status = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.PropertyId)
                .ForeignKey("dbo.Comments", t => t.CommentId, cascadeDelete: true)
                .Index(t => t.CommentId);
            
            CreateTable(
                "dbo.CommentTags",
                c => new
                    {
                        TagId = c.Int(nullable: false, identity: true),
                        CommentId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        Culture = c.String(),
                        Value = c.String(),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(),
                        Status = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.TagId)
                .ForeignKey("dbo.Comments", t => t.CommentId, cascadeDelete: true)
                .Index(t => t.CommentId);
            
            CreateTable(
                "dbo.GeoLocations",
                c => new
                    {
                        GeoLocationId = c.Int(nullable: false, identity: true),
                        Path = c.String(),
                        Code = c.String(),
                        Parent = c.String(),
                        Name = c.String(),
                        Type = c.Int(nullable: false),
                        SortOrder = c.Int(nullable: false),
                        Latitude = c.Single(),
                        Longitude = c.Single(),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(),
                        Status = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.GeoLocationId);
            
            CreateTable(
                "dbo.TaxOffices",
                c => new
                    {
                        TaxOfficeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        GeoCode = c.String(),
                        Code = c.String(),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(),
                        Status = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.TaxOfficeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CommentTags", "CommentId", "dbo.Comments");
            DropForeignKey("dbo.CommentProperties", "CommentId", "dbo.Comments");
            DropForeignKey("dbo.Comments", "ParentId", "dbo.Comments");
            DropIndex("dbo.CommentTags", new[] { "CommentId" });
            DropIndex("dbo.CommentProperties", new[] { "CommentId" });
            DropIndex("dbo.Comments", new[] { "ParentId" });
            DropTable("dbo.TaxOffices");
            DropTable("dbo.GeoLocations");
            DropTable("dbo.CommentTags");
            DropTable("dbo.CommentProperties");
            DropTable("dbo.Comments");
        }
    }
}
