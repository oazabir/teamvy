namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class assd : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tasks", "EstimatedPreviewDate", c => c.DateTime());
            AlterColumn("dbo.Tasks", "ActualPreviewDate", c => c.DateTime());
            AlterColumn("dbo.Tasks", "ForecastLiveDate", c => c.DateTime());
            AlterColumn("dbo.Tasks", "ActualLiveDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tasks", "ActualLiveDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Tasks", "ForecastLiveDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Tasks", "ActualPreviewDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Tasks", "EstimatedPreviewDate", c => c.DateTime(nullable: false));
        }
    }
}
