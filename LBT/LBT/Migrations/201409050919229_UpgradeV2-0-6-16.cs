namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpgradeV20616 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BankAccount", "AccountId", c => c.String(nullable: false, defaultValue: "Neexistující úèet"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BankAccount", "AccountId");
        }
    }
}
