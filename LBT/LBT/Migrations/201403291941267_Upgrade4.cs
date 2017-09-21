namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Upgrade4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "ReminderTime", c => c.String());
            AddColumn("dbo.UserProfile", "IsEventsPrivate", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfile", "IsEventsPrivate");
            DropColumn("dbo.UserProfile", "ReminderTime");
        }
    }
}
