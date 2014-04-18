namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateemailscheduler180401 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.EmailSchedulers", "ScheduledTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EmailSchedulers", "ScheduledTime", c => c.Time(precision: 7));
        }
    }
}
