namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SlnoAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProjectStatus", "SlNo", c => c.Int(nullable: true));
            Sql("UPDATE [dbo].[ProjectStatus] SET SlNo = 0");
            AlterColumn("dbo.ProjectStatus", "SlNo", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProjectStatus", "SlNo");
        }
    }
}
