namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sprintadded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Sprints",
                c => new
                    {
                        SprintID = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        ProjectID = c.Long(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.SprintID)
                .ForeignKey("dbo.Projects", t => t.ProjectID)
                .Index(t => t.ProjectID);
            
            AddColumn("dbo.Tasks", "SprintID", c => c.Long());
            CreateIndex("dbo.Tasks", "SprintID");
            AddForeignKey("dbo.Tasks", "SprintID", "dbo.Sprints", "SprintID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tasks", "SprintID", "dbo.Sprints");
            DropForeignKey("dbo.Sprints", "ProjectID", "dbo.Projects");
            DropIndex("dbo.Tasks", new[] { "SprintID" });
            DropIndex("dbo.Sprints", new[] { "ProjectID" });
            DropColumn("dbo.Tasks", "SprintID");
            DropTable("dbo.Sprints");
        }
    }
}
