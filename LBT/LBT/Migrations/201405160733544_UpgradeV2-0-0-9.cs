namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpgradeV2009 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BankAccount",
                c => new
                    {
                        BankAccountId = c.Int(nullable: false, identity: true),
                        BankAccountType = c.Int(nullable: false),
                        Token = c.String(nullable: false, maxLength: 128),
                        ValidTo = c.DateTime(nullable: false),
                        TransactionStartDate = c.DateTime(nullable: false),
                        LastDownloadId = c.Long(),
                    })
                .PrimaryKey(t => t.BankAccountId);
            
            CreateTable(
                "dbo.BankAccountHistory",
                c => new
                    {
                        BankAccountHistoryId = c.Int(nullable: false, identity: true),
                        BankAccountId = c.Int(nullable: false),
                        TransactionId = c.Long(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Ammount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Currency = c.String(maxLength: 3),
                        Exchange = c.String(maxLength: 255),
                        BankCode = c.String(maxLength: 10),
                        BankName = c.String(maxLength: 255),
                        Ks = c.Int(),
                        Vs = c.Long(),
                        Ss = c.Long(),
                    })
                .PrimaryKey(t => t.BankAccountHistoryId)
                .ForeignKey("dbo.BankAccount", t => t.BankAccountId)
                .Index(t => t.BankAccountId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.BankAccountHistory", new[] { "BankAccountId" });
            DropForeignKey("dbo.BankAccountHistory", "BankAccountId", "dbo.BankAccount");
            DropTable("dbo.BankAccountHistory");
            DropTable("dbo.BankAccount");
        }
    }
}
