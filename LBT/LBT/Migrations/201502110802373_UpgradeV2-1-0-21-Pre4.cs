using LBT.Models;

namespace LBT.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpgradeV21021Pre4 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Team",
                c => new
                    {
                        TeamId = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        OwnerId = c.Int(nullable: false),
                        Note = c.String(),
                        Task = c.String(),
                    })
                .PrimaryKey(t => t.TeamId)
                .ForeignKey("dbo.UserProfile", t => t.OwnerId)
                .Index(t => t.OwnerId);
            
            CreateTable(
                "dbo.TeamMember",
                c => new
                    {
                        TeamMemberId = c.Int(nullable: false, identity: true),
                        TeamId = c.Int(nullable: false),
                        MemberId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TeamMemberId)
                .ForeignKey("dbo.Team", t => t.TeamId)
                .ForeignKey("dbo.UserProfile", t => t.MemberId)
                .Index(t => t.TeamId)
                .Index(t => t.MemberId);

            AddColumn("dbo.MeetingTitleType", "MeetingType", c => c.Int(nullable: false, defaultValue: (int)MeetingType.Ostatni));
        }
        
        public override void Down()
        {
            DropIndex("dbo.TeamMember", new[] { "MemberId" });
            DropIndex("dbo.TeamMember", new[] { "TeamId" });
            DropIndex("dbo.Team", new[] { "OwnerId" });
            DropForeignKey("dbo.TeamMember", "MemberId", "dbo.UserProfile");
            DropForeignKey("dbo.TeamMember", "TeamId", "dbo.Team");
            DropForeignKey("dbo.Team", "OwnerId", "dbo.UserProfile");
            DropColumn("dbo.MeetingTitleType", "MeetingType");
            DropTable("dbo.TeamMember");
            DropTable("dbo.Team");
        }
    }
}
