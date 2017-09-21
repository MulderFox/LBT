namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpgradeV21526 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "AutomaticLogoutInterval", c => c.Int(nullable: false, defaultValue: 10));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfile", "AutomaticLogoutInterval");
        }
    }
}
