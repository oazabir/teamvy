namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class notificationtaskporpertychange : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Notifications", "User_UserId", "dbo.Users");
            DropIndex("dbo.Notifications", new[] { "User_UserId" });
            DropColumn("dbo.Notifications", "UserID");
            RenameColumn(table: "dbo.Notifications", name: "User_UserId", newName: "UserID");
            AlterColumn("dbo.Notifications", "UserID", c => c.Guid(nullable: false));
            AlterColumn("dbo.Notifications", "UserID", c => c.Guid(nullable: false));
            CreateIndex("dbo.Notifications", "UserID");
            AddForeignKey("dbo.Notifications", "UserID", "dbo.Users", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notifications", "UserID", "dbo.Users");
            DropIndex("dbo.Notifications", new[] { "UserID" });
            AlterColumn("dbo.Notifications", "UserID", c => c.Guid());
            AlterColumn("dbo.Notifications", "UserID", c => c.Long(nullable: false));
            RenameColumn(table: "dbo.Notifications", name: "UserID", newName: "User_UserId");
            AddColumn("dbo.Notifications", "UserID", c => c.Long(nullable: false));
            CreateIndex("dbo.Notifications", "User_UserId");
            AddForeignKey("dbo.Notifications", "User_UserId", "dbo.Users", "UserId");
        }
    }
}
