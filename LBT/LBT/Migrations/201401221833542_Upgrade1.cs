namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Upgrade1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "Title", c => c.String(maxLength: 20));
            AddColumn("dbo.UserProfile", "RegistrarId", c => c.Int());
            AddColumn("dbo.PeopleContact", "Title", c => c.String(maxLength: 20));
            AddColumn("dbo.PeopleContact", "Skype", c => c.String(maxLength: 100));
            AddColumn("dbo.PeopleContact", "TrackingEmailSent", c => c.Boolean(nullable: false));
            AddColumn("dbo.PeopleContact", "SecondContacted", c => c.DateTime());
            AddColumn("dbo.PeopleContact", "SecondMeeting", c => c.DateTime());
            AddColumn("dbo.PeopleContact", "SecondTrackingEmailSent", c => c.Boolean(nullable: false));
            AddColumn("dbo.PeopleContact", "ThirdMeeting", c => c.DateTime());
            AddColumn("dbo.PeopleContact", "LoyaltySystemExplained", c => c.Boolean(nullable: false));
            AddColumn("dbo.PeopleContact", "AutoCashback", c => c.Boolean(nullable: false));
            AddColumn("dbo.PeopleContact", "ShoppingPlanBackSet", c => c.Boolean(nullable: false));
            AddColumn("dbo.PeopleContact", "OwnUnitsContained", c => c.Boolean(nullable: false));
            AddColumn("dbo.PeopleContact", "Assignments", c => c.String());
            AddColumn("dbo.PeopleContact", "Note", c => c.String());
            AddColumn("dbo.MSPContact", "TeamMeetingLocation", c => c.String());
            AlterColumn("dbo.PeopleContact", "PhoneNumber1", c => c.String(maxLength: 40));
            AlterColumn("dbo.PeopleContact", "PhoneNumberPrefix1Id", c => c.Int());
            DropColumn("dbo.UserProfile", "Title1");
            DropColumn("dbo.UserProfile", "Title2");
            DropColumn("dbo.PeopleContact", "Title1");
            DropColumn("dbo.PeopleContact", "Title2");
            DropColumn("dbo.PeopleContact", "ShoppingPlanAndAutoCashbackSet");
            DropColumn("dbo.PeopleContact", "UnitsContained");
            DropColumn("dbo.PeopleContact", "BusinessInfoLocation");
            AddForeignKey("dbo.UserProfile", "RegistrarId", "dbo.UserProfile", "UserId");
            CreateIndex("dbo.UserProfile", "RegistrarId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.UserProfile", new[] { "RegistrarId" });
            DropForeignKey("dbo.UserProfile", "RegistrarId", "dbo.UserProfile");
            AddColumn("dbo.PeopleContact", "BusinessInfoLocation", c => c.String());
            AddColumn("dbo.PeopleContact", "UnitsContained", c => c.Boolean(nullable: false));
            AddColumn("dbo.PeopleContact", "ShoppingPlanAndAutoCashbackSet", c => c.Boolean(nullable: false));
            AddColumn("dbo.PeopleContact", "Title2", c => c.String(maxLength: 10));
            AddColumn("dbo.PeopleContact", "Title1", c => c.String(maxLength: 10));
            AddColumn("dbo.UserProfile", "Title2", c => c.String(maxLength: 10));
            AddColumn("dbo.UserProfile", "Title1", c => c.String(maxLength: 10));
            AlterColumn("dbo.PeopleContact", "PhoneNumberPrefix1Id", c => c.Int(nullable: false));
            AlterColumn("dbo.PeopleContact", "PhoneNumber1", c => c.String(nullable: false, maxLength: 40));
            DropColumn("dbo.MSPContact", "TeamMeetingLocation");
            DropColumn("dbo.PeopleContact", "Note");
            DropColumn("dbo.PeopleContact", "Assignments");
            DropColumn("dbo.PeopleContact", "OwnUnitsContained");
            DropColumn("dbo.PeopleContact", "ShoppingPlanBackSet");
            DropColumn("dbo.PeopleContact", "AutoCashback");
            DropColumn("dbo.PeopleContact", "LoyaltySystemExplained");
            DropColumn("dbo.PeopleContact", "ThirdMeeting");
            DropColumn("dbo.PeopleContact", "SecondTrackingEmailSent");
            DropColumn("dbo.PeopleContact", "SecondMeeting");
            DropColumn("dbo.PeopleContact", "SecondContacted");
            DropColumn("dbo.PeopleContact", "TrackingEmailSent");
            DropColumn("dbo.PeopleContact", "Skype");
            DropColumn("dbo.PeopleContact", "Title");
            DropColumn("dbo.UserProfile", "RegistrarId");
            DropColumn("dbo.UserProfile", "Title");
        }
    }
}
