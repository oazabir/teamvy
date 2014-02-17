namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class projectChange : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Projects", "CreatedBy_UserId", "dbo.Users");
            DropForeignKey("dbo.Projects", "ModifiedBy_UserId", "dbo.Users");
            DropIndex("dbo.Projects", new[] { "CreatedBy_UserId" });
            DropIndex("dbo.Projects", new[] { "ModifiedBy_UserId" });
            AddColumn("dbo.Projects", "CreatedBy", c => c.Guid(nullable: false));
            AddColumn("dbo.Projects", "ModifiedBy", c => c.Guid(nullable: false));
            CreateIndex("dbo.Projects", "CreatedBy");
            CreateIndex("dbo.Projects", "ModifiedBy");
            AddForeignKey("dbo.Projects", "CreatedBy", "dbo.Users", "UserId");
            AddForeignKey("dbo.Projects", "ModifiedBy", "dbo.Users", "UserId");
            DropColumn("dbo.Projects", "CreatedBy_UserId");
            DropColumn("dbo.Projects", "ModifiedBy_UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Projects", "ModifiedBy_UserId", c => c.Guid(nullable: false));
            AddColumn("dbo.Projects", "CreatedBy_UserId", c => c.Guid(nullable: false));
            DropForeignKey("dbo.Projects", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Projects", "CreatedBy", "dbo.Users");
            DropIndex("dbo.Projects", new[] { "ModifiedBy" });
            DropIndex("dbo.Projects", new[] { "CreatedBy" });
            DropColumn("dbo.Projects", "ModifiedBy");
            DropColumn("dbo.Projects", "CreatedBy");
            CreateIndex("dbo.Projects", "ModifiedBy_UserId");
            CreateIndex("dbo.Projects", "CreatedBy_UserId");
            AddForeignKey("dbo.Projects", "ModifiedBy_UserId", "dbo.Users", "UserId");
            AddForeignKey("dbo.Projects", "CreatedBy_UserId", "dbo.Users", "UserId");
        }
    }
}
