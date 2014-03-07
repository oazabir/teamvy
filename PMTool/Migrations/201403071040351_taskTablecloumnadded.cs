namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class taskTablecloumnadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tasks", "EstimatedPreviewDate", c => c.DateTime(nullable: true));
            AddColumn("dbo.Tasks", "ActualPreviewDate", c => c.DateTime(nullable: true));
            AddColumn("dbo.Tasks", "ForecastLiveDate", c => c.DateTime(nullable: true));
            AddColumn("dbo.Tasks", "ActualLiveDate", c => c.DateTime(nullable: true));


            Sql("UPDATE [dbo].[Tasks] SET EstimatedPreviewDate = '2014-03-07',ActualPreviewDate= '2014-03-07',ForecastLiveDate='2014-03-07',ActualLiveDate='2014-03-07' ");

            AlterColumn("dbo.Tasks", "EstimatedPreviewDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Tasks", "ActualPreviewDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Tasks", "ForecastLiveDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Tasks", "ActualLiveDate", c => c.DateTime(nullable: false));
            
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tasks", "ActualLiveDate");
            DropColumn("dbo.Tasks", "ForecastLiveDate");
            DropColumn("dbo.Tasks", "ActualPreviewDate");
            DropColumn("dbo.Tasks", "EstimatedPreviewDate");
        }
    }
}
