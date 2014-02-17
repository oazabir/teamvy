namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class taskChange : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tasks", "CreatedByUser_UserId", "dbo.Users");
            DropForeignKey("dbo.Tasks", "ModifiedByUser_UserId", "dbo.Users");
            DropIndex("dbo.Tasks", new[] { "CreatedByUser_UserId" });
            DropIndex("dbo.Tasks", new[] { "ModifiedByUser_UserId" });
            DropColumn("dbo.Tasks", "CreatedBy");
            RenameColumn(table: "dbo.Tasks", name: "CreatedByUser_UserId", newName: "CreatedBy");
            RenameColumn(table: "dbo.Tasks", name: "ModifiedByUser_UserId", newName: "ModifiedBy");
            AlterColumn("dbo.Tasks", "CreatedBy", c => c.Guid(nullable: false));
            AlterColumn("dbo.Tasks", "ModifiedBy", c => c.Guid(nullable: false));
            CreateIndex("dbo.Tasks", "CreatedBy");
            CreateIndex("dbo.Tasks", "ModifiedBy");
            AddForeignKey("dbo.Tasks", "CreatedBy", "dbo.Users", "UserId");
            AddForeignKey("dbo.Tasks", "ModifiedBy", "dbo.Users", "UserId");
            DropColumn("dbo.Tasks", "ModifieddBy");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tasks", "ModifieddBy", c => c.Guid(nullable: false));
            DropForeignKey("dbo.Tasks", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Tasks", "CreatedBy", "dbo.Users");
            DropIndex("dbo.Tasks", new[] { "ModifiedBy" });
            DropIndex("dbo.Tasks", new[] { "CreatedBy" });
            AlterColumn("dbo.Tasks", "ModifiedBy", c => c.Guid());
            AlterColumn("dbo.Tasks", "CreatedBy", c => c.Guid());
            RenameColumn(table: "dbo.Tasks", name: "ModifiedBy", newName: "ModifiedByUser_UserId");
            RenameColumn(table: "dbo.Tasks", name: "CreatedBy", newName: "CreatedByUser_UserId");
            AddColumn("dbo.Tasks", "CreatedBy", c => c.Guid(nullable: false));
            CreateIndex("dbo.Tasks", "ModifiedByUser_UserId");
            CreateIndex("dbo.Tasks", "CreatedByUser_UserId");
            AddForeignKey("dbo.Tasks", "ModifiedByUser_UserId", "dbo.Users", "UserId");
            AddForeignKey("dbo.Tasks", "CreatedByUser_UserId", "dbo.Users", "UserId");
        }
    }
}
