using System.Security.Cryptography;
using XSystem.Security.Cryptography;

namespace dipwebapp.Models.Repository
{
    public class LoginRepository
    {
        
        diplomskidbContext context = new diplomskidbContext();

        public UserBO Login(string email, string password)
        {

            UserBO userBO = new UserBO();
            foreach(Appuser u in context.Appuser)
            {
                if(u.Email == email && u.Pass == password)
                {
                    userBO.Id = u.Id;
                    userBO.Username = u.Username;
                    userBO.Email = u.Email;
                    userBO.UserRole = u.Userrole;
                }
            }

            return userBO;
        }
        
    }
        
}
