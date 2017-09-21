namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpgradeV21021Pre5 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BankAccountUser",
                c => new
                    {
                        BankAccountUserId = c.Int(nullable: false, identity: true),
                        BankAccountId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BankAccountUserId)
                .ForeignKey("dbo.BankAccount", t => t.BankAccountId)
                .ForeignKey("dbo.UserProfile", t => t.UserId)
                .Index(t => t.BankAccountId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.BankAccountUser", new[] { "UserId" });
            DropIndex("dbo.BankAccountUser", new[] { "BankAccountId" });
            DropForeignKey("dbo.BankAccountUser", "UserId", "dbo.UserProfile");
            DropForeignKey("dbo.BankAccountUser", "BankAccountId", "dbo.BankAccount");
            DropTable("dbo.BankAccountUser");
        }
    }
}
