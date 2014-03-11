namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adddefaultvalue : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE [dbo].[ProjectStatus] SET color = '#3B6998'");
        }
        
        public override void Down()
        {
        }
    }
}
