namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tests",
                c => new
                    {
                        TestID = c.Long(nullable: false, identity: true),
                        ClientName = c.String(nullable: false, maxLength: 4000),
                        TestAddress = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.TestID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Tests");
        }
    }
}
