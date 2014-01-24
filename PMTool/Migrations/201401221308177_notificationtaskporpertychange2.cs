namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class notificationtaskporpertychange2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Notifications", "TaskID", "dbo.Tasks");
            DropIndex("dbo.Notifications", new[] { "TaskID" });
            AlterColumn("dbo.Notifications", "TaskID", c => c.Long());
            CreateIndex("dbo.Notifications", "TaskID");
            AddForeignKey("dbo.Notifications", "TaskID", "dbo.Tasks", "TaskID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notifications", "TaskID", "dbo.Tasks");
            DropIndex("dbo.Notifications", new[] { "TaskID" });
            AlterColumn("dbo.Notifications", "TaskID", c => c.Long(nullable: false));
            CreateIndex("dbo.Notifications", "TaskID");
            AddForeignKey("dbo.Notifications", "TaskID", "dbo.Tasks", "TaskID");
        }
    }
}
