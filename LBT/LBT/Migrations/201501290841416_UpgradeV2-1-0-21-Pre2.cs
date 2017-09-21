namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpgradeV21021Pre2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "ClaAccessNotification", c => c.Int(nullable: false));
            AddColumn("dbo.UserProfile", "ClaAccessFixCurrencyChange", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("dbo.Meeting", "SecondaryLeaderId", c => c.Int());
            AddForeignKey("dbo.Meeting", "SecondaryLeaderId", "dbo.UserProfile", "UserId");
            CreateIndex("dbo.Meeting", "SecondaryLeaderId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Meeting", new[] { "SecondaryLeaderId" });
            DropForeignKey("dbo.Meeting", "SecondaryLeaderId", "dbo.UserProfile");
            DropColumn("dbo.Meeting", "SecondaryLeaderId");
            DropColumn("dbo.UserProfile", "ClaAccessFixCurrencyChange");
            DropColumn("dbo.UserProfile", "ClaAccessNotification");
        }
    }
}
