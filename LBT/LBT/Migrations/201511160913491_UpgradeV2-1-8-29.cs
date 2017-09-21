namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpgradeV21829 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Video",
                c => new
                    {
                        VideoId = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        RelativeFilePath = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.VideoId);
            
            CreateTable(
                "dbo.VideoUser",
                c => new
                    {
                        VideoUserId = c.Int(nullable: false, identity: true),
                        VideoId = c.Int(nullable: false),
                        UserProfileId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.VideoUserId)
                .ForeignKey("dbo.Video", t => t.VideoId)
                .ForeignKey("dbo.UserProfile", t => t.UserProfileId)
                .Index(t => t.VideoId)
                .Index(t => t.UserProfileId);
            
            CreateTable(
                "dbo.VideoToken",
                c => new
                    {
                        VideoTokenId = c.Int(nullable: false, identity: true),
                        VideoId = c.Int(nullable: false),
                        Expired = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.VideoTokenId)
                .ForeignKey("dbo.Video", t => t.VideoId)
                .Index(t => t.VideoId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.VideoToken", new[] { "VideoId" });
            DropIndex("dbo.VideoUser", new[] { "UserProfileId" });
            DropIndex("dbo.VideoUser", new[] { "VideoId" });
            DropForeignKey("dbo.VideoToken", "VideoId", "dbo.Video");
            DropForeignKey("dbo.VideoUser", "UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.VideoUser", "VideoId", "dbo.Video");
            DropTable("dbo.VideoToken");
            DropTable("dbo.VideoUser");
            DropTable("dbo.Video");
        }
    }
}
