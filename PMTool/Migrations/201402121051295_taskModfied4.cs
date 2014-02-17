namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class taskModfied4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tasks", "ProjectColumnID", "dbo.ProjectColumns");
            DropIndex("dbo.Tasks", new[] { "ProjectColumnID" });
            AlterColumn("dbo.Tasks", "ProjectColumnID", c => c.Long());
            CreateIndex("dbo.Tasks", "ProjectColumnID");
            AddForeignKey("dbo.Tasks", "ProjectColumnID", "dbo.ProjectColumns", "ProjectColumnID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tasks", "ProjectColumnID", "dbo.ProjectColumns");
            DropIndex("dbo.Tasks", new[] { "ProjectColumnID" });
            AlterColumn("dbo.Tasks", "ProjectColumnID", c => c.Long(nullable: false));
            CreateIndex("dbo.Tasks", "ProjectColumnID");
            AddForeignKey("dbo.Tasks", "ProjectColumnID", "dbo.ProjectColumns", "ProjectColumnID");
        }
    }
}
