namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class taskactivitylogCreated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TaskActivityLogs",
                c => new
                    {
                        TaskActivityLogID = c.Long(nullable: false, identity: true),
                        Comment = c.String(),
                        FileUrl = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                        TaskID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.TaskActivityLogID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TaskActivityLogs");
        }
    }
}
