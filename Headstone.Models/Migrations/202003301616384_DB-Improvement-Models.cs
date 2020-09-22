namespace Headstone.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DBImprovementModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BasketItems",
                c => new
                    {
                        BasketItemID = c.Int(nullable: false, identity: true),
                        BasketID = c.Int(nullable: false),
                        ProductID = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        BasePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(),
                        Status = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.BasketItemID)
                .ForeignKey("dbo.Baskets", t => t.BasketID, cascadeDelete: true)
                .Index(t => t.BasketID);
            
            CreateTable(
                "dbo.Baskets",
                c => new
                    {
                        BasketID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(),
                        Status = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.BasketID);
            
            CreateTable(
                "dbo.Campaigns",
                c => new
                    {
                        CampaignID = c.Int(nullable: false, identity: true),
                        RelatedDataEntityName = c.String(),
                        RelatedDataEntityID = c.String(),
                        Name = c.String(),
                        ShortDescription = c.String(),
                        LongDescription = c.String(),
                        DiscountType = c.Int(nullable: false),
                        DiscountAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(),
                        Status = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.CampaignID);
            
            CreateTable(
                "dbo.CampaignProperties",
                c => new
                    {
                        PropertyId = c.Int(nullable: false, identity: true),
                        CampaignID = c.Int(nullable: false),
                        Key = c.String(),
                        Culture = c.String(),
                        Value = c.String(),
                        Extra = c.String(),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(),
                        Status = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.PropertyId)
                .ForeignKey("dbo.Campaigns", t => t.CampaignID, cascadeDelete: true)
                .Index(t => t.CampaignID);
            
            CreateTable(
                "dbo.Coupons",
                c => new
                    {
                        CouponID = c.Int(nullable: false, identity: true),
                        CouponCode = c.String(),
                        OwnerID = c.Int(),
                        CampaignID = c.Int(),
                        FromDate = c.DateTime(nullable: false),
                        ToDate = c.DateTime(),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(),
                        Status = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.CouponID)
                .ForeignKey("dbo.Campaigns", t => t.CampaignID)
                .Index(t => t.CampaignID);
            
            CreateTable(
                "dbo.OrderLines",
                c => new
                    {
                        OrderLineID = c.Int(nullable: false, identity: true),
                        OrderID = c.Int(nullable: false),
                        ProductID = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        BasePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(),
                        Status = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.OrderLineID)
                .ForeignKey("dbo.Orders", t => t.OrderID, cascadeDelete: true)
                .Index(t => t.OrderID);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        BasketID = c.Int(nullable: false),
                        CampaignID = c.Int(nullable: false),
                        AddressID = c.Int(nullable: false),
                        BillingInfoID = c.Int(nullable: false),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(),
                        Status = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.OrderID)
                .ForeignKey("dbo.Baskets", t => t.BasketID, cascadeDelete: true)
                .ForeignKey("dbo.Campaigns", t => t.CampaignID, cascadeDelete: true)
                .Index(t => t.BasketID)
                .Index(t => t.CampaignID);
            
            CreateTable(
                "dbo.OrderProperties",
                c => new
                    {
                        PropertyId = c.Int(nullable: false, identity: true),
                        OrderID = c.Int(nullable: false),
                        Key = c.String(),
                        Culture = c.String(),
                        Value = c.String(),
                        Extra = c.String(),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(),
                        Status = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.PropertyId)
                .ForeignKey("dbo.Orders", t => t.OrderID, cascadeDelete: true)
                .Index(t => t.OrderID);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        TransactionID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        OrderID = c.Int(nullable: false),
                        CardNumber = c.String(),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(),
                        Status = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.TransactionID)
                .ForeignKey("dbo.Orders", t => t.OrderID, cascadeDelete: true)
                .Index(t => t.OrderID);
            
            DropColumn("dbo.Comments", "OrganizationId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Comments", "OrganizationId", c => c.Int());
            DropForeignKey("dbo.Transactions", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.OrderProperties", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.OrderLines", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.Orders", "CampaignID", "dbo.Campaigns");
            DropForeignKey("dbo.Orders", "BasketID", "dbo.Baskets");
            DropForeignKey("dbo.Coupons", "CampaignID", "dbo.Campaigns");
            DropForeignKey("dbo.CampaignProperties", "CampaignID", "dbo.Campaigns");
            DropForeignKey("dbo.BasketItems", "BasketID", "dbo.Baskets");
            DropIndex("dbo.Transactions", new[] { "OrderID" });
            DropIndex("dbo.OrderProperties", new[] { "OrderID" });
            DropIndex("dbo.Orders", new[] { "CampaignID" });
            DropIndex("dbo.Orders", new[] { "BasketID" });
            DropIndex("dbo.OrderLines", new[] { "OrderID" });
            DropIndex("dbo.Coupons", new[] { "CampaignID" });
            DropIndex("dbo.CampaignProperties", new[] { "CampaignID" });
            DropIndex("dbo.BasketItems", new[] { "BasketID" });
            DropTable("dbo.Transactions");
            DropTable("dbo.OrderProperties");
            DropTable("dbo.Orders");
            DropTable("dbo.OrderLines");
            DropTable("dbo.Coupons");
            DropTable("dbo.CampaignProperties");
            DropTable("dbo.Campaigns");
            DropTable("dbo.Baskets");
            DropTable("dbo.BasketItems");
        }
    }
}
