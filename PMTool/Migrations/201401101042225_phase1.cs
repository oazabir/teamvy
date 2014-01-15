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
                        Project_ProjectID = c.Long(),
                        Task_TaskID = c.Long(),
                        Task_TaskID1 = c.Long(),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Projects", t => t.Project_ProjectID)
                .ForeignKey("dbo.Tasks", t => t.Task_TaskID)
                .ForeignKey("dbo.Tasks", t => t.Task_TaskID1)
                .Index(t => t.Project_ProjectID)
                .Index(t => t.Task_TaskID)
                .Index(t => t.Task_TaskID1);
            
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
                        User_UserId = c.Guid(),
                    })
                .PrimaryKey(t => t.ProjectID)
                .ForeignKey("dbo.Users", t => t.CreatedByUser_UserId)
                .ForeignKey("dbo.Users", t => t.ModifiedByUser_UserId)
                .ForeignKey("dbo.Users", t => t.User_UserId)
                .Index(t => t.CreatedByUser_UserId)
                .Index(t => t.ModifiedByUser_UserId)
                .Index(t => t.User_UserId);
            
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
                        User_UserId = c.Guid(),
                        User_UserId1 = c.Guid(),
                    })
                .PrimaryKey(t => t.TaskID)
                .ForeignKey("dbo.Tasks", t => t.Task_TaskID)
                .ForeignKey("dbo.Users", t => t.CreatedByUser_UserId)
                .ForeignKey("dbo.Users", t => t.ModifiedByUser_UserId)
                .ForeignKey("dbo.Priorities", t => t.PriorityID, cascadeDelete: true)
                .ForeignKey("dbo.Projects", t => t.ProjectID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.User_UserId)
                .ForeignKey("dbo.Users", t => t.User_UserId1)
                .Index(t => t.Task_TaskID)
                .Index(t => t.CreatedByUser_UserId)
                .Index(t => t.ModifiedByUser_UserId)
                .Index(t => t.PriorityID)
                .Index(t => t.ProjectID)
                .Index(t => t.User_UserId)
                .Index(t => t.User_UserId1);
            
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
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
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
            
            CreateTable(
                "dbo.TaskLabels",
                c => new
                    {
                        Task_TaskID = c.Long(nullable: false),
                        Label_LabelID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.Task_TaskID, t.Label_LabelID })
                .ForeignKey("dbo.Tasks", t => t.Task_TaskID, cascadeDelete: true)
                .ForeignKey("dbo.Labels", t => t.Label_LabelID, cascadeDelete: true)
                .Index(t => t.Task_TaskID)
                .Index(t => t.Label_LabelID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Labels", "ModifiedByUser_UserId", "dbo.Users");
            DropForeignKey("dbo.Labels", "CreatedByUser_UserId", "dbo.Users");
            DropForeignKey("dbo.Tasks", "User_UserId1", "dbo.Users");
            DropForeignKey("dbo.Tasks", "User_UserId", "dbo.Users");
            DropForeignKey("dbo.Users", "Task_TaskID1", "dbo.Tasks");
            DropForeignKey("dbo.TaskLabels", "Label_LabelID", "dbo.Labels");
            DropForeignKey("dbo.TaskLabels", "Task_TaskID", "dbo.Tasks");
            DropForeignKey("dbo.Users", "Task_TaskID", "dbo.Tasks");
            DropForeignKey("dbo.Tasks", "ProjectID", "dbo.Projects");
            DropForeignKey("dbo.Tasks", "PriorityID", "dbo.Priorities");
            DropForeignKey("dbo.Tasks", "ModifiedByUser_UserId", "dbo.Users");
            DropForeignKey("dbo.Tasks", "CreatedByUser_UserId", "dbo.Users");
            DropForeignKey("dbo.Tasks", "Task_TaskID", "dbo.Tasks");
            DropForeignKey("dbo.RoleUsers", "User_UserId", "dbo.Users");
            DropForeignKey("dbo.RoleUsers", "Role_RoleId", "dbo.Roles");
            DropForeignKey("dbo.Projects", "User_UserId", "dbo.Users");
            DropForeignKey("dbo.Users", "Project_ProjectID", "dbo.Projects");
            DropForeignKey("dbo.Projects", "ModifiedByUser_UserId", "dbo.Users");
            DropForeignKey("dbo.Projects", "CreatedByUser_UserId", "dbo.Users");
            DropIndex("dbo.Labels", new[] { "ModifiedByUser_UserId" });
            DropIndex("dbo.Labels", new[] { "CreatedByUser_UserId" });
            DropIndex("dbo.Tasks", new[] { "User_UserId1" });
            DropIndex("dbo.Tasks", new[] { "User_UserId" });
            DropIndex("dbo.Users", new[] { "Task_TaskID1" });
            DropIndex("dbo.TaskLabels", new[] { "Label_LabelID" });
            DropIndex("dbo.TaskLabels", new[] { "Task_TaskID" });
            DropIndex("dbo.Users", new[] { "Task_TaskID" });
            DropIndex("dbo.Tasks", new[] { "ProjectID" });
            DropIndex("dbo.Tasks", new[] { "PriorityID" });
            DropIndex("dbo.Tasks", new[] { "ModifiedByUser_UserId" });
            DropIndex("dbo.Tasks", new[] { "CreatedByUser_UserId" });
            DropIndex("dbo.Tasks", new[] { "Task_TaskID" });
            DropIndex("dbo.RoleUsers", new[] { "User_UserId" });
            DropIndex("dbo.RoleUsers", new[] { "Role_RoleId" });
            DropIndex("dbo.Projects", new[] { "User_UserId" });
            DropIndex("dbo.Users", new[] { "Project_ProjectID" });
            DropIndex("dbo.Projects", new[] { "ModifiedByUser_UserId" });
            DropIndex("dbo.Projects", new[] { "CreatedByUser_UserId" });
            DropTable("dbo.TaskLabels");
            DropTable("dbo.RoleUsers");
            DropTable("dbo.UserProfile");
            DropTable("dbo.Priorities");
            DropTable("dbo.Tasks");
            DropTable("dbo.Roles");
            DropTable("dbo.Projects");
            DropTable("dbo.Users");
            DropTable("dbo.Labels");
        }
    }
}
