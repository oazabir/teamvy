namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class timelogchange : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TimeLogs", "User_UserId", "dbo.Users");
            DropIndex("dbo.TimeLogs", new[] { "User_UserId" });
            DropColumn("dbo.TimeLogs", "UserID");
            RenameColumn(table: "dbo.TimeLogs", name: "User_UserId", newName: "UserID");
            AlterColumn("dbo.TimeLogs", "UserID", c => c.Guid(nullable: false));
            CreateIndex("dbo.TimeLogs", "UserID");
            AddForeignKey("dbo.TimeLogs", "UserID", "dbo.Users", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TimeLogs", "UserID", "dbo.Users");
            DropIndex("dbo.TimeLogs", new[] { "UserID" });
            AlterColumn("dbo.TimeLogs", "UserID", c => c.Guid());
            RenameColumn(table: "dbo.TimeLogs", name: "UserID", newName: "User_UserId");
            AddColumn("dbo.TimeLogs", "UserID", c => c.Guid(nullable: false));
            CreateIndex("dbo.TimeLogs", "User_UserId");
            AddForeignKey("dbo.TimeLogs", "User_UserId", "dbo.Users", "UserId");
        }
    }
}
