namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class notificationFatureCreated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        NotificationID = c.Long(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        IsNoticed = c.Boolean(nullable: false),
                        TaskID = c.Long(nullable: false),
                        ProjectID = c.Long(),
                    })
                .PrimaryKey(t => t.NotificationID)
                .ForeignKey("dbo.Projects", t => t.ProjectID)
                .ForeignKey("dbo.Tasks", t => t.TaskID)
                .Index(t => t.ProjectID)
                .Index(t => t.TaskID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notifications", "TaskID", "dbo.Tasks");
            DropForeignKey("dbo.Notifications", "ProjectID", "dbo.Projects");
            DropIndex("dbo.Notifications", new[] { "TaskID" });
            DropIndex("dbo.Notifications", new[] { "ProjectID" });
            DropTable("dbo.Notifications");
        }
    }
}
