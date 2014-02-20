namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pm : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Notifications", "ProjectID", "dbo.Projects");
            DropIndex("dbo.Notifications", new[] { "ProjectID" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Notifications", "ProjectID");
            AddForeignKey("dbo.Notifications", "ProjectID", "dbo.Projects", "ProjectID");
        }
    }
}
