namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class logmodified : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TaskActivityLogs", "FileDisplayName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TaskActivityLogs", "FileDisplayName");
        }
    }
}
