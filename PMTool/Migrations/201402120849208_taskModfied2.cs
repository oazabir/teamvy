namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class taskModfied2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tasks", "ProjectColumn_ProjectColumnID", c => c.Long());
            CreateIndex("dbo.Tasks", "ProjectColumn_ProjectColumnID");
            AddForeignKey("dbo.Tasks", "ProjectColumn_ProjectColumnID", "dbo.ProjectColumns", "ProjectColumnID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tasks", "ProjectColumn_ProjectColumnID", "dbo.ProjectColumns");
            DropIndex("dbo.Tasks", new[] { "ProjectColumn_ProjectColumnID" });
            DropColumn("dbo.Tasks", "ProjectColumn_ProjectColumnID");
        }
    }
}
