namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Upgrade2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserProfile", "LeaderUserId", "dbo.UserProfile");
            DropIndex("dbo.UserProfile", new[] { "LeaderUserId" });
            AddColumn("dbo.UserProfile", "GoogleCalendarId", c => c.String());
            AddColumn("dbo.UserProfile", "MspCoach", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserProfile", "Active", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("dbo.UserProfile", "LyonessId", c => c.String(nullable: false, defaultValue: "000.000.000.000", maxLength: 15));
            AddColumn("dbo.UserProfile", "Note", c => c.String());
            AddColumn("dbo.UserProfile", "Tasks", c => c.String());
            AddColumn("dbo.UserProfile", "Skype", c => c.String());
            AddColumn("dbo.PeopleContact", "Tasks", c => c.String());
            AddColumn("dbo.PeopleContact", "LyonessId", c => c.String(maxLength: 15));
            AddColumn("dbo.PeopleContact", "WorkflowState", c => c.Int(nullable: false));
            DropColumn("dbo.UserProfile", "LeaderUserId");
            DropColumn("dbo.UserProfile", "Cc");
            DropColumn("dbo.UserProfile", "Mps");
            DropColumn("dbo.PeopleContact", "Assignments");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PeopleContact", "Assignments", c => c.String());
            AddColumn("dbo.UserProfile", "Mps", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserProfile", "Cc", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserProfile", "LeaderUserId", c => c.Int());
            DropColumn("dbo.PeopleContact", "WorkflowState");
            DropColumn("dbo.PeopleContact", "LyonessId");
            DropColumn("dbo.PeopleContact", "Tasks");
            DropColumn("dbo.UserProfile", "Skype");
            DropColumn("dbo.UserProfile", "Tasks");
            DropColumn("dbo.UserProfile", "Note");
            DropColumn("dbo.UserProfile", "LyonessId");
            DropColumn("dbo.UserProfile", "Active");
            DropColumn("dbo.UserProfile", "MspCoach");
            DropColumn("dbo.UserProfile", "GoogleCalendarId");
            CreateIndex("dbo.UserProfile", "LeaderUserId");
            AddForeignKey("dbo.UserProfile", "LeaderUserId", "dbo.UserProfile", "UserId");
        }
    }
}
