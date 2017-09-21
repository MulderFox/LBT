namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpgradeV1077 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PeopleContact", "WorkflowStatePrevious", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PeopleContact", "WorkflowStatePrevious");
        }
    }
}
