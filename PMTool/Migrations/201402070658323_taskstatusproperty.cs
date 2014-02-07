namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class taskstatusproperty : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tasks", "Status", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tasks", "Status", c => c.Int(nullable: false));
        }
    }
}
