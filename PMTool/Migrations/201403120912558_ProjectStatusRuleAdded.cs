namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProjectStatusRuleAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProjectStatusRules",
                c => new
                    {
                        ProjectStatusRuleID = c.Long(nullable: false, identity: true),
                        ProjectID = c.Long(nullable: false),
                        FromDateName = c.DateTime(nullable: false),
                        ToDateName = c.DateTime(nullable: false),
                        FromStatusID = c.Long(nullable: false),
                        ToStatusID = c.Long(nullable: false),
                        DateMaper = c.Int(nullable: false),
                        FromStatus_ProjectStatusID = c.Long(),
                        FromTo_ProjectStatusID = c.Long(),
                    })
                .PrimaryKey(t => t.ProjectStatusRuleID)
                .ForeignKey("dbo.ProjectStatus", t => t.FromStatus_ProjectStatusID)
                .ForeignKey("dbo.ProjectStatus", t => t.FromTo_ProjectStatusID)
                .ForeignKey("dbo.Projects", t => t.ProjectID)
                .Index(t => t.FromStatus_ProjectStatusID)
                .Index(t => t.FromTo_ProjectStatusID)
                .Index(t => t.ProjectID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProjectStatusRules", "ProjectID", "dbo.Projects");
            DropForeignKey("dbo.ProjectStatusRules", "FromTo_ProjectStatusID", "dbo.ProjectStatus");
            DropForeignKey("dbo.ProjectStatusRules", "FromStatus_ProjectStatusID", "dbo.ProjectStatus");
            DropIndex("dbo.ProjectStatusRules", new[] { "ProjectID" });
            DropIndex("dbo.ProjectStatusRules", new[] { "FromTo_ProjectStatusID" });
            DropIndex("dbo.ProjectStatusRules", new[] { "FromStatus_ProjectStatusID" });
            DropTable("dbo.ProjectStatusRules");
        }
    }
}
