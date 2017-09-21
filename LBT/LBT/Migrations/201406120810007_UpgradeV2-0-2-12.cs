namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpgradeV20212 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TopTen", "ShortTermCareerGoal", c => c.String());
            AlterColumn("dbo.TopTen", "LongTermCareerGoal", c => c.String());
            AlterColumn("dbo.TopTen", "ActualCareerStage", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TopTen", "ActualCareerStage", c => c.String(maxLength: 5));
            AlterColumn("dbo.TopTen", "LongTermCareerGoal", c => c.String(maxLength: 90));
            AlterColumn("dbo.TopTen", "ShortTermCareerGoal", c => c.String(maxLength: 90));
        }
    }
}
