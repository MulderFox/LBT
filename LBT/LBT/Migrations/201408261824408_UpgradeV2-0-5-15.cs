namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpgradeV20515 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MeetingTitleType",
                c => new
                    {
                        MeetingTitleTypeId = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.MeetingTitleTypeId);
            
            AddColumn("dbo.Meeting", "MeetingTitleTypeId", c => c.Int());
            AddForeignKey("dbo.Meeting", "MeetingTitleTypeId", "dbo.MeetingTitleType", "MeetingTitleTypeId");
            CreateIndex("dbo.Meeting", "MeetingTitleTypeId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Meeting", new[] { "MeetingTitleTypeId" });
            DropForeignKey("dbo.Meeting", "MeetingTitleTypeId", "dbo.MeetingTitleType");
            DropColumn("dbo.Meeting", "MeetingTitleTypeId");
            DropTable("dbo.MeetingTitleType");
        }
    }
}
