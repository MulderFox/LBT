namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Upgradev20616Pre3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Meeting", "BankAccountId", "dbo.BankAccount");
            DropIndex("dbo.Meeting", new[] { "BankAccountId" });
            DropColumn("dbo.Meeting", "BankAccountId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Meeting", "BankAccountId", c => c.Int());
            CreateIndex("dbo.Meeting", "BankAccountId");
            AddForeignKey("dbo.Meeting", "BankAccountId", "dbo.BankAccount", "BankAccountId");
        }
    }
}
