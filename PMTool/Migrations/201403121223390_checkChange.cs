namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class checkChange : DbMigration
    {
        public override void Up()
        {
            Sql("update Tasks set Title = SUBSTRING(title,0,74) ");
            AlterColumn("dbo.Tasks", "Title", c => c.String(nullable: false, maxLength: 75));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tasks", "Title", c => c.String(nullable: false));
        }
    }
}
