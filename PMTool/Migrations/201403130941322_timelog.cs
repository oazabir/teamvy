namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class timelog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TimeLogs",
                c => new
                    {
                        LogID = c.Long(nullable: false, identity: true),
                        TaskID = c.Long(nullable: false),
                        UserID = c.Guid(nullable: false),
                        EntryDate = c.DateTime(nullable: false),
                        TaskHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                        CreatedBy = c.Guid(nullable: false),
                        ModifiedBy = c.Guid(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                        ActionDate = c.DateTime(nullable: false),
                        User_UserId = c.Guid(),
                    })
                .PrimaryKey(t => t.LogID)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .ForeignKey("dbo.Tasks", t => t.TaskID)
                .ForeignKey("dbo.Users", t => t.User_UserId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.TaskID)
                .Index(t => t.User_UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TimeLogs", "User_UserId", "dbo.Users");
            DropForeignKey("dbo.TimeLogs", "TaskID", "dbo.Tasks");
            DropForeignKey("dbo.TimeLogs", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.TimeLogs", "CreatedBy", "dbo.Users");
            DropIndex("dbo.TimeLogs", new[] { "User_UserId" });
            DropIndex("dbo.TimeLogs", new[] { "TaskID" });
            DropIndex("dbo.TimeLogs", new[] { "ModifiedBy" });
            DropIndex("dbo.TimeLogs", new[] { "CreatedBy" });
            DropTable("dbo.TimeLogs");
        }
    }
}
