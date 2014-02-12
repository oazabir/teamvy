namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class taskModfied : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tasks", "CurrentStatus", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tasks", "CurrentStatus");
        }
    }
}
