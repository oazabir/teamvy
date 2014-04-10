namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modifiedcomment : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Comments", "ParentComment", c => c.Long());
            AlterColumn("dbo.Comments", "ModifiedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Comments", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Comments", "ParentComment", c => c.Long(nullable: false));
        }
    }
}
