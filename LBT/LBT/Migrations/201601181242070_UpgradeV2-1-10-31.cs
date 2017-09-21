namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpgradeV211031 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LazyMail",
                c => new
                    {
                        LazyMailId = c.Int(nullable: false, identity: true),
                        Address = c.String(nullable: false),
                        TextBody = c.String(nullable: false),
                        TimeToSend = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.LazyMailId);
            
            AddColumn("dbo.Video", "EmailSenderBody", c => c.String(nullable: false));
            AddColumn("dbo.Video", "AllUsers", c => c.Boolean(nullable: false));
            AddColumn("dbo.Video", "Duration", c => c.Int(nullable: false));
            AddColumn("dbo.VideoToken", "SenderId", c => c.Int());
            AddColumn("dbo.VideoToken", "RecipientId", c => c.Int());
            AddColumn("dbo.VideoToken", "IsPlayedByRecepient", c => c.Boolean(nullable: false));
            AddForeignKey("dbo.VideoToken", "SenderId", "dbo.UserProfile", "UserId");
            AddForeignKey("dbo.VideoToken", "RecipientId", "dbo.PeopleContact", "PeopleContactId");
            CreateIndex("dbo.VideoToken", "SenderId");
            CreateIndex("dbo.VideoToken", "RecipientId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.VideoToken", new[] { "RecipientId" });
            DropIndex("dbo.VideoToken", new[] { "SenderId" });
            DropForeignKey("dbo.VideoToken", "RecipientId", "dbo.PeopleContact");
            DropForeignKey("dbo.VideoToken", "SenderId", "dbo.UserProfile");
            DropColumn("dbo.VideoToken", "IsPlayedByRecepient");
            DropColumn("dbo.VideoToken", "RecipientId");
            DropColumn("dbo.VideoToken", "SenderId");
            DropColumn("dbo.Video", "Duration");
            DropColumn("dbo.Video", "AllUsers");
            DropColumn("dbo.Video", "EmailSenderBody");
            DropTable("dbo.LazyMail");
        }
    }
}
