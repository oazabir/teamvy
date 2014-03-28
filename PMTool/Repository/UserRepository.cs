using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMTool.Models;

namespace PMTool.Repository
{ 
    public class UserRepository : IUserRepository
    {
         PMToolContext context;

         public UserRepository(PMToolContext _context)
        {
            
            this.context = _context;
        }


         public UserProfile GetUserByEmail(string email)
        {
            return context.UserProfiles.Where(Usr => Usr.Email == email).FirstOrDefault();
        }

         public UserProfile GetUserByUserName(string userName)
        {
            return context.UserProfiles.Where(Usr => Usr.UserName == userName).FirstOrDefault();
        }

        public UserProfile GetUserByUserID(long userID)
        {
            return context.UserProfiles.Where(Usr => Usr.UserId == userID).FirstOrDefault();
        }

        public List<UserProfile> All()
        {
            return context.UserProfiles.ToList();
        }


        public void Insert(UserProfile user)
        {

            context.UserProfiles.Add(user);

        }

        public void Delete(long id)
        {
      
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Update(UserProfile user)
        {
            context.Entry(user).State = System.Data.Entity.EntityState.Modified;
        }

        public void Dispose()
        {
            context.Dispose();
        }


    }
    public interface IUserRepository : IDisposable
    {
        UserProfile GetUserByEmail(string email);
        void Delete(long id);
        void Save();
        void Insert(UserProfile user);
        List<UserProfile> All();
        UserProfile GetUserByUserID(long userID);
        void Update(UserProfile user);

    }
}