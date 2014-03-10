namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class activitylogchange : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.TaskActivityLogs", "TaskID");
            AddForeignKey("dbo.TaskActivityLogs", "TaskID", "dbo.Tasks", "TaskID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TaskActivityLogs", "TaskID", "dbo.Tasks");
            DropIndex("dbo.TaskActivityLogs", new[] { "TaskID" });
        }
    }
}
