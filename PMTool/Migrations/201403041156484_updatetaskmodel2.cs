namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatetaskmodel2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tasks", "PriorityID", "dbo.Priorities");
            DropIndex("dbo.Tasks", new[] { "PriorityID" });
            AlterColumn("dbo.Tasks", "PriorityID", c => c.Int());
            CreateIndex("dbo.Tasks", "PriorityID");
            AddForeignKey("dbo.Tasks", "PriorityID", "dbo.Priorities", "PriorityID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tasks", "PriorityID", "dbo.Priorities");
            DropIndex("dbo.Tasks", new[] { "PriorityID" });
            AlterColumn("dbo.Tasks", "PriorityID", c => c.Int(nullable: false));
            CreateIndex("dbo.Tasks", "PriorityID");
            AddForeignKey("dbo.Tasks", "PriorityID", "dbo.Priorities", "PriorityID");
        }
    }
}
