namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class emailsentstatus01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmailSentStatus",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        EmailSchedulerID = c.Long(nullable: false),
                        ScheduleTypeID = c.Int(),
                        ScheduleDate = c.DateTime(),
                        ScheduleTime = c.Time(precision: 7),
                        SentTime = c.Time(precision: 7),
                        SentStatus = c.Boolean(),
                        ActionTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.EmailSentStatus");
        }
    }
}
