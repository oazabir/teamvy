namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateemailscheduler180403 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.EmailSchedulers", "ModifiedBy", "dbo.UserProfile");
            DropIndex("dbo.EmailSchedulers", new[] { "ModifiedBy" });
            AlterColumn("dbo.EmailSchedulers", "ModifiedBy", c => c.Int());
            CreateIndex("dbo.EmailSchedulers", "ModifiedBy");
            AddForeignKey("dbo.EmailSchedulers", "ModifiedBy", "dbo.UserProfile", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmailSchedulers", "ModifiedBy", "dbo.UserProfile");
            DropIndex("dbo.EmailSchedulers", new[] { "ModifiedBy" });
            AlterColumn("dbo.EmailSchedulers", "ModifiedBy", c => c.Int(nullable: false));
            CreateIndex("dbo.EmailSchedulers", "ModifiedBy");
            AddForeignKey("dbo.EmailSchedulers", "ModifiedBy", "dbo.UserProfile", "UserId");
        }
    }
}
