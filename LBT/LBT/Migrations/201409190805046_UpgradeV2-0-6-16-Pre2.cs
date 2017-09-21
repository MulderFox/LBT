namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpgradeV20616Pre2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BankAccount", "Token", c => c.String(maxLength: 128));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BankAccount", "Token", c => c.String(nullable: false, maxLength: 128));
        }
    }
}
