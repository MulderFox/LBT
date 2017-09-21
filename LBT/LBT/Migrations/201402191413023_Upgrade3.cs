namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Upgrade3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "SmsEmail", c => c.String(maxLength: 100));
            AddColumn("dbo.UserProfile", "UseMail", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("dbo.UserProfile", "UseSms", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("dbo.UserProfile", "UseGoogleCalendar", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfile", "UseGoogleCalendar");
            DropColumn("dbo.UserProfile", "UseSms");
            DropColumn("dbo.UserProfile", "UseMail");
            DropColumn("dbo.UserProfile", "SmsEmail");
        }
    }
}
