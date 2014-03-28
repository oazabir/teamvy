using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMTool.Repository
{
    public class MembershipRepository
    {
        private SecurityContext _context;

        public MembershipRepository(SecurityContext context)
        {
            _context = context;
        }

        public string GetConfirmationToken(long userId)
        {
            string cmd = "select ConfirmationToken from webpages_Membership where UserId = " + userId.ToString();
            return _context.Database.SqlQuery<string>(cmd).FirstOrDefault();
        }
    }
}