using PMTool.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web;

namespace PMTool.Repository
{
    public class UserProfileRepository : GenericRepository<UserProfile>
    {
        public UserProfileRepository(SecurityContext context) : base(context) { }

        //public List<UserProfile> All()
        //{
        //    return context.UserProfiles.ToList();
        //}

        //public UserProfile GetUserByUserID(long userID)
        //{
        //    return context.UserProfiles.Where(Usr => Usr.UserId == userID).FirstOrDefault();
        //}

    }
}