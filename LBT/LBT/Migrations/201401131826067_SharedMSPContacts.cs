namespace LBT.Migrations
{
    using System.Data.Entity.Migrations;

    // ReSharper disable InconsistentNaming
    public partial class SharedMSPContacts : DbMigration
    // ReSharper restore InconsistentNaming
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SharedMSPContact",
                c => new
                    {
                        SharedMSPContactId = c.Int(nullable: false, identity: true),
                        FromUserId = c.Int(nullable: false),
                        ToUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SharedMSPContactId)
                .ForeignKey("dbo.UserProfile", t => t.FromUserId)
                .ForeignKey("dbo.UserProfile", t => t.ToUserId)
                .Index(t => t.FromUserId)
                .Index(t => t.ToUserId);
        }

        public override void Down()
        {
            DropIndex("dbo.SharedMSPContact", new[] { "ToUserId" });
            DropIndex("dbo.SharedMSPContact", new[] { "FromUserId" });
            DropForeignKey("dbo.SharedMSPContact", "ToUserId", "dbo.UserProfile");
            DropForeignKey("dbo.SharedMSPContact", "FromUserId", "dbo.UserProfile");
            DropTable("dbo.SharedMSPContact");
        }
    }
}
