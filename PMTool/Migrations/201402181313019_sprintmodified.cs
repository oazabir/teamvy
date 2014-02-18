namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sprintmodified : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sprints", "StartDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Sprints", "EndDateDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sprints", "EndDateDate");
            DropColumn("dbo.Sprints", "StartDate");
        }
    }
}
