using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using PMTool.Repository;
using PMTool.Entities;
using WebMatrix.WebData;


public static class WebSecurity
{
    public static UserProfile GetUser(string username)
    {
        UnitOfWork uow = new UnitOfWork();
        return uow.UserRepository.GetUserByUserName(username);
    }

    public static void UpdateUser(UserProfile user)
    {
        using (UnitOfWork uow = new UnitOfWork())
        {
            uow.UserProfileRepository.Update(user);
            uow.Save();
        }
    }

    public static UserProfile GetCurrentUser()
    {
        return GetUser(CurrentUserName);
    }

    public static void CreateUser(UserProfile user)
    {
        UserProfile dbUser = GetUser(user.UserName);
        if (dbUser != null)
            throw new Exception("User with that username already exists.");
        UnitOfWork uow = new UnitOfWork();
        uow.UserProfileRepository.Insert(user);
        uow.Save();
    }

    public static bool FoundUser(string username)
    {
        UnitOfWork uow = new UnitOfWork();
        UserProfile user = GetUser(username);// uow.UserProfileRepository.Get(u => u.UserName == username).FirstOrDefault();
        return user != null;
    }

    public static string GetEmail(string username)
    {
        //string email = null;
        //UnitOfWork uow = new UnitOfWork();
        //UserProfile user = uow.UserProfileRepository.Get(u => u.UserName == username).SingleOrDefault();
        //if (user != null)
        //    email = user.Email;
        //return email;
        return username; // Since username and useremail is same
    }

    public static void Register()
    {
        SecurityContext context = new SecurityContext();
        context.Database.Initialize(true);
        if (!WebMatrix.WebData.WebSecurity.Initialized)
            WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection("PMToolContext",
                "UserProfile", "UserId", "UserName", autoCreateTables: true);
    }

    public static bool ValidateUser(string userName, string password)
    {
        var membership = (WebMatrix.WebData.SimpleMembershipProvider)Membership.Provider;
        return membership.ValidateUser(userName, password);

    }

    public static bool Login(string userName, string password, bool persistCookie = false)
    {
        return WebMatrix.WebData.WebSecurity.Login(userName, password, persistCookie);
    }

    public static bool ChangePassword(string userName, string oldPassword, string newPassword)
    {
        return WebMatrix.WebData.WebSecurity.ChangePassword(userName, oldPassword, newPassword);
    }

    public static bool ConfirmAccount(string accountConfirmationToken)
    {
        return WebMatrix.WebData.WebSecurity.ConfirmAccount(accountConfirmationToken);
    }

    public static void CreateAccount(string userName, string password, bool requireConfirmationToken = false)
    {
        WebMatrix.WebData.WebSecurity.CreateAccount(userName, password, requireConfirmationToken);
    }

    public static string CreateUserAndAccount(string userName, string password, string email,string firstName,string lastname, bool requireConfirmationToken = false ) 
    {
        return WebMatrix.WebData.WebSecurity.CreateUserAndAccount(userName, password, new { Email = email, FirstName = firstName, LastName=lastname }, requireConfirmationToken);
    }

    public static int GetUserId(string userName)
    {
        return WebMatrix.WebData.WebSecurity.GetUserId(userName);
    }

    public static void Logout()
    {
        WebMatrix.WebData.WebSecurity.Logout();
    }

    public static bool IsAuthenticated { get { return WebMatrix.WebData.WebSecurity.IsAuthenticated; } }

    public static bool IsConfirmed(string username)
    {
        return WebMatrix.WebData.WebSecurity.IsConfirmed(username);
    }

    public static string CurrentUserName { get { return WebMatrix.WebData.WebSecurity.CurrentUserName; } }

    public static int GetCurrentUserId { get { return WebMatrix.WebData.WebSecurity.CurrentUserId; } }

    public static bool DeleteUser(string username, bool deleteAllRelatedData)
    {
        var roleProvider = (SimpleRoleProvider)Roles.Provider;
        var membership = (SimpleMembershipProvider)Membership.Provider;
        if (deleteAllRelatedData)
        {
            string[] roles = roleProvider.GetRolesForUser(username);
            if (roles.Length > 0)
            {
                string[] users = { username };
                roleProvider.RemoveUsersFromRoles(users, roles);
            }
            membership.DeleteAccount(username);
        }
        bool wasDeleted = membership.DeleteUser(username, true);
        return wasDeleted;
    }

    public static string GeneratePasswordResetToken(string userName, int tokenExpirationInMinutesFromNow = 1440)
    {
        return WebMatrix.WebData.WebSecurity.GeneratePasswordResetToken(userName, tokenExpirationInMinutesFromNow);
    }

    public static bool ResetPassword(string passwordResetToken, string newPassword)
    {
        return WebMatrix.WebData.WebSecurity.ResetPassword(passwordResetToken, newPassword);
    }

    public static string GetConfirmationToken(string userName)
    {
        UnitOfWork uow = new UnitOfWork();
        long userId = uow.UserProfileRepository.Get(u => u.UserName == userName).Select(x => x.UserId).SingleOrDefault();
        string token = uow.MembershipRepository.GetConfirmationToken(userId);
        return token;
    }


    public static void InitializeDatabaseConnection(string connectionStringName, string userTableName, string userIdColumn, string userNameColumn, bool autoCreateTables)
    {

    }

    public static void InitializeDatabaseConnection(string connectionString, string providerName, string userTableName, string userIdColumn, string userNameColumn, bool autoCreateTables)
    {

    }


    //public static void CreateUser(UserProfile user)
    //{
    //    UserProfile dbUser = GetUser(user.UserName);
    //    if (dbUser != null)
    //        throw new Exception("User with that username already exists.");
    //    UnitOfWork uow = new UnitOfWork();
    //    uow.UserProfileRepository.Insert(user);
    //    uow.Save();
    //}

}

/*
   public sealed class WebSecurity
    {
        public static HttpContextBase Context
        {
            get { return new HttpContextWrapper(HttpContext.Current); }
        }

        public static HttpRequestBase Request
        {
            get { return Context.Request; }
        }

        public static HttpResponseBase Response
        {
            get { return Context.Response; }
        }

        public static System.Security.Principal.IPrincipal User
        {
            get { return Context.User; }
        }

        public static bool IsAuthenticated
        {
            get { return User.Identity.IsAuthenticated; }
        }

        public static MembershipCreateStatus Register(string Username, string Password, string Email, bool IsApproved, string FirstName, string LastName)
        {
            MembershipCreateStatus CreateStatus;
            Membership.CreateUser(Username, Password, Email, null, null, IsApproved, Guid.NewGuid(), out CreateStatus);

            if (CreateStatus == MembershipCreateStatus.Success)
            {
                using (PMToolContext Context = new PMToolContext())
                {
                    User User = Context.Users.FirstOrDefault(Usr => Usr.Username == Username);
                    User.FirstName = FirstName;
                    User.LastName = LastName;
                    Context.SaveChanges();
                }

                if (IsApproved)
                {
                   // FormsAuthentication.SetAuthCookie(Username, false);
                }
            }

            return CreateStatus;
        }

        public static Boolean Login(string Username, string Password, bool persistCookie  = false)
        {

            bool success = Membership.ValidateUser(Username, Password);
            if (success)
            {
                FormsAuthentication.SetAuthCookie(Username, persistCookie);
            }
            return success;

        }

        public static void Logout()
        {
            FormsAuthentication.SignOut();
        }

        public static MembershipUser GetUser(string Username)
        {
            return Membership.GetUser(Username);
        }

        public static bool ChangePassword(string userName, string currentPassword, string newPassword)
        {
            bool success = false;
            try
            {
                MembershipUser currentUser = Membership.GetUser(userName, true);
                success = currentUser.ChangePassword(currentPassword, newPassword);
            }
            catch (ArgumentException)
            {
                
            }

            return success;
        }

        public static bool DeleteUser(string Username)
        {
            return Membership.DeleteUser(Username);
        }

        //public static int GetUserId(string userName)
        //{
        //    MembershipUser user = Membership.GetUser(userName);
        //    return (int)user.ProviderUserKey;
        //}

        public static int GetUserId(string userName)
        {
            MembershipUser user = Membership.GetUser(userName);
            return (int)user.ProviderUserKey;
        }
       
        public static string CreateAccount(string userName, string password)
        {
            return CreateAccount(userName, password, requireConfirmationToken: false);
        }

        public static string CreateAccount(string userName, string password, bool requireConfirmationToken = false)
        {
            CodeFirstMembershipProvider CodeFirstMembership = Membership.Provider as CodeFirstMembershipProvider;
            return CodeFirstMembership.CreateAccount(userName, password, requireConfirmationToken);
        }

        public static string CreateAccount(string userName, string email,string password, string FirstName,string LastName ,bool requireConfirmationToken = false)
        {
            string status="";
            using (UnitOfWork reopo = new UnitOfWork())
            {
                User existingUser = reopo.UserRepository.GetUserByEmail(email);

                string HashedPassword = Crypto.HashPassword(password);
                if (HashedPassword.Length > 128)
                {
                    status = "InvalidPassword";
                    return null;
                }

                if (existingUser != null)
                {
                    status = "DuplicateEmail";
                    return null;
                }
                User NewUser = new User
                {
                    UserId = Guid.NewGuid(),
                    Username = email,
                    Password = HashedPassword,
                    IsApproved = true,
                    Email = email,
                    CreateDate = DateTime.UtcNow,
                    LastPasswordChangedDate = DateTime.UtcNow,
                    PasswordFailuresSinceLastSuccess = 0,
                    LastLoginDate = DateTime.UtcNow,
                    LastActivityDate = DateTime.UtcNow,
                    LastLockoutDate = DateTime.UtcNow,
                    IsLockedOut = false,
                    LastPasswordFailureDate = DateTime.UtcNow,
                    FirstName = FirstName,
                    LastName = LastName
                };

                reopo.UserRepository.Insert(NewUser);
                reopo.Save();
            }
            return status;
        }

        public static string CreateUserAndAccount(string userName, string password)
        {
            return CreateUserAndAccount(userName, password, propertyValues: null, requireConfirmationToken: false);
        }

        public static string CreateUserAndAccount(string userName, string password, bool requireConfirmation)
        {
            return CreateUserAndAccount(userName, password, propertyValues: null, requireConfirmationToken: requireConfirmation);
        }

        public static string CreateUserAndAccount(string userName, string password, IDictionary<string, object> values)
        {
            return CreateUserAndAccount(userName, password, propertyValues: values, requireConfirmationToken: false);
        }

        public static string CreateUserAndAccount(string userName, string password, object propertyValues = null, bool requireConfirmationToken = false)
        {
            CodeFirstMembershipProvider CodeFirstMembership = Membership.Provider as CodeFirstMembershipProvider;

            IDictionary<string, object> values = null;
            if (propertyValues != null)
            {
                values = new RouteValueDictionary(propertyValues);
            }

            return CodeFirstMembership.CreateUserAndAccount(userName, password, requireConfirmationToken, values);
        }
       
        
       public static List<MembershipUser> FindUsersByEmail(string Email, int PageIndex, int PageSize)
        {
            int totalRecords;
            return Membership.FindUsersByEmail(Email, PageIndex, PageSize, out totalRecords).Cast<MembershipUser>().ToList();
        }



        public static List<MembershipUser> FindUsersByName(string Username, int PageIndex, int PageSize)
        {
            int totalRecords;
            return Membership.FindUsersByName(Username, PageIndex, PageSize, out totalRecords).Cast<MembershipUser>().ToList();
        }

        public static List<MembershipUser> GetAllUsers(int PageIndex, int PageSize)
        {
            int totalRecords;
            return Membership.GetAllUsers(PageIndex, PageSize, out totalRecords).Cast<MembershipUser>().ToList();
        }

        public static void InitializeDatabaseConnection(string connectionStringName, string userTableName, string userIdColumn, string userNameColumn, bool autoCreateTables)
        {
          
        }

        public static void InitializeDatabaseConnection(string connectionString, string providerName, string userTableName, string userIdColumn, string userNameColumn, bool autoCreateTables)
        {
          
        }

       //public static string GeneratePasswordResetToken(string userName)
       //{
       //    //return System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

       //    MembershipUser user = Membership.GetUser(userName); //User Name = User Email
       //    string confirmationGuid = user.ProviderUserKey.ToString();

       //    return confirmationGuid;

       //}

        public static string GeneratePasswordResetToken(string userName, int tokenExpirationInMinutesFromNow = 1440)
        {
            return WebMatrix.WebData.WebSecurity.GeneratePasswordResetToken(userName, tokenExpirationInMinutesFromNow);
        }



    }

*/