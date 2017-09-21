namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpgradeV20313Pre2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.MeetingAttendee", new[] { "RegistrarId" });
            DropIndex("dbo.MeetingAttendee", new[] { "AttendeeId" });
            DropIndex("dbo.MeetingAttendee", new[] { "MeetingId" });
            DropIndex("dbo.Meeting", new[] { "IdentifyingGroupId" });
            DropIndex("dbo.Meeting", new[] { "BankAccountId" });
            DropIndex("dbo.Meeting", new[] { "OrganizerId" });
            DropForeignKey("dbo.MeetingAttendee", "RegistrarId", "dbo.UserProfile");
            DropForeignKey("dbo.MeetingAttendee", "AttendeeId", "dbo.PeopleContact");
            DropForeignKey("dbo.MeetingAttendee", "MeetingId", "dbo.Meeting");
            DropForeignKey("dbo.Meeting", "IdentifyingGroupId", "dbo.IdentifyingGroup");
            DropForeignKey("dbo.Meeting", "BankAccountId", "dbo.BankAccount");
            DropForeignKey("dbo.Meeting", "OrganizerId", "dbo.UserProfile");
            DropTable("dbo.MeetingAttendee");
            DropTable("dbo.Meeting");
            DropTable("dbo.IdentifyingGroup");

            CreateTable(
                "dbo.Meeting",
                c => new
                {
                    MeetingId = c.Int(nullable: false, identity: true),
                    OrganizerId = c.Int(nullable: false),
                    BankAccountId = c.Int(),
                    MainLeaderId = c.Int(nullable: false),
                    Title = c.String(nullable: false),
                    MeetingKind = c.Int(nullable: false),
                    MeetingType = c.Int(nullable: false),
                    City = c.String(nullable: false),
                    AddressLine1 = c.String(nullable: false),
                    AddressLine2 = c.String(),
                    Chargeable = c.Boolean(nullable: false),
                    Price = c.Int(),
                    WithAccommodation = c.Boolean(nullable: false),
                    PriceWithAccommodation = c.Int(),
                    Started = c.DateTime(nullable: false),
                    Finished = c.DateTime(nullable: false),
                    BringYourOwn = c.String(),
                    Capacity = c.Int(nullable: false),
                    AccomodationCapacity = c.Int(nullable: false),
                    RegisterDeadline = c.DateTime(),
                    Lecturer = c.String(),
                    InvitationCardUrl = c.String(),
                    Note = c.String(),
                    Total = c.Int(),
                    TotalWithAccommodation = c.Int(),
                })
                .PrimaryKey(t => t.MeetingId)
                .ForeignKey("dbo.UserProfile", t => t.OrganizerId)
                .ForeignKey("dbo.BankAccount", t => t.BankAccountId)
                .ForeignKey("dbo.UserProfile", t => t.MainLeaderId)
                .Index(t => t.OrganizerId)
                .Index(t => t.BankAccountId)
                .Index(t => t.MainLeaderId);

            CreateTable(
                "dbo.MeetingAttendee",
                c => new
                {
                    MeetingAttendeeId = c.Int(nullable: false, identity: true),
                    MeetingId = c.Int(nullable: false),
                    AttendeeId = c.Int(),
                    UserAttendeeId = c.Int(),
                    RegistrarId = c.Int(nullable: false),
                    MeetingAttendeeType = c.Int(nullable: false),
                    PaidAmount = c.Int(nullable: false),
                    Reserved = c.DateTime(),
                    Registered = c.DateTime(),
                    BonusLyonessId = c.String(),
                    AccommodationNote = c.String(),
                    BankAccountHistoryNote = c.String(),
                    Note = c.String(),
                })
                .PrimaryKey(t => t.MeetingAttendeeId)
                .ForeignKey("dbo.Meeting", t => t.MeetingId)
                .ForeignKey("dbo.PeopleContact", t => t.AttendeeId)
                .ForeignKey("dbo.UserProfile", t => t.UserAttendeeId)
                .ForeignKey("dbo.UserProfile", t => t.RegistrarId)
                .Index(t => t.MeetingId)
                .Index(t => t.AttendeeId)
                .Index(t => t.UserAttendeeId)
                .Index(t => t.RegistrarId);
        }
        
        public override void Down()
        {
            DropIndex("dbo.MeetingAttendee", new[] { "RegistrarId" });
            DropIndex("dbo.MeetingAttendee", new[] { "AttendeeId" });
            DropIndex("dbo.MeetingAttendee", new[] { "MeetingId" });
            DropIndex("dbo.Meeting", new[] { "MainLeaderId" });
            DropIndex("dbo.Meeting", new[] { "BankAccountId" });
            DropIndex("dbo.Meeting", new[] { "OrganizerId" });
            DropForeignKey("dbo.MeetingAttendee", "RegistrarId", "dbo.UserProfile");
            DropForeignKey("dbo.MeetingAttendee", "AttendeeId", "dbo.PeopleContact");
            DropForeignKey("dbo.MeetingAttendee", "MeetingId", "dbo.Meeting");
            DropForeignKey("dbo.Meeting", "MainLeaderId", "dbo.UserProfile");
            DropForeignKey("dbo.Meeting", "BankAccountId", "dbo.BankAccount");
            DropForeignKey("dbo.Meeting", "OrganizerId", "dbo.UserProfile");
            DropTable("dbo.MeetingAttendee");
            DropTable("dbo.Meeting");

            CreateTable(
                "dbo.IdentifyingGroup",
                c => new
                {
                    IdentifyingGroupId = c.Int(nullable: false, identity: true),
                    Title = c.String(nullable: false),
                    Note = c.String(),
                })
                .PrimaryKey(t => t.IdentifyingGroupId);

            CreateTable(
                "dbo.Meeting",
                c => new
                {
                    MeetingId = c.Int(nullable: false, identity: true),
                    OrganizerId = c.Int(nullable: false),
                    BankAccountId = c.Int(),
                    IdentifyingGroupId = c.Int(nullable: false),
                    Title = c.String(nullable: false),
                    MeetingKind = c.Int(nullable: false),
                    MeetingType = c.Int(nullable: false),
                    City = c.String(nullable: false),
                    AddressLine1 = c.String(nullable: false),
                    AddressLine2 = c.String(),
                    Chargeable = c.Boolean(nullable: false),
                    Price = c.Int(),
                    WithAccommodation = c.Boolean(nullable: false),
                    PriceWithAccommodation = c.Int(),
                    Started = c.DateTime(nullable: false),
                    Finished = c.DateTime(nullable: false),
                    BringYourOwn = c.String(),
                    Capacity = c.Int(nullable: false),
                    AccomodationCapacity = c.Int(nullable: false),
                    RegisterDeadline = c.DateTime(),
                    Description = c.String(nullable: false),
                    InvitationCardUrl = c.String(),
                    Note = c.String(),
                    Total = c.Int(),
                    TotalWithAccommodation = c.Int(),
                })
                .PrimaryKey(t => t.MeetingId)
                .ForeignKey("dbo.UserProfile", t => t.OrganizerId)
                .ForeignKey("dbo.BankAccount", t => t.BankAccountId)
                .ForeignKey("dbo.IdentifyingGroup", t => t.IdentifyingGroupId)
                .Index(t => t.OrganizerId)
                .Index(t => t.BankAccountId)
                .Index(t => t.IdentifyingGroupId);

            CreateTable(
                "dbo.MeetingAttendee",
                c => new
                {
                    MeetingAttendeeId = c.Int(nullable: false, identity: true),
                    MeetingId = c.Int(nullable: false),
                    AttendeeId = c.Int(nullable: false),
                    RegistrarId = c.Int(nullable: false),
                    MeetingAttendeeType = c.Int(nullable: false),
                    PaidAmount = c.Int(nullable: false),
                    Reserved = c.DateTime(),
                    Registered = c.DateTime(),
                    BonusLyonessId = c.String(),
                    AccommodationNote = c.String(),
                    BankAccountHistoryNote = c.String(),
                    Note = c.String(),
                })
                .PrimaryKey(t => t.MeetingAttendeeId)
                .ForeignKey("dbo.Meeting", t => t.MeetingId)
                .ForeignKey("dbo.PeopleContact", t => t.AttendeeId)
                .ForeignKey("dbo.UserProfile", t => t.RegistrarId)
                .Index(t => t.MeetingId)
                .Index(t => t.AttendeeId)
                .Index(t => t.RegistrarId);
        }
    }
}
