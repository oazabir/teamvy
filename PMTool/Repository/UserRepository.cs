using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMTool.Repository
{ 
    public class UserRepository : IUserRepository
    {
         PMToolContext context;

         public UserRepository(PMToolContext _context)
        {
            
            this.context = _context;
        }


        public User GetUserByEmail(string email)
        {
            return context.Users.Where(Usr => Usr.Email == email).FirstOrDefault();
        }

        public User GetUserByUserID(Guid userID)
        {
            return context.Users.Where(Usr => Usr.UserId == userID).FirstOrDefault();
        }

        public List<User> All()
        {
            return context.Users.ToList();
        }


        public void Insert(User user)
        {
    
            context.Users.Add(user);

        }

        public void Delete(long id)
        {
      
        }

        public void Save()
        {
            context.SaveChanges();
        }


        public void Dispose()
        {
            context.Dispose();
        }
    }
    public interface IUserRepository : IDisposable
    {
        User GetUserByEmail(string email);
        void Delete(long id);
        void Save();
        void Insert(User user);
        List<User> All();
        User GetUserByUserID(Guid userID);

    }
}