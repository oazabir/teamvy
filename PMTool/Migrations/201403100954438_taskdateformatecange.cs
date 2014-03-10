namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class taskdateformatecange : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tasks", "EstimatedPreviewDate", c => c.DateTime(nullable: true));
            AlterColumn("dbo.Tasks", "ActualPreviewDate", c => c.DateTime(nullable: true));
            AlterColumn("dbo.Tasks", "ForecastLiveDate", c => c.DateTime(nullable: true));
            AlterColumn("dbo.Tasks", "ActualLiveDate", c => c.DateTime(nullable: true));
        }
        
        public override void Down()
        {
        }
    }
}
