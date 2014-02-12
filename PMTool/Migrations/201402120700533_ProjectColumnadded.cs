namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProjectColumnadded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProjectColumns",
                c => new
                    {
                        ProjectColumnID = c.Long(nullable: false, identity: true),
                        ProjectID = c.Long(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ProjectColumnID)
                .ForeignKey("dbo.Projects", t => t.ProjectID)
                .Index(t => t.ProjectID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProjectColumns", "ProjectID", "dbo.Projects");
            DropIndex("dbo.ProjectColumns", new[] { "ProjectID" });
            DropTable("dbo.ProjectColumns");
        }
    }
}
