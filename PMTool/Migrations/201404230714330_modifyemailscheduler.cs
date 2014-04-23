namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modifyemailscheduler : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmailSchedulers", "SchedulerTitleID", c => c.Int(nullable: false));
            DropColumn("dbo.EmailSchedulers", "SchedulerID");
            DropColumn("dbo.EmailSchedulers", "Title");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EmailSchedulers", "Title", c => c.String(nullable: false));
            AddColumn("dbo.EmailSchedulers", "SchedulerID", c => c.Int(nullable: false));
            DropColumn("dbo.EmailSchedulers", "SchedulerTitleID");
        }
    }
}
