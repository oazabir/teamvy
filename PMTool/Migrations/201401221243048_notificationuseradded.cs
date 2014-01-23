namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class notificationuseradded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notifications", "UserID", c => c.Long(nullable: false));
            AddColumn("dbo.Notifications", "User_UserId", c => c.Guid());
            CreateIndex("dbo.Notifications", "User_UserId");
            AddForeignKey("dbo.Notifications", "User_UserId", "dbo.Users", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notifications", "User_UserId", "dbo.Users");
            DropIndex("dbo.Notifications", new[] { "User_UserId" });
            DropColumn("dbo.Notifications", "User_UserId");
            DropColumn("dbo.Notifications", "UserID");
        }
    }
}
