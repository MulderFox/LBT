namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpgradeV21021 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Currency",
                c => new
                    {
                        CurrencyId = c.Int(nullable: false, identity: true),
                        CurrencyType = c.Int(nullable: false),
                        ExchangeRateToCZK = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.CurrencyId);
            
            AddColumn("dbo.UserProfile", "ClaAccessAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.UserProfile", "ClaAccessYearlyAccessCZK", c => c.Int(nullable: false));
            AddColumn("dbo.UserProfile", "ClaAccessYearlyAccessEUR", c => c.Int(nullable: false));
            AddColumn("dbo.UserProfile", "ClaAccessYearlyAccessUSD", c => c.Int(nullable: false));
            AddColumn("dbo.UserProfile", "ClaAccessCurrency", c => c.Int(nullable: false));
            AddColumn("dbo.UserProfile", "ClaAccessTrial", c => c.Boolean(nullable: false));
            AddColumn("dbo.BankAccount", "Title", c => c.String(nullable: false));
            AddColumn("dbo.BankAccount", "Owner", c => c.String(nullable: false));
            AddColumn("dbo.BankAccount", "CurrencyType", c => c.Int(nullable: false));
            AddColumn("dbo.Meeting", "BankAccountId", c => c.Int());
            AlterColumn("dbo.BankAccount", "Token", c => c.String(nullable: false, maxLength: 128));
            AddForeignKey("dbo.Meeting", "BankAccountId", "dbo.BankAccount", "BankAccountId");
            CreateIndex("dbo.Meeting", "BankAccountId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Meeting", new[] { "BankAccountId" });
            DropForeignKey("dbo.Meeting", "BankAccountId", "dbo.BankAccount");
            AlterColumn("dbo.BankAccount", "Token", c => c.String(maxLength: 128));
            DropColumn("dbo.Meeting", "BankAccountId");
            DropColumn("dbo.BankAccount", "CurrencyType");
            DropColumn("dbo.BankAccount", "Owner");
            DropColumn("dbo.BankAccount", "Title");
            DropColumn("dbo.UserProfile", "ClaAccessTrial");
            DropColumn("dbo.UserProfile", "ClaAccessCurrency");
            DropColumn("dbo.UserProfile", "ClaAccessYearlyAccessUSD");
            DropColumn("dbo.UserProfile", "ClaAccessYearlyAccessEUR");
            DropColumn("dbo.UserProfile", "ClaAccessYearlyAccessCZK");
            DropColumn("dbo.UserProfile", "ClaAccessAmount");
            DropTable("dbo.Currency");
        }
    }
}
