namespace Headstone.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DBBugfix : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Campaigns", "RelatedDataEntityID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Campaigns", "RelatedDataEntityID", c => c.String());
        }
    }
}
