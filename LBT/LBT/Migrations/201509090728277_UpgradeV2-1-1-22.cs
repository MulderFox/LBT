namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpgradeV21122 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MSPContact", "DistrictId", "dbo.District");
            DropForeignKey("dbo.MSPContact", "PhoneNumberPrefix1Id", "dbo.PhoneNumberPrefix");
            DropForeignKey("dbo.MSPContact", "PhoneNumberPrefix2Id", "dbo.PhoneNumberPrefix");
            DropForeignKey("dbo.MSPContact", "RegistrarId", "dbo.UserProfile");
            DropForeignKey("dbo.SharedMSPContact", "FromUserId", "dbo.UserProfile");
            DropForeignKey("dbo.SharedMSPContact", "ToUserId", "dbo.UserProfile");
            //DropIndex("dbo.MSPContact", new[] { "DistrictId" });
            DropIndex("dbo.MSPContact", new[] { "PhoneNumberPrefix1Id" });
            DropIndex("dbo.MSPContact", new[] { "PhoneNumberPrefix2Id" });
            DropIndex("dbo.MSPContact", new[] { "RegistrarId" });
            DropIndex("dbo.SharedMSPContact", new[] { "FromUserId" });
            DropIndex("dbo.SharedMSPContact", new[] { "ToUserId" });
            AddColumn("dbo.Meeting", "SecondBankAccountId", c => c.Int());
            AddColumn("dbo.Meeting", "SecondPrice", c => c.Int());
            AddColumn("dbo.MeetingAttendee", "SecondPaidAmount", c => c.Int(nullable: false));
            AddForeignKey("dbo.Meeting", "SecondBankAccountId", "dbo.BankAccount", "BankAccountId");
            CreateIndex("dbo.Meeting", "SecondBankAccountId");
            DropColumn("dbo.UserProfile", "ContactedMspCount");
            DropColumn("dbo.UserProfile", "ContactedMspCountLastMonth");
            DropColumn("dbo.UserProfile", "RegistredMspQuota");
            DropColumn("dbo.UserProfile", "RegistredMspQuotaLastMonth");
            DropColumn("dbo.UserProfile", "ActiveMspQuota");
            DropColumn("dbo.UserProfile", "ActiveMspQuotaLastMonth");
            DropTable("dbo.MSPContact");
            DropTable("dbo.SharedMSPContact");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SharedMSPContact",
                c => new
                    {
                        SharedMSPContactId = c.Int(nullable: false, identity: true),
                        FromUserId = c.Int(nullable: false),
                        ToUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SharedMSPContactId);
            
            CreateTable(
                "dbo.MSPContact",
                c => new
                    {
                        MSPContactId = c.Int(nullable: false, identity: true),
                        ICO = c.String(nullable: false, maxLength: 40),
                        CompanyName = c.String(nullable: false, maxLength: 128),
                        ContactPerson = c.String(nullable: false, maxLength: 128),
                        Address = c.String(nullable: false),
                        PhoneNumberPrefix1Id = c.Int(nullable: false),
                        PhoneNumber1 = c.String(nullable: false, maxLength: 40),
                        BusinessInfoLocation = c.String(),
                        TeamMeetingLocation = c.String(),
                        MSPActive = c.Boolean(nullable: false),
                        DistrictId = c.Int(),
                        PhoneNumberPrefix2Id = c.Int(),
                        RegistrarId = c.Int(nullable: false),
                        PhoneNumber2 = c.String(maxLength: 40),
                        Email1 = c.String(maxLength: 100),
                        Email2 = c.String(maxLength: 100),
                        Created = c.DateTime(nullable: false),
                        FirstContacted = c.DateTime(),
                        Presented = c.DateTime(),
                        BusinessInfoParticipated = c.DateTime(),
                        TeamMeetingParticipated = c.DateTime(),
                        Registrated = c.DateTime(),
                        PremiumMembershipGranted = c.DateTime(),
                        Potential = c.Int(),
                        MobileApplicationInstalledAndTrained = c.Boolean(nullable: false),
                        ContactDead = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.MSPContactId);
            
            AddColumn("dbo.UserProfile", "ActiveMspQuotaLastMonth", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.UserProfile", "ActiveMspQuota", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.UserProfile", "RegistredMspQuotaLastMonth", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.UserProfile", "RegistredMspQuota", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.UserProfile", "ContactedMspCountLastMonth", c => c.Int());
            AddColumn("dbo.UserProfile", "ContactedMspCount", c => c.Int());
            DropIndex("dbo.Meeting", new[] { "SecondBankAccountId" });
            DropForeignKey("dbo.Meeting", "SecondBankAccountId", "dbo.BankAccount");
            DropColumn("dbo.MeetingAttendee", "SecondPaidAmount");
            DropColumn("dbo.Meeting", "SecondPrice");
            DropColumn("dbo.Meeting", "SecondBankAccountId");
            CreateIndex("dbo.SharedMSPContact", "ToUserId");
            CreateIndex("dbo.SharedMSPContact", "FromUserId");
            CreateIndex("dbo.MSPContact", "RegistrarId");
            CreateIndex("dbo.MSPContact", "PhoneNumberPrefix2Id");
            CreateIndex("dbo.MSPContact", "PhoneNumberPrefix1Id");
            CreateIndex("dbo.MSPContact", "DistrictId");
            AddForeignKey("dbo.SharedMSPContact", "ToUserId", "dbo.UserProfile", "UserId");
            AddForeignKey("dbo.SharedMSPContact", "FromUserId", "dbo.UserProfile", "UserId");
            AddForeignKey("dbo.MSPContact", "RegistrarId", "dbo.UserProfile", "UserId");
            AddForeignKey("dbo.MSPContact", "PhoneNumberPrefix2Id", "dbo.PhoneNumberPrefix", "PhoneNumberPrefixId");
            AddForeignKey("dbo.MSPContact", "PhoneNumberPrefix1Id", "dbo.PhoneNumberPrefix", "PhoneNumberPrefixId");
            AddForeignKey("dbo.MSPContact", "DistrictId", "dbo.District", "DistrictId");
        }
    }
}
