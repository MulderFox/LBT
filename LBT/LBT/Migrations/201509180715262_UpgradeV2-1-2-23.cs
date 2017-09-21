namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpgradeV21223 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "LCID", c => c.Int(nullable: false, defaultValue: 1029));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfile", "LCID");
        }
    }
}
