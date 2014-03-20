namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrateforrelase : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TimeLogs", "SprintID", c => c.Long(nullable: false));
            CreateIndex("dbo.TimeLogs", "SprintID");
            AddForeignKey("dbo.TimeLogs", "SprintID", "dbo.Sprints", "SprintID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TimeLogs", "SprintID", "dbo.Sprints");
            DropIndex("dbo.TimeLogs", new[] { "SprintID" });
            DropColumn("dbo.TimeLogs", "SprintID");
        }
    }
}
