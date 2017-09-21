namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpgradeV21324 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Meeting", "WebinarUrl", c => c.String());
            AlterColumn("dbo.Meeting", "City", c => c.String());
            AlterColumn("dbo.Meeting", "AddressLine1", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Meeting", "AddressLine1", c => c.String(nullable: false));
            AlterColumn("dbo.Meeting", "City", c => c.String(nullable: false));
            DropColumn("dbo.Meeting", "WebinarUrl");
        }
    }
}
