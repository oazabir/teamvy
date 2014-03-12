namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ruleChange3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProjectStatusRules", "ProjectStatus_ProjectStatusID", "dbo.ProjectStatus");
            DropIndex("dbo.ProjectStatusRules", new[] { "ProjectStatus_ProjectStatusID" });
            RenameColumn(table: "dbo.ProjectStatusRules", name: "ProjectStatus_ProjectStatusID", newName: "ProjectStatusID");
            AlterColumn("dbo.ProjectStatusRules", "ProjectStatusID", c => c.Long(nullable: false));
            CreateIndex("dbo.ProjectStatusRules", "ProjectStatusID");
            AddForeignKey("dbo.ProjectStatusRules", "ProjectStatusID", "dbo.ProjectStatus", "ProjectStatusID");
            DropColumn("dbo.ProjectStatusRules", "StatusID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProjectStatusRules", "StatusID", c => c.Long(nullable: false));
            DropForeignKey("dbo.ProjectStatusRules", "ProjectStatusID", "dbo.ProjectStatus");
            DropIndex("dbo.ProjectStatusRules", new[] { "ProjectStatusID" });
            AlterColumn("dbo.ProjectStatusRules", "ProjectStatusID", c => c.Long());
            RenameColumn(table: "dbo.ProjectStatusRules", name: "ProjectStatusID", newName: "ProjectStatus_ProjectStatusID");
            CreateIndex("dbo.ProjectStatusRules", "ProjectStatus_ProjectStatusID");
            AddForeignKey("dbo.ProjectStatusRules", "ProjectStatus_ProjectStatusID", "dbo.ProjectStatus", "ProjectStatusID");
        }
    }
}
