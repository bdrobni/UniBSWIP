using Microsoft.AspNetCore.Mvc;
using dipwebapp.Models;
using dipwebapp.Models.Repository;
using XAct.Users;

namespace dipwebapp.Controllers
{
    public class AdminController : Controller
    {
        const string SessionUserID = "_UserID";
        const string SessionUsername = "_Username";
        const string SessionUserRole = "_Role";

        private SiteRepository _siteRepository = new SiteRepository();
        private AdminRepository _adminRepository = new AdminRepository();
        public IActionResult ViewUsers()
        {           
            return View(_adminRepository.ShowUsers());
        }
        public IActionResult AlterUser(int id)
        {
            return View(_siteRepository.GetUser(id));
        }
        public IActionResult ChangeUser(UserBO userBO)
        {
            try
            {
                _adminRepository.AlterUser(userBO);
                return View("UserMessage", "User profile change complete");
            }
            catch (Exception ex)
            {
                return View("UserMessage", "An error occurred, please try again." + ex.Message);
            }
        }
        public IActionResult ConfirmDeleteUser(int id)
        {
            return View(_siteRepository.GetUser(id));
        }
        public IActionResult DeleteUser(UserBO userBO) 
        {
            try
            {
                _adminRepository.DeleteUser(userBO.Id);
                return View("UserMessage", "User profile and associated files have been deleted.");
            }
            catch (Exception ex)
            {
                return View("UserMessage", "An error occurred, please try again." + ex.Message);
            }

        }
        public IActionResult ViewTags()
        {
            return View(_siteRepository.GetTagList());
        }
        public IActionResult CreateTag()
        {
            return View();
        }
        public IActionResult NewTag(TagBO tagbo)
        {
            if(tagbo.Name != null)
            {
                if (!_adminRepository.CheckTagExists(tagbo))
                {
                    try
                    {
                        _siteRepository.CreateTag(tagbo);
                        return View("UserMessage", "New tag created");
                    }
                    catch (Exception ex)
                    {
                        return View("UserMessage", "An error occurred, please try again." + ex.Message);
                    }
                }
                return View("UserMessage", "A tag with the same name already exists");
            }
            return View("UserMessage", "All fields must be filled");
        }
        public IActionResult AlterTag(int id)
        {
            return View(_siteRepository.GetTag(id));
        }
        public IActionResult SaveTagChanges(TagBO tagbo)
        {
            try
            {
                _siteRepository.AlterTag(tagbo);
                return View("UserMessage", "Tag changes have been saved");
            }
            catch (Exception ex)
            {
                return View("UserMessage", "An error occurred, please try again." + ex.Message);
            }
        }
        public IActionResult DeleteTag(int id)
        {
            try
            {                
                _adminRepository.DeleteTag(id);
                return View("ViewTags", _siteRepository.GetTagList());
            }
            catch (Exception ex)
            {
                return View("UserMessage", "An error occurred, please try again." + ex.Message);
            }
        }
    }
}
