namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserProfile",
                c => new
                         {
                             UserId = c.Int(nullable: false, identity: true),
                             LeaderUserId = c.Int(),
                             DistrictId = c.Int(nullable: false),
                             PhoneNumberPrefix1Id = c.Int(nullable: false),
                             PhoneNumberPrefix2Id = c.Int(),
                             UserName = c.String(),
                             FirstName = c.String(nullable: false, maxLength: 40),
                             LastName = c.String(nullable: false, maxLength: 40),
                             Title1 = c.String(maxLength: 10),
                             Title2 = c.String(maxLength: 10),
                             City = c.String(nullable: false, maxLength: 128),
                             PhoneNumber1 = c.String(nullable: false, maxLength: 40),
                             PhoneNumber2 = c.String(maxLength: 40),
                             Email1 = c.String(nullable: false, maxLength: 100),
                             Email2 = c.String(maxLength: 100),
                             GoogleCalendarUrl = c.String(maxLength: 256),
                             PremiumMembershipGranted = c.DateTime(),
                             AccessGranted = c.DateTime(nullable: false),
                             RegistredPeopleQuota = c.Decimal(precision: 18, scale: 2),
                             RegistredPeopleQuotaPerMonth = c.Decimal(precision: 18, scale: 2),
                             PremiumPartnersQuota = c.Decimal(precision: 18, scale: 2),
                             PremiumPartnersQuotaPerMonth = c.Decimal(precision: 18, scale: 2),
                             BuyersQuota = c.Decimal(precision: 18, scale: 2),
                             ContactedPeopleCount = c.Int(),
                             ContactedPeopleCountPerMonth = c.Int(),
                             ContactedMspCount = c.Int(),
                             ContactedMspCountPerMonth = c.Int(),
                             RegistredMspQuota = c.Decimal(precision: 18, scale: 2),
                             RegistredMspQuotaPerMonth = c.Decimal(precision: 18, scale: 2),
                             ActiveMspQuota = c.Decimal(precision: 18, scale: 2),
                             ActiveMspQuotaPerMonth = c.Decimal(precision: 18, scale: 2),
                             Ca = c.Boolean(nullable: false),
                             Cc = c.Boolean(nullable: false),
                             Presenting = c.Boolean(nullable: false),
                             Mps = c.Boolean(nullable: false),
                             LastAccessed = c.DateTime(),
                         })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.District", t => t.DistrictId)
                .ForeignKey("dbo.PhoneNumberPrefix", t => t.PhoneNumberPrefix1Id)
                .ForeignKey("dbo.PhoneNumberPrefix", t => t.PhoneNumberPrefix2Id)
                .ForeignKey("dbo.UserProfile", t => t.LeaderUserId)
                .Index(t => t.DistrictId)
                .Index(t => t.PhoneNumberPrefix1Id)
                .Index(t => t.PhoneNumberPrefix2Id)
                .Index(t => t.LeaderUserId);
            
            CreateTable(
                "dbo.District",
                c => new
                    {
                        DistrictId = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.DistrictId)
                .Index(t => t.Title, true);
            
            CreateTable(
                "dbo.PhoneNumberPrefix",
                c => new
                    {
                        PhoneNumberPrefixId = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 40),
                    })
                .PrimaryKey(t => t.PhoneNumberPrefixId);
            
            CreateTable(
                "dbo.PeopleContact",
                c => new
                    {
                        PeopleContactId = c.Int(nullable: false, identity: true),
                        DistrictId = c.Int(nullable: false),
                        PhoneNumberPrefix1Id = c.Int(nullable: false),
                        PhoneNumberPrefix2Id = c.Int(),
                        RegistrarId = c.Int(nullable: false),
                        FirstName = c.String(nullable: false, maxLength: 40),
                        LastName = c.String(nullable: false, maxLength: 40),
                        Title1 = c.String(maxLength: 10),
                        Title2 = c.String(maxLength: 10),
                        City = c.String(nullable: false, maxLength: 128),
                        PhoneNumber1 = c.String(nullable: false, maxLength: 40),
                        PhoneNumber2 = c.String(maxLength: 40),
                        Email1 = c.String(maxLength: 100),
                        Email2 = c.String(maxLength: 100),
                        Created = c.DateTime(nullable: false),
                        FirstContacted = c.DateTime(),
                        Presented = c.DateTime(),
                        BusinessInfoParticipated = c.DateTime(),
                        BusinessInfoLocation = c.String(),
                        TeamMeetingParticipated = c.DateTime(),
                        Registrated = c.DateTime(),
                        PremiumMembershipGranted = c.DateTime(),
                        AccessGranted = c.DateTime(),
                        Potential = c.Int(),
                        MoneyToPurchaseAccountSended = c.Boolean(nullable: false),
                        MobileApplicationInstalledAndTrained = c.Boolean(nullable: false),
                        AbleToPurchase = c.Boolean(nullable: false),
                        ShoppingPlanAndAutoCashbackSet = c.Boolean(nullable: false),
                        UnitsContained = c.Boolean(nullable: false),
                        ContactDead = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.PeopleContactId)
                .ForeignKey("dbo.UserProfile", t => t.RegistrarId)
                .ForeignKey("dbo.District", t => t.DistrictId)
                .ForeignKey("dbo.PhoneNumberPrefix", t => t.PhoneNumberPrefix1Id)
                .ForeignKey("dbo.PhoneNumberPrefix", t => t.PhoneNumberPrefix2Id)
                .Index(t => t.RegistrarId)
                .Index(t => t.DistrictId)
                .Index(t => t.PhoneNumberPrefix1Id)
                .Index(t => t.PhoneNumberPrefix2Id);

            CreateTable(
                "dbo.MSPContact",
                c => new
                         {
                             MSPContactId = c.Int(nullable: false, identity: true),
                             DistrictId = c.Int(nullable: false),
                             PhoneNumberPrefix1Id = c.Int(nullable: false),
                             PhoneNumberPrefix2Id = c.Int(),
                             RegistrarId = c.Int(nullable: false),
                             ICO = c.String(nullable: false, maxLength: 40),
                             CompanyName = c.String(nullable: false, maxLength: 128),
                             ContactPerson = c.String(nullable: false, maxLength: 128),
                             Address = c.String(nullable: false),
                             PhoneNumber1 = c.String(nullable: false, maxLength: 40),
                             PhoneNumber2 = c.String(maxLength: 40),
                             Email1 = c.String(maxLength: 100),
                             Email2 = c.String(maxLength: 100),
                             Created = c.DateTime(nullable: false),
                             FirstContacted = c.DateTime(),
                             Presented = c.DateTime(),
                             BusinessInfoParticipated = c.DateTime(),
                             BusinessInfoLocation = c.String(),
                             TeamMeetingParticipated = c.DateTime(),
                             Registrated = c.DateTime(),
                             PremiumMembershipGranted = c.DateTime(),
                             Potential = c.Int(),
                             MobileApplicationInstalledAndTrained = c.Boolean(nullable: false),
                             ContactDead = c.Boolean(nullable: false),
                             MSPActive = c.Boolean(nullable: false),
                         })
                .PrimaryKey(t => t.MSPContactId)
                .ForeignKey("dbo.UserProfile", t => t.RegistrarId)
                .ForeignKey("dbo.District", t => t.DistrictId)
                .ForeignKey("dbo.PhoneNumberPrefix", t => t.PhoneNumberPrefix1Id)
                .ForeignKey("dbo.PhoneNumberPrefix", t => t.PhoneNumberPrefix2Id)
                .Index(t => t.RegistrarId)
                .Index(t => t.DistrictId)
                .Index(t => t.PhoneNumberPrefix1Id)
                .Index(t => t.PhoneNumberPrefix2Id)
                .Index(t => t.ICO, true);

        }
        
        public override void Down()
        {
            DropIndex("dbo.MSPContact", new[] { "PhoneNumberPrefix2Id" });
            DropIndex("dbo.MSPContact", new[] { "PhoneNumberPrefix1Id" });
            DropIndex("dbo.MSPContact", new[] { "DistrictId" });
            DropIndex("dbo.MSPContact", new[] { "RegistrarId" });
            DropIndex("dbo.PeopleContact", new[] { "PhoneNumberPrefix2Id" });
            DropIndex("dbo.PeopleContact", new[] { "PhoneNumberPrefix1Id" });
            DropIndex("dbo.PeopleContact", new[] { "DistrictId" });
            DropIndex("dbo.PeopleContact", new[] { "RegistrarId" });
            DropIndex("dbo.UserProfile", new[] { "LeaderUserId" });
            DropIndex("dbo.UserProfile", new[] { "PhoneNumberPrefix2Id" });
            DropIndex("dbo.UserProfile", new[] { "PhoneNumberPrefix1Id" });
            DropIndex("dbo.UserProfile", new[] { "DistrictId" });
            DropForeignKey("dbo.MSPContact", "PhoneNumberPrefix2Id", "dbo.PhoneNumberPrefix");
            DropForeignKey("dbo.MSPContact", "PhoneNumberPrefix1Id", "dbo.PhoneNumberPrefix");
            DropForeignKey("dbo.MSPContact", "DistrictId", "dbo.District");
            DropForeignKey("dbo.MSPContact", "RegistrarId", "dbo.UserProfile");
            DropForeignKey("dbo.PeopleContact", "PhoneNumberPrefix2Id", "dbo.PhoneNumberPrefix");
            DropForeignKey("dbo.PeopleContact", "PhoneNumberPrefix1Id", "dbo.PhoneNumberPrefix");
            DropForeignKey("dbo.PeopleContact", "DistrictId", "dbo.District");
            DropForeignKey("dbo.PeopleContact", "RegistrarId", "dbo.UserProfile");
            DropForeignKey("dbo.UserProfile", "LeaderUserId", "dbo.UserProfile");
            DropForeignKey("dbo.UserProfile", "PhoneNumberPrefix2Id", "dbo.PhoneNumberPrefix");
            DropForeignKey("dbo.UserProfile", "PhoneNumberPrefix1Id", "dbo.PhoneNumberPrefix");
            DropForeignKey("dbo.UserProfile", "DistrictId", "dbo.District");
            DropTable("dbo.MSPContact");
            DropTable("dbo.PeopleContact");
            DropTable("dbo.PhoneNumberPrefix");
            DropTable("dbo.District");
            DropTable("dbo.UserProfile");
        }
    }
}
