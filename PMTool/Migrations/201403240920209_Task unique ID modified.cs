namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TaskuniqueIDmodified : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tasks", "TaskUID", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tasks", "TaskUID", c => c.String(nullable: false));
        }
    }
}
