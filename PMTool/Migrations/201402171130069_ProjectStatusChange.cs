namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProjectStatusChange : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProjectColumns", "ProjectID", "dbo.Projects");
            DropForeignKey("dbo.Tasks", "ProjectColumnID", "dbo.ProjectColumns");
            DropIndex("dbo.ProjectColumns", new[] { "ProjectID" });
            DropIndex("dbo.Tasks", new[] { "ProjectColumnID" });
            CreateTable(
                "dbo.ProjectStatus",
                c => new
                    {
                        ProjectStatusID = c.Long(nullable: false, identity: true),
                        ProjectID = c.Long(nullable: false),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectStatusID)
                .ForeignKey("dbo.Projects", t => t.ProjectID)
                .Index(t => t.ProjectID);
            
            AddColumn("dbo.Tasks", "ProjectStatusID", c => c.Long());
            CreateIndex("dbo.Tasks", "ProjectStatusID");
            AddForeignKey("dbo.Tasks", "ProjectStatusID", "dbo.ProjectStatus", "ProjectStatusID");
            DropColumn("dbo.Tasks", "ProjectColumnID");
            DropTable("dbo.ProjectColumns");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ProjectColumns",
                c => new
                    {
                        ProjectColumnID = c.Long(nullable: false, identity: true),
                        ProjectID = c.Long(nullable: false),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectColumnID);
            
            AddColumn("dbo.Tasks", "ProjectColumnID", c => c.Long());
            DropForeignKey("dbo.Tasks", "ProjectStatusID", "dbo.ProjectStatus");
            DropForeignKey("dbo.ProjectStatus", "ProjectID", "dbo.Projects");
            DropIndex("dbo.Tasks", new[] { "ProjectStatusID" });
            DropIndex("dbo.ProjectStatus", new[] { "ProjectID" });
            DropColumn("dbo.Tasks", "ProjectStatusID");
            DropTable("dbo.ProjectStatus");
            CreateIndex("dbo.Tasks", "ProjectColumnID");
            CreateIndex("dbo.ProjectColumns", "ProjectID");
            AddForeignKey("dbo.Tasks", "ProjectColumnID", "dbo.ProjectColumns", "ProjectColumnID");
            AddForeignKey("dbo.ProjectColumns", "ProjectID", "dbo.Projects", "ProjectID");
        }
    }
}
