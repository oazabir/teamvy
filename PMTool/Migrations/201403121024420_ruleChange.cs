namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ruleChange : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProjectStatusRules", "FromStatus_ProjectStatusID", "dbo.ProjectStatus");
            DropForeignKey("dbo.ProjectStatusRules", "FromTo_ProjectStatusID", "dbo.ProjectStatus");
            DropIndex("dbo.ProjectStatusRules", new[] { "FromStatus_ProjectStatusID" });
            DropIndex("dbo.ProjectStatusRules", new[] { "FromTo_ProjectStatusID" });
            AddColumn("dbo.ProjectStatusRules", "DateName", c => c.DateTime(nullable: false));
            AddColumn("dbo.ProjectStatusRules", "StatusID", c => c.Long(nullable: false));
            AddColumn("dbo.ProjectStatusRules", "ProjectStatus_ProjectStatusID", c => c.Long());
            CreateIndex("dbo.ProjectStatusRules", "ProjectStatus_ProjectStatusID");
            AddForeignKey("dbo.ProjectStatusRules", "ProjectStatus_ProjectStatusID", "dbo.ProjectStatus", "ProjectStatusID");
            DropColumn("dbo.ProjectStatusRules", "FromDateName");
            DropColumn("dbo.ProjectStatusRules", "ToDateName");
            DropColumn("dbo.ProjectStatusRules", "FromStatusID");
            DropColumn("dbo.ProjectStatusRules", "ToStatusID");
            DropColumn("dbo.ProjectStatusRules", "FromStatus_ProjectStatusID");
            DropColumn("dbo.ProjectStatusRules", "FromTo_ProjectStatusID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProjectStatusRules", "FromTo_ProjectStatusID", c => c.Long());
            AddColumn("dbo.ProjectStatusRules", "FromStatus_ProjectStatusID", c => c.Long());
            AddColumn("dbo.ProjectStatusRules", "ToStatusID", c => c.Long(nullable: false));
            AddColumn("dbo.ProjectStatusRules", "FromStatusID", c => c.Long(nullable: false));
            AddColumn("dbo.ProjectStatusRules", "ToDateName", c => c.DateTime(nullable: false));
            AddColumn("dbo.ProjectStatusRules", "FromDateName", c => c.DateTime(nullable: false));
            DropForeignKey("dbo.ProjectStatusRules", "ProjectStatus_ProjectStatusID", "dbo.ProjectStatus");
            DropIndex("dbo.ProjectStatusRules", new[] { "ProjectStatus_ProjectStatusID" });
            DropColumn("dbo.ProjectStatusRules", "ProjectStatus_ProjectStatusID");
            DropColumn("dbo.ProjectStatusRules", "StatusID");
            DropColumn("dbo.ProjectStatusRules", "DateName");
            CreateIndex("dbo.ProjectStatusRules", "FromTo_ProjectStatusID");
            CreateIndex("dbo.ProjectStatusRules", "FromStatus_ProjectStatusID");
            AddForeignKey("dbo.ProjectStatusRules", "FromTo_ProjectStatusID", "dbo.ProjectStatus", "ProjectStatusID");
            AddForeignKey("dbo.ProjectStatusRules", "FromStatus_ProjectStatusID", "dbo.ProjectStatus", "ProjectStatusID");
        }
    }
}
