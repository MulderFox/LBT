namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GoogleApi : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "GoogleCredentialsJson", c => c.String());
            DropColumn("dbo.UserProfile", "GoogleCalendarUrl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserProfile", "GoogleCalendarUrl", c => c.String(maxLength: 256));
            DropColumn("dbo.UserProfile", "GoogleCredentialsJson");
        }
    }
}
