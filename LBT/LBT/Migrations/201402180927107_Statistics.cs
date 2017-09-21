namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Statistics : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "RegistredPeopleQuotaLastMonth", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.UserProfile", "PremiumPartnersQuotaLastMonth", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.UserProfile", "ContactedPeopleCountLastMonth", c => c.Int());
            AddColumn("dbo.UserProfile", "ContactedMspCountLastMonth", c => c.Int());
            AddColumn("dbo.UserProfile", "RegistredMspQuotaLastMonth", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.UserProfile", "ActiveMspQuotaLastMonth", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.UserProfile", "RegistredPeopleQuotaPerMonth");
            DropColumn("dbo.UserProfile", "PremiumPartnersQuotaPerMonth");
            DropColumn("dbo.UserProfile", "ContactedPeopleCountPerMonth");
            DropColumn("dbo.UserProfile", "ContactedMspCountPerMonth");
            DropColumn("dbo.UserProfile", "RegistredMspQuotaPerMonth");
            DropColumn("dbo.UserProfile", "ActiveMspQuotaPerMonth");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserProfile", "ActiveMspQuotaPerMonth", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.UserProfile", "RegistredMspQuotaPerMonth", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.UserProfile", "ContactedMspCountPerMonth", c => c.Int());
            AddColumn("dbo.UserProfile", "ContactedPeopleCountPerMonth", c => c.Int());
            AddColumn("dbo.UserProfile", "PremiumPartnersQuotaPerMonth", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.UserProfile", "RegistredPeopleQuotaPerMonth", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.UserProfile", "ActiveMspQuotaLastMonth");
            DropColumn("dbo.UserProfile", "RegistredMspQuotaLastMonth");
            DropColumn("dbo.UserProfile", "ContactedMspCountLastMonth");
            DropColumn("dbo.UserProfile", "ContactedPeopleCountLastMonth");
            DropColumn("dbo.UserProfile", "PremiumPartnersQuotaLastMonth");
            DropColumn("dbo.UserProfile", "RegistredPeopleQuotaLastMonth");
        }
    }
}
