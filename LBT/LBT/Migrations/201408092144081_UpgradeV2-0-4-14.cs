namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpgradeV20414 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PhoneNumberPrefix", "MatchRegex", c => c.String(maxLength: 128));
            AddColumn("dbo.PhoneNumberPrefix", "ReplaceRegex", c => c.String(maxLength: 128));
            AddColumn("dbo.PhoneNumberPrefix", "ExportablePrefix", c => c.String(maxLength: 40));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PhoneNumberPrefix", "ExportablePrefix");
            DropColumn("dbo.PhoneNumberPrefix", "ReplaceRegex");
            DropColumn("dbo.PhoneNumberPrefix", "MatchRegex");
        }
    }
}
