namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class taskActualhrAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tasks", "ActualTaskHoure", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tasks", "ActualTaskHoure");
        }
    }
}
