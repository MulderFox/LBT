namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Upgrade5 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Statistics",
                c => new
                    {
                        StatisticsId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(),
                        StatisticsGroup = c.Int(nullable: false),
                        RegistredPeopleQuota = c.Decimal(precision: 18, scale: 2),
                        RegistredPeopleQuotaLastMonth = c.Decimal(precision: 18, scale: 2),
                        PremiumPartnersQuota = c.Decimal(precision: 18, scale: 2),
                        PremiumPartnersQuotaLastMonth = c.Decimal(precision: 18, scale: 2),
                        BuyersQuota = c.Decimal(precision: 18, scale: 2),
                        ContactedPeopleCount = c.Int(),
                        ContactedPeopleCountLastMonth = c.Int(),
                    })
                .PrimaryKey(t => t.StatisticsId)
                .ForeignKey("dbo.UserProfile", t => t.UserId)
                .Index(t => t.UserId);
            
            AlterColumn("dbo.PeopleContact", "City", c => c.String(maxLength: 128));
            AlterColumn("dbo.PeopleContact", "DistrictId", c => c.Int());
            AlterColumn("dbo.MSPContact", "DistrictId", c => c.Int());
        }
        
        public override void Down()
        {
            DropIndex("dbo.Statistics", new[] { "UserId" });
            DropForeignKey("dbo.Statistics", "UserId", "dbo.UserProfile");
            AlterColumn("dbo.MSPContact", "DistrictId", c => c.Int(nullable: false));
            AlterColumn("dbo.PeopleContact", "DistrictId", c => c.Int(nullable: false));
            AlterColumn("dbo.PeopleContact", "City", c => c.String(nullable: false, maxLength: 128));
            DropTable("dbo.Statistics");
        }
    }
}
