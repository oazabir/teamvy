namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class taskModfied3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tasks", "ProjectColumn_ProjectColumnID", "dbo.ProjectColumns");
            DropIndex("dbo.Tasks", new[] { "ProjectColumn_ProjectColumnID" });
            RenameColumn(table: "dbo.Tasks", name: "ProjectColumn_ProjectColumnID", newName: "ProjectColumnID");
            AlterColumn("dbo.Tasks", "ProjectColumnID", c => c.Long(nullable: false));
            CreateIndex("dbo.Tasks", "ProjectColumnID");
            AddForeignKey("dbo.Tasks", "ProjectColumnID", "dbo.ProjectColumns", "ProjectColumnID");
            DropColumn("dbo.Tasks", "CurrentStatus");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tasks", "CurrentStatus", c => c.Long(nullable: false));
            DropForeignKey("dbo.Tasks", "ProjectColumnID", "dbo.ProjectColumns");
            DropIndex("dbo.Tasks", new[] { "ProjectColumnID" });
            AlterColumn("dbo.Tasks", "ProjectColumnID", c => c.Long());
            RenameColumn(table: "dbo.Tasks", name: "ProjectColumnID", newName: "ProjectColumn_ProjectColumnID");
            CreateIndex("dbo.Tasks", "ProjectColumn_ProjectColumnID");
            AddForeignKey("dbo.Tasks", "ProjectColumn_ProjectColumnID", "dbo.ProjectColumns", "ProjectColumnID");
        }
    }
}
