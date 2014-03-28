namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class taskuid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tasks", "TaskUID", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tasks", "TaskUID");
        }
    }
}
