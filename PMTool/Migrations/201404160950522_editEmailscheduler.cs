namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editEmailscheduler : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmailSchedulers", "RecipientUserType", c => c.Int());
            DropColumn("dbo.EmailSchedulers", "RecipientUsers");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EmailSchedulers", "RecipientUsers", c => c.Int());
            DropColumn("dbo.EmailSchedulers", "RecipientUserType");
        }
    }
}
