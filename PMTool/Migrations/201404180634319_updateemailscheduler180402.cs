namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateemailscheduler180402 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.EmailSchedulers", "ScheduledTime", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EmailSchedulers", "ScheduledTime", c => c.DateTime());
        }
    }
}
