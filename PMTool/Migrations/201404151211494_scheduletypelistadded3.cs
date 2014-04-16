namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class scheduletypelistadded3 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.EmailSchedulers", "EmailRecipientUsers");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EmailSchedulers", "EmailRecipientUsers", c => c.Int(nullable: false));
        }
    }
}
