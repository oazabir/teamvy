namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TaskMessagemodified : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TaskMessages",
                c => new
                    {
                        TaskMessageID = c.Long(nullable: false, identity: true),
                        TaskID = c.Long(nullable: false),
                        FormUserID = c.Guid(nullable: false),
                        ToUserID = c.Guid(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        Message = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.TaskMessageID)
                .ForeignKey("dbo.Users", t => t.FormUserID)
                .ForeignKey("dbo.Tasks", t => t.TaskID)
                .ForeignKey("dbo.Users", t => t.ToUserID)
                .Index(t => t.FormUserID)
                .Index(t => t.TaskID)
                .Index(t => t.ToUserID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TaskMessages", "ToUserID", "dbo.Users");
            DropForeignKey("dbo.TaskMessages", "TaskID", "dbo.Tasks");
            DropForeignKey("dbo.TaskMessages", "FormUserID", "dbo.Users");
            DropIndex("dbo.TaskMessages", new[] { "ToUserID" });
            DropIndex("dbo.TaskMessages", new[] { "TaskID" });
            DropIndex("dbo.TaskMessages", new[] { "FormUserID" });
            DropTable("dbo.TaskMessages");
        }
    }
}
