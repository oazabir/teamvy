namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProjectOwners1 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.ProjectOwners", name: "UserId", newName: "__mig_tmp__0");
            RenameColumn(table: "dbo.ProjectOwners", name: "ProjectID", newName: "UserId");
            RenameColumn(table: "dbo.ProjectOwners", name: "__mig_tmp__0", newName: "ProjectID");
            AlterColumn("dbo.ProjectOwners", "UserId", c => c.Guid(nullable: false));
            AlterColumn("dbo.ProjectOwners", "ProjectID", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProjectOwners", "ProjectID", c => c.Guid(nullable: false));
            AlterColumn("dbo.ProjectOwners", "UserId", c => c.Long(nullable: false));
            RenameColumn(table: "dbo.ProjectOwners", name: "ProjectID", newName: "__mig_tmp__0");
            RenameColumn(table: "dbo.ProjectOwners", name: "UserId", newName: "ProjectID");
            RenameColumn(table: "dbo.ProjectOwners", name: "__mig_tmp__0", newName: "UserId");
        }
    }
}
