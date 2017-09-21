namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpgradeV20110 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TopTen",
                c => new
                    {
                        TopTenId = c.Int(nullable: false, identity: true),
                        FromUserId = c.Int(nullable: false),
                        ToUserId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Role = c.Int(),
                        ShortTermCareerGoal = c.String(maxLength: 90),
                        LongTermCareerGoal = c.String(maxLength: 90),
                        TrainingPhone = c.Boolean(nullable: false),
                        TrainingVideo = c.Boolean(nullable: false),
                        TrainingBiPresentation = c.Boolean(nullable: false),
                        TrainingStLecture = c.Boolean(nullable: false),
                        Od1 = c.Boolean(nullable: false),
                        Od2 = c.Boolean(nullable: false),
                        Rhetoric = c.Boolean(nullable: false),
                        TeamLeadershipping = c.Boolean(nullable: false),
                        ConductingMeetings = c.Boolean(nullable: false),
                        Sns = c.Boolean(nullable: false),
                        AdditionalTraining = c.Boolean(nullable: false),
                        ActualCareerStage = c.String(maxLength: 5),
                        LastContact = c.DateTime(nullable: false),
                        Note = c.String(),
                        Tasks = c.String(),
                    })
                .PrimaryKey(t => t.TopTenId)
                .ForeignKey("dbo.UserProfile", t => t.FromUserId)
                .ForeignKey("dbo.UserProfile", t => t.ToUserId)
                .Index(t => t.FromUserId)
                .Index(t => t.ToUserId);
            
            AddColumn("dbo.UserProfile", "ClaAccessExpired", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropIndex("dbo.TopTen", new[] { "ToUserId" });
            DropIndex("dbo.TopTen", new[] { "FromUserId" });
            DropForeignKey("dbo.TopTen", "ToUserId", "dbo.UserProfile");
            DropForeignKey("dbo.TopTen", "FromUserId", "dbo.UserProfile");
            DropColumn("dbo.UserProfile", "ClaAccessExpired");
            DropTable("dbo.TopTen");
        }
    }
}
