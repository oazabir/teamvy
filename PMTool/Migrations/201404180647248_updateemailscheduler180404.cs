namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateemailscheduler180404 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.EmailSchedulers", "ModificationDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EmailSchedulers", "ModificationDate", c => c.DateTime(nullable: false));
        }
    }
}
