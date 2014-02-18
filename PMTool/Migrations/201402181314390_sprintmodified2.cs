namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sprintmodified2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sprints", "EndDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Sprints", "EndDateDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Sprints", "EndDateDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Sprints", "EndDate");
        }
    }
}
