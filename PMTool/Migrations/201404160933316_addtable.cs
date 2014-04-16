namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtable : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.EmailSchedulers", "day");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EmailSchedulers", "day", c => c.Int(nullable: false));
        }
    }
}
