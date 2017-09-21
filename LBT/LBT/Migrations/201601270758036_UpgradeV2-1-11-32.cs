namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpgradeV211132 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ManualType",
                c => new
                    {
                        ManualTypeId = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 128),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ManualTypeId);
            
            CreateTable(
                "dbo.Manual",
                c => new
                    {
                        ManualId = c.Int(nullable: false, identity: true),
                        ManualTypeId = c.Int(nullable: false),
                        Title = c.String(nullable: false, maxLength: 128),
                        RelativeFilePath = c.String(nullable: false),
                        IsDownloadable = c.Boolean(nullable: false),
                        IsAccessForAuthGuest = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ManualId)
                .ForeignKey("dbo.ManualType", t => t.ManualTypeId)
                .Index(t => t.ManualTypeId);
            
            AddColumn("dbo.VideoToken", "IsPlayedByRecipient", c => c.Boolean(nullable: false));
            DropColumn("dbo.VideoToken", "IsPlayedByRecepient");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VideoToken", "IsPlayedByRecepient", c => c.Boolean(nullable: false));
            DropIndex("dbo.Manual", new[] { "ManualTypeId" });
            DropForeignKey("dbo.Manual", "ManualTypeId", "dbo.ManualType");
            DropColumn("dbo.VideoToken", "IsPlayedByRecipient");
            DropTable("dbo.Manual");
            DropTable("dbo.ManualType");
        }
    }
}
