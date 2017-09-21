namespace LBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Upgrade : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SharedContact",
                c => new
                    {
                        SharedContactId = c.Int(nullable: false, identity: true),
                        FromUserId = c.Int(nullable: false),
                        ToUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SharedContactId)
                .ForeignKey("dbo.UserProfile", t => t.FromUserId)
                .ForeignKey("dbo.UserProfile", t => t.ToUserId)
                .Index(t => t.FromUserId)
                .Index(t => t.ToUserId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.SharedContact", new[] { "ToUserId" });
            DropIndex("dbo.SharedContact", new[] { "FromUserId" });
            DropForeignKey("dbo.SharedContact", "ToUserId", "dbo.UserProfile");
            DropForeignKey("dbo.SharedContact", "FromUserId", "dbo.UserProfile");
            DropTable("dbo.SharedContact");
        }
    }
}
