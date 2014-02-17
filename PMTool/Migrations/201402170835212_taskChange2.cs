namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class taskChange2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tasks", "ParentTask_TaskID", "dbo.Tasks");
            DropIndex("dbo.Tasks", new[] { "ParentTask_TaskID" });
            DropColumn("dbo.Tasks", "ParentTaskId");
            RenameColumn(table: "dbo.Tasks", name: "ParentTask_TaskID", newName: "ParentTaskId");
            CreateIndex("dbo.Tasks", "ParentTaskId");
            AddForeignKey("dbo.Tasks", "ParentTaskId", "dbo.Tasks", "TaskID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tasks", "ParentTaskId", "dbo.Tasks");
            DropIndex("dbo.Tasks", new[] { "ParentTaskId" });
            RenameColumn(table: "dbo.Tasks", name: "ParentTaskId", newName: "ParentTask_TaskID");
            AddColumn("dbo.Tasks", "ParentTaskId", c => c.Long());
            CreateIndex("dbo.Tasks", "ParentTask_TaskID");
            AddForeignKey("dbo.Tasks", "ParentTask_TaskID", "dbo.Tasks", "TaskID");
        }
    }
}
