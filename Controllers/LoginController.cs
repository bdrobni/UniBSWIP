using Microsoft.AspNetCore.Mvc;
using dipwebapp.Models;
using dipwebapp.Models.Repository;

namespace dipwebapp.Controllers
{
    public class LoginController : Controller
    {
        
        const string SessionUserID = "_UserID";
        const string SessionUsername = "_Username";
        const string SessionUserRole = "_Role";

        private LoginRepository _loginRepository = new LoginRepository();
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult LoginUser(string email, string password)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = _loginRepository.Login(email, password);

                    HttpContext.Session.SetInt32(SessionUserID, user.Id);
                    HttpContext.Session.SetString(SessionUsername, user.Username);
                    if (user.UserRole == "administrator")
                    {
                        HttpContext.Session.SetString(SessionUserRole, "admin");
                        return View("UserMessage", "You have been logged in as administrator.");
                    }
                    else HttpContext.Session.SetString(SessionUserRole, "user");
                    return View("UserMessage", "Login successful.");
                }
                catch (Exception)
                {
                    return View("UserMessage", "Please ensure you have entered the correct email and password.");
                }

            }
            else return View("UserMessage", "All fields must be filled.");
        }
        public IActionResult Logout() 
        {
            HttpContext.Session.Clear();
            return View("UserMessage", "You have been logged out.");
        }
        
    }
       
}
