namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tasktablechange : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Tasks", "IsLocked");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tasks", "IsLocked", c => c.Boolean(nullable: false));
        }
    }
}
