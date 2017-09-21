namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpgradeV21930 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Video", "EmailSubject", c => c.String(nullable: false));
            AddColumn("dbo.Video", "EmailBody", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Video", "EmailBody");
            DropColumn("dbo.Video", "EmailSubject");
        }
    }
}
