namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProjectColumnModified : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ProjectColumns", "Description");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProjectColumns", "Description", c => c.String());
        }
    }
}
