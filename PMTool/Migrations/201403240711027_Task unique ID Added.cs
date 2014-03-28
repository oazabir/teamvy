namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TaskuniqueIDAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tasks", "ActualTaskHour", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Tasks", "TaskUID", c => c.String(nullable: false));
            DropColumn("dbo.Tasks", "ActualTaskHoure");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tasks", "ActualTaskHoure", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Tasks", "TaskUID");
            DropColumn("dbo.Tasks", "ActualTaskHour");
        }
    }
}
