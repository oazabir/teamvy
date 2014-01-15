namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class phase1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Labels",
                c => new
                    {
                        LabelID = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.Guid(nullable: false),
                        ModifieddBy = c.Guid(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                        ActionDate = c.DateTime(nullable: false),
                        CreatedByUser_UserId = c.Guid(),
                        ModifiedByUser_UserId = c.Guid(),
                    })
                .PrimaryKey(t => t.LabelID)
                .ForeignKey("dbo.Users", t => t.CreatedByUser_UserId)
                .ForeignKey("dbo.Users", t => t.ModifiedByUser_UserId)
                .Index(t => t.CreatedByUser_UserId)
                .Index(t => t.ModifiedByUser_UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Guid(nullable: false),
                        Username = c.String(nullable: false),
                        Email = c.String(),
                        Password = c.String(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Comment = c.String(),
                        IsApproved = c.Boolean(nullable: false),
                        PasswordFailuresSinceLastSuccess = c.Int(nullable: false),
                        LastPasswordFailureDate = c.DateTime(),
                        LastActivityDate = c.DateTime(),
                        LastLockoutDate = c.DateTime(),
                        LastLoginDate = c.DateTime(),
                        ConfirmationToken = c.String(),
                        CreateDate = c.DateTime(),
                        IsLockedOut = c.Boolean(nullable: false),
                        LastPasswordChangedDate = c.DateTime(),
                        PasswordVerificationToken = c.String(),
                        PasswordVerificationTokenExpirationDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.Tasks",
                c => new
                    {
                        TaskID = c.Long(nullable: false, identity: true),
                        ProjectID = c.Long(nullable: false),
                        Title = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        TaskHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PriorityID = c.Int(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        ParentTaskId = c.Int(),
                        Attachments = c.String(),
                        CreatedBy = c.Guid(nullable: false),
                        ModifieddBy = c.Guid(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                        ActionDate = c.DateTime(nullable: false),
                        Task_TaskID = c.Long(),
                        CreatedByUser_UserId = c.Guid(),
                        ModifiedByUser_UserId = c.Guid(),
                    })
                .PrimaryKey(t => t.TaskID)
                .ForeignKey("dbo.Tasks", t => t.Task_TaskID)
                .ForeignKey("dbo.Users", t => t.CreatedByUser_UserId)
                .ForeignKey("dbo.Users", t => t.ModifiedByUser_UserId)
                .ForeignKey("dbo.Priorities", t => t.PriorityID, cascadeDelete: true)
                .ForeignKey("dbo.Projects", t => t.ProjectID, cascadeDelete: true)
                .Index(t => t.Task_TaskID)
                .Index(t => t.CreatedByUser_UserId)
                .Index(t => t.ModifiedByUser_UserId)
                .Index(t => t.PriorityID)
                .Index(t => t.ProjectID);
            
            CreateTable(
                "dbo.Priorities",
                c => new
                    {
                        PriorityID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.PriorityID);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        ProjectID = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.Guid(nullable: false),
                        ModifieddBy = c.Guid(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                        ActionDate = c.DateTime(nullable: false),
                        CreatedByUser_UserId = c.Guid(),
                        ModifiedByUser_UserId = c.Guid(),
                    })
                .PrimaryKey(t => t.ProjectID)
                .ForeignKey("dbo.Users", t => t.CreatedByUser_UserId)
                .ForeignKey("dbo.Users", t => t.ModifiedByUser_UserId)
                .Index(t => t.CreatedByUser_UserId)
                .Index(t => t.ModifiedByUser_UserId);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        RoleId = c.Guid(nullable: false),
                        RoleName = c.String(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.RoleId);
            
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.TaskFollowers",
                c => new
                    {
                        UserId = c.Long(nullable: false),
                        TaskId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.TaskId })
                .ForeignKey("dbo.Tasks", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.TaskId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.TaskId);
            
            CreateTable(
                "dbo.TaskLabels",
                c => new
                    {
                        LabelId = c.Long(nullable: false),
                        TaskId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.LabelId, t.TaskId })
                .ForeignKey("dbo.Tasks", t => t.LabelId, cascadeDelete: true)
                .ForeignKey("dbo.Labels", t => t.TaskId, cascadeDelete: true)
                .Index(t => t.LabelId)
                .Index(t => t.TaskId);
            
            CreateTable(
                "dbo.ProjectUsers",
                c => new
                    {
                        ProjectId = c.Long(nullable: false),
                        UserId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProjectId, t.UserId })
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.ProjectId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.TaskUsers",
                c => new
                    {
                        TaskId = c.Long(nullable: false),
                        UserId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.TaskId, t.UserId })
                .ForeignKey("dbo.Tasks", t => t.TaskId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.TaskId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.RoleUsers",
                c => new
                    {
                        Role_RoleId = c.Guid(nullable: false),
                        User_UserId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Role_RoleId, t.User_UserId })
                .ForeignKey("dbo.Roles", t => t.Role_RoleId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.User_UserId, cascadeDelete: true)
                .Index(t => t.Role_RoleId)
                .Index(t => t.User_UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Labels", "ModifiedByUser_UserId", "dbo.Users");
            DropForeignKey("dbo.Labels", "CreatedByUser_UserId", "dbo.Users");
            DropForeignKey("dbo.RoleUsers", "User_UserId", "dbo.Users");
            DropForeignKey("dbo.RoleUsers", "Role_RoleId", "dbo.Roles");
            DropForeignKey("dbo.TaskUsers", "UserId", "dbo.Users");
            DropForeignKey("dbo.TaskUsers", "TaskId", "dbo.Tasks");
            DropForeignKey("dbo.Tasks", "ProjectID", "dbo.Projects");
            DropForeignKey("dbo.ProjectUsers", "UserId", "dbo.Users");
            DropForeignKey("dbo.ProjectUsers", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.Projects", "ModifiedByUser_UserId", "dbo.Users");
            DropForeignKey("dbo.Projects", "CreatedByUser_UserId", "dbo.Users");
            DropForeignKey("dbo.Tasks", "PriorityID", "dbo.Priorities");
            DropForeignKey("dbo.Tasks", "ModifiedByUser_UserId", "dbo.Users");
            DropForeignKey("dbo.TaskLabels", "TaskId", "dbo.Labels");
            DropForeignKey("dbo.TaskLabels", "LabelId", "dbo.Tasks");
            DropForeignKey("dbo.TaskFollowers", "TaskId", "dbo.Users");
            DropForeignKey("dbo.TaskFollowers", "UserId", "dbo.Tasks");
            DropForeignKey("dbo.Tasks", "CreatedByUser_UserId", "dbo.Users");
            DropForeignKey("dbo.Tasks", "Task_TaskID", "dbo.Tasks");
            DropIndex("dbo.Labels", new[] { "ModifiedByUser_UserId" });
            DropIndex("dbo.Labels", new[] { "CreatedByUser_UserId" });
            DropIndex("dbo.RoleUsers", new[] { "User_UserId" });
            DropIndex("dbo.RoleUsers", new[] { "Role_RoleId" });
            DropIndex("dbo.TaskUsers", new[] { "UserId" });
            DropIndex("dbo.TaskUsers", new[] { "TaskId" });
            DropIndex("dbo.Tasks", new[] { "ProjectID" });
            DropIndex("dbo.ProjectUsers", new[] { "UserId" });
            DropIndex("dbo.ProjectUsers", new[] { "ProjectId" });
            DropIndex("dbo.Projects", new[] { "ModifiedByUser_UserId" });
            DropIndex("dbo.Projects", new[] { "CreatedByUser_UserId" });
            DropIndex("dbo.Tasks", new[] { "PriorityID" });
            DropIndex("dbo.Tasks", new[] { "ModifiedByUser_UserId" });
            DropIndex("dbo.TaskLabels", new[] { "TaskId" });
            DropIndex("dbo.TaskLabels", new[] { "LabelId" });
            DropIndex("dbo.TaskFollowers", new[] { "TaskId" });
            DropIndex("dbo.TaskFollowers", new[] { "UserId" });
            DropIndex("dbo.Tasks", new[] { "CreatedByUser_UserId" });
            DropIndex("dbo.Tasks", new[] { "Task_TaskID" });
            DropTable("dbo.RoleUsers");
            DropTable("dbo.TaskUsers");
            DropTable("dbo.ProjectUsers");
            DropTable("dbo.TaskLabels");
            DropTable("dbo.TaskFollowers");
            DropTable("dbo.UserProfile");
            DropTable("dbo.Roles");
            DropTable("dbo.Projects");
            DropTable("dbo.Priorities");
            DropTable("dbo.Tasks");
            DropTable("dbo.Users");
            DropTable("dbo.Labels");
        }
    }
}
