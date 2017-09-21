namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpgradeV21021Pre6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "IsPoliciesAccepted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Meeting", "Private", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Meeting", "Private");
            DropColumn("dbo.UserProfile", "IsPoliciesAccepted");
        }
    }
}
