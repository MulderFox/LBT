namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpgradeV21728 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PropertiesBag",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 128),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Key);
            
            AddColumn("dbo.UserProfile", "Address", c => c.String());
            AddColumn("dbo.UserProfile", "PSC", c => c.String());
            AddColumn("dbo.UserProfile", "DIC", c => c.String());
            AddColumn("dbo.UserProfile", "ICO", c => c.String());
            AddColumn("dbo.BankAccount", "IBAN", c => c.String(nullable: false));
            AddColumn("dbo.BankAccount", "SWIFT", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BankAccount", "SWIFT");
            DropColumn("dbo.BankAccount", "IBAN");
            DropColumn("dbo.UserProfile", "ICO");
            DropColumn("dbo.UserProfile", "DIC");
            DropColumn("dbo.UserProfile", "PSC");
            DropColumn("dbo.UserProfile", "Address");
            DropTable("dbo.PropertiesBag");
        }
    }
}
