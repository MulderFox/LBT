namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpgradeV20616Pre4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Meeting", "SecondaryOrganizerId", c => c.Int());
            AddForeignKey("dbo.Meeting", "SecondaryOrganizerId", "dbo.UserProfile", "UserId");
            CreateIndex("dbo.Meeting", "SecondaryOrganizerId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Meeting", new[] { "SecondaryOrganizerId" });
            DropForeignKey("dbo.Meeting", "SecondaryOrganizerId", "dbo.UserProfile");
            DropColumn("dbo.Meeting", "SecondaryOrganizerId");
        }
    }
}
