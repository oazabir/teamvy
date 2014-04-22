namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TimeLogModelCng2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TimeLogs", "RemainingHour", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TimeLogs", "RemainingHour", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
