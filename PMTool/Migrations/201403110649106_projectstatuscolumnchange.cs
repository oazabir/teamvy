namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class projectstatuscolumnchange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProjectStatus", "color", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProjectStatus", "color");
        }
    }
}
