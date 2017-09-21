namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpgradeV211233 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Manual", "Order", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Manual", "Order");
        }
    }
}
