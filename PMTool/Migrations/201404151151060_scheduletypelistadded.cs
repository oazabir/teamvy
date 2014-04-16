namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class scheduletypelistadded : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.EmailSchedulers", "ScheduleType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EmailSchedulers", "ScheduleType", c => c.Int(nullable: false));
        }
    }
}
