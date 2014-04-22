namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addComment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Message = c.String(nullable: false),
                        TaskID = c.Long(nullable: false),
                        ParentComment = c.Long(nullable: false),
                        CommentBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.UserProfile", t => t.CommentBy)
                .ForeignKey("dbo.Tasks", t => t.TaskID)
                .Index(t => t.CommentBy)
                .Index(t => t.TaskID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "TaskID", "dbo.Tasks");
            DropForeignKey("dbo.Comments", "CommentBy", "dbo.UserProfile");
            DropIndex("dbo.Comments", new[] { "TaskID" });
            DropIndex("dbo.Comments", new[] { "CommentBy" });
            DropTable("dbo.Comments");
        }
    }
}
