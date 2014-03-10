namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class taskdescriptionpropertychange : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tasks", "Description", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tasks", "Description", c => c.String(nullable: false));
        }
    }
}
