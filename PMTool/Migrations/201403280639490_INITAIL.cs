namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class INITAIL : DbMigration
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
                        CreateDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                        ActionDate = c.DateTime(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LabelID)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy)
                .ForeignKey("dbo.UserProfile", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy);
            
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        NewProp = c.String(),
                        Role_RoleId = c.Guid(),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Roles", t => t.Role_RoleId)
                .Index(t => t.Role_RoleId);
            
            CreateTable(
                "dbo.Tasks",
                c => new
                    {
                        TaskID = c.Long(nullable: false, identity: true),
                        ProjectID = c.Long(nullable: false),
                        Title = c.String(nullable: false, maxLength: 75),
                        Description = c.String(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        TaskHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PriorityID = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        ParentTaskId = c.Long(),
                        Attachments = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                        ActionDate = c.DateTime(nullable: false),
                        Status = c.String(),
                        ProjectStatusID = c.Long(),
                        allStatus = c.String(),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(nullable: false),
                        ActualTaskHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SprintID = c.Long(),
                        EstimatedPreviewDate = c.DateTime(),
                        ActualPreviewDate = c.DateTime(),
                        ForecastLiveDate = c.DateTime(),
                        ActualLiveDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.TaskID)
                .ForeignKey("dbo.Tasks", t => t.ParentTaskId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy)
                .ForeignKey("dbo.Projects", t => t.ProjectID)
                .ForeignKey("dbo.Sprints", t => t.SprintID)
                .ForeignKey("dbo.UserProfile", t => t.ModifiedBy)
                .ForeignKey("dbo.Priorities", t => t.PriorityID)
                .ForeignKey("dbo.ProjectStatus", t => t.ProjectStatusID)
                .Index(t => t.ParentTaskId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ProjectID)
                .Index(t => t.SprintID)
                .Index(t => t.ModifiedBy)
                .Index(t => t.PriorityID)
                .Index(t => t.ProjectStatusID);
            
            CreateTable(
                "dbo.TimeLogs",
                c => new
                    {
                        LogID = c.Long(nullable: false, identity: true),
                        SprintID = c.Long(nullable: false),
                        TaskID = c.Long(nullable: false),
                        UserID = c.Int(nullable: false),
                        EntryDate = c.DateTime(nullable: false),
                        TaskHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                        ActionDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.LogID)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy)
                .ForeignKey("dbo.UserProfile", t => t.ModifiedBy)
                .ForeignKey("dbo.Sprints", t => t.SprintID)
                .ForeignKey("dbo.Tasks", t => t.TaskID)
                .ForeignKey("dbo.UserProfile", t => t.UserID)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.SprintID)
                .Index(t => t.TaskID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.Sprints",
                c => new
                    {
                        SprintID = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        ProjectID = c.Long(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SprintID)
                .ForeignKey("dbo.Projects", t => t.ProjectID)
                .Index(t => t.ProjectID);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        ProjectID = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                        ActionDate = c.DateTime(nullable: false),
                        allStatus = c.String(),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectID)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy)
                .ForeignKey("dbo.UserProfile", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy);
            
            CreateTable(
                "dbo.ProjectStatus",
                c => new
                    {
                        ProjectStatusID = c.Long(nullable: false, identity: true),
                        ProjectID = c.Long(nullable: false),
                        Name = c.String(nullable: false),
                        color = c.String(),
                        SlNo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectStatusID)
                .ForeignKey("dbo.Projects", t => t.ProjectID)
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
                "dbo.Notifications",
                c => new
                    {
                        NotificationID = c.Long(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        IsNoticed = c.Boolean(nullable: false),
                        TaskID = c.Long(),
                        ProjectID = c.Long(),
                        UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.NotificationID)
                .ForeignKey("dbo.Tasks", t => t.TaskID)
                .ForeignKey("dbo.UserProfile", t => t.UserID)
                .Index(t => t.TaskID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.ProjectStatusRules",
                c => new
                    {
                        ProjectStatusRuleID = c.Long(nullable: false, identity: true),
                        ProjectID = c.Long(nullable: false),
                        DateMaper = c.Int(nullable: false),
                        ProjectStatusID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectStatusRuleID)
                .ForeignKey("dbo.Projects", t => t.ProjectID)
                .ForeignKey("dbo.ProjectStatus", t => t.ProjectStatusID)
                .Index(t => t.ProjectID)
                .Index(t => t.ProjectStatusID);
            
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
                "dbo.TaskActivityLogs",
                c => new
                    {
                        TaskActivityLogID = c.Long(nullable: false, identity: true),
                        Comment = c.String(),
                        FileUrl = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        FileDisplayName = c.String(),
                        ModificationDate = c.DateTime(nullable: false),
                        TaskID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.TaskActivityLogID)
                .ForeignKey("dbo.Tasks", t => t.TaskID)
                .Index(t => t.TaskID);
            
            CreateTable(
                "dbo.TaskMessages",
                c => new
                    {
                        TaskMessageID = c.Long(nullable: false, identity: true),
                        TaskID = c.Long(nullable: false),
                        FormUserID = c.Int(nullable: false),
                        ToUserID = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        Message = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.TaskMessageID)
                .ForeignKey("dbo.UserProfile", t => t.FormUserID)
                .ForeignKey("dbo.Tasks", t => t.TaskID)
                .ForeignKey("dbo.UserProfile", t => t.ToUserID)
                .Index(t => t.FormUserID)
                .Index(t => t.TaskID)
                .Index(t => t.ToUserID);
            
            CreateTable(
                "dbo.TaskFollowers",
                c => new
                    {
                        UserId = c.Long(nullable: false),
                        TaskId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.TaskId })
                .ForeignKey("dbo.Tasks", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.UserProfile", t => t.TaskId, cascadeDelete: true)
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
                "dbo.ProjectOwners",
                c => new
                    {
                        ProjectID = c.Long(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProjectID, t.UserId })
                .ForeignKey("dbo.Projects", t => t.ProjectID, cascadeDelete: true)
                .ForeignKey("dbo.UserProfile", t => t.UserId, cascadeDelete: true)
                .Index(t => t.ProjectID)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.ProjectUsers",
                c => new
                    {
                        ProjectId = c.Long(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProjectId, t.UserId })
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .ForeignKey("dbo.UserProfile", t => t.UserId, cascadeDelete: true)
                .Index(t => t.ProjectId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.TaskUsers",
                c => new
                    {
                        TaskId = c.Long(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TaskId, t.UserId })
                .ForeignKey("dbo.Tasks", t => t.TaskId, cascadeDelete: true)
                .ForeignKey("dbo.UserProfile", t => t.UserId, cascadeDelete: true)
                .Index(t => t.TaskId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TaskMessages", "ToUserID", "dbo.UserProfile");
            DropForeignKey("dbo.TaskMessages", "TaskID", "dbo.Tasks");
            DropForeignKey("dbo.TaskMessages", "FormUserID", "dbo.UserProfile");
            DropForeignKey("dbo.TaskActivityLogs", "TaskID", "dbo.Tasks");
            DropForeignKey("dbo.UserProfile", "Role_RoleId", "dbo.Roles");
            DropForeignKey("dbo.ProjectStatusRules", "ProjectStatusID", "dbo.ProjectStatus");
            DropForeignKey("dbo.ProjectStatusRules", "ProjectID", "dbo.Projects");
            DropForeignKey("dbo.Notifications", "UserID", "dbo.UserProfile");
            DropForeignKey("dbo.Notifications", "TaskID", "dbo.Tasks");
            DropForeignKey("dbo.Labels", "ModifiedBy", "dbo.UserProfile");
            DropForeignKey("dbo.Labels", "CreatedBy", "dbo.UserProfile");
            DropForeignKey("dbo.TaskUsers", "UserId", "dbo.UserProfile");
            DropForeignKey("dbo.TaskUsers", "TaskId", "dbo.Tasks");
            DropForeignKey("dbo.Tasks", "ProjectStatusID", "dbo.ProjectStatus");
            DropForeignKey("dbo.Tasks", "PriorityID", "dbo.Priorities");
            DropForeignKey("dbo.Tasks", "ModifiedBy", "dbo.UserProfile");
            DropForeignKey("dbo.TimeLogs", "UserID", "dbo.UserProfile");
            DropForeignKey("dbo.TimeLogs", "TaskID", "dbo.Tasks");
            DropForeignKey("dbo.TimeLogs", "SprintID", "dbo.Sprints");
            DropForeignKey("dbo.Tasks", "SprintID", "dbo.Sprints");
            DropForeignKey("dbo.ProjectUsers", "UserId", "dbo.UserProfile");
            DropForeignKey("dbo.ProjectUsers", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.Tasks", "ProjectID", "dbo.Projects");
            DropForeignKey("dbo.Sprints", "ProjectID", "dbo.Projects");
            DropForeignKey("dbo.ProjectStatus", "ProjectID", "dbo.Projects");
            DropForeignKey("dbo.ProjectOwners", "UserId", "dbo.UserProfile");
            DropForeignKey("dbo.ProjectOwners", "ProjectID", "dbo.Projects");
            DropForeignKey("dbo.Projects", "ModifiedBy", "dbo.UserProfile");
            DropForeignKey("dbo.Projects", "CreatedBy", "dbo.UserProfile");
            DropForeignKey("dbo.TimeLogs", "ModifiedBy", "dbo.UserProfile");
            DropForeignKey("dbo.TimeLogs", "CreatedBy", "dbo.UserProfile");
            DropForeignKey("dbo.TaskLabels", "TaskId", "dbo.Labels");
            DropForeignKey("dbo.TaskLabels", "LabelId", "dbo.Tasks");
            DropForeignKey("dbo.TaskFollowers", "TaskId", "dbo.UserProfile");
            DropForeignKey("dbo.TaskFollowers", "UserId", "dbo.Tasks");
            DropForeignKey("dbo.Tasks", "CreatedBy", "dbo.UserProfile");
            DropForeignKey("dbo.Tasks", "ParentTaskId", "dbo.Tasks");
            DropIndex("dbo.TaskMessages", new[] { "ToUserID" });
            DropIndex("dbo.TaskMessages", new[] { "TaskID" });
            DropIndex("dbo.TaskMessages", new[] { "FormUserID" });
            DropIndex("dbo.TaskActivityLogs", new[] { "TaskID" });
            DropIndex("dbo.UserProfile", new[] { "Role_RoleId" });
            DropIndex("dbo.ProjectStatusRules", new[] { "ProjectStatusID" });
            DropIndex("dbo.ProjectStatusRules", new[] { "ProjectID" });
            DropIndex("dbo.Notifications", new[] { "UserID" });
            DropIndex("dbo.Notifications", new[] { "TaskID" });
            DropIndex("dbo.Labels", new[] { "ModifiedBy" });
            DropIndex("dbo.Labels", new[] { "CreatedBy" });
            DropIndex("dbo.TaskUsers", new[] { "UserId" });
            DropIndex("dbo.TaskUsers", new[] { "TaskId" });
            DropIndex("dbo.Tasks", new[] { "ProjectStatusID" });
            DropIndex("dbo.Tasks", new[] { "PriorityID" });
            DropIndex("dbo.Tasks", new[] { "ModifiedBy" });
            DropIndex("dbo.TimeLogs", new[] { "UserID" });
            DropIndex("dbo.TimeLogs", new[] { "TaskID" });
            DropIndex("dbo.TimeLogs", new[] { "SprintID" });
            DropIndex("dbo.Tasks", new[] { "SprintID" });
            DropIndex("dbo.ProjectUsers", new[] { "UserId" });
            DropIndex("dbo.ProjectUsers", new[] { "ProjectId" });
            DropIndex("dbo.Tasks", new[] { "ProjectID" });
            DropIndex("dbo.Sprints", new[] { "ProjectID" });
            DropIndex("dbo.ProjectStatus", new[] { "ProjectID" });
            DropIndex("dbo.ProjectOwners", new[] { "UserId" });
            DropIndex("dbo.ProjectOwners", new[] { "ProjectID" });
            DropIndex("dbo.Projects", new[] { "ModifiedBy" });
            DropIndex("dbo.Projects", new[] { "CreatedBy" });
            DropIndex("dbo.TimeLogs", new[] { "ModifiedBy" });
            DropIndex("dbo.TimeLogs", new[] { "CreatedBy" });
            DropIndex("dbo.TaskLabels", new[] { "TaskId" });
            DropIndex("dbo.TaskLabels", new[] { "LabelId" });
            DropIndex("dbo.TaskFollowers", new[] { "TaskId" });
            DropIndex("dbo.TaskFollowers", new[] { "UserId" });
            DropIndex("dbo.Tasks", new[] { "CreatedBy" });
            DropIndex("dbo.Tasks", new[] { "ParentTaskId" });
            DropTable("dbo.TaskUsers");
            DropTable("dbo.ProjectUsers");
            DropTable("dbo.ProjectOwners");
            DropTable("dbo.TaskLabels");
            DropTable("dbo.TaskFollowers");
            DropTable("dbo.TaskMessages");
            DropTable("dbo.TaskActivityLogs");
            DropTable("dbo.Roles");
            DropTable("dbo.ProjectStatusRules");
            DropTable("dbo.Notifications");
            DropTable("dbo.Priorities");
            DropTable("dbo.ProjectStatus");
            DropTable("dbo.Projects");
            DropTable("dbo.Sprints");
            DropTable("dbo.TimeLogs");
            DropTable("dbo.Tasks");
            DropTable("dbo.UserProfile");
            DropTable("dbo.Labels");
        }
    }
}
