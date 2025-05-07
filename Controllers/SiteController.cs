using Microsoft.AspNetCore.Mvc;
using dipwebapp.Models;
using dipwebapp.Models.Repository;
using System;
using XAct;
using System.Security.Cryptography;
using Microsoft.Extensions.Hosting;

namespace dipwebapp.Controllers
{
    public class SiteController : Controller
    {
        diplomskidbContext context = new diplomskidbContext();
        const string SessionUserID = "_UserID";
        const string SessionUsername = "_Username";
        const string SessionUserRole = "_Role";

        private SiteRepository _siteRepository = new SiteRepository();
        private AdminRepository _adminRepository = new AdminRepository();

        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _environment;

        public SiteController(ILogger<HomeController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public IActionResult Register()
        {
            return View();
        }
        
        public IActionResult RegisterUser(UserBO user)
        {
            if (user.Email != null && user.Password != null && user.Username != null)
            {
                try
                {
                    if (!_siteRepository.CheckUserExists(user))
                    {
                        user.Id = _siteRepository.GenerateUserId();
                        user.UserRole = "user";
                        _siteRepository.RegisterUser(user);
                        HttpContext.Session.SetInt32(SessionUserID, user.Id);
                        HttpContext.Session.SetString(SessionUsername, user.Username);
                        HttpContext.Session.SetString(SessionUserRole, "user");
                        return View("UserMessage", "Registration successful");
                    }
                    else return View("UserMessage", "Email address is already in use.");
                }
                catch (Exception ex)
                {
                    return View("UserMessage", "An error occurred, please try again." + ex.Message);
                }
            }
            else return View("UserMessage", "All fields must be filled.");
        }
        public IActionResult UserProfile(int id)
        {
            UserBO userBO = _siteRepository.GetUser(id);
            if (userBO != null)
            {
                try
                {
                    List<ObjectBO> list = (List<ObjectBO>)_siteRepository.FetchObjectList();
                    userBO.AttachedObjects = from attached in list
                                             where attached.Author.Id == userBO.Id
                                             select attached;
                    return View(userBO);
                }
                catch (Exception ex)
                {
                    return View("UserMessage", "An error occurred, please try again." + ex.Message);
                }
            }
            return View("UserMessage", "An error occurred, please try again.");
        }

        public IActionResult Search(SearchViewModel searchViewModel)
        {           
            searchViewModel = new SearchViewModel();      
            searchViewModel.SelectedObjects = (List<ObjectBO>)_siteRepository.FetchObjectList();           
            searchViewModel.Tags = (List<TagBO>)_siteRepository.GetTagList();
            
            return View(searchViewModel);
        }
        public IActionResult SearchModelsByCriteria(SearchViewModel searchviewmodel)
        {
            SearchViewModel svm = searchviewmodel;
            List<ObjectBO> objectList = (List<ObjectBO>)_siteRepository.FetchObjectList();
            if(svm.SearchBoxContent != null && svm.SearchBoxContent != "")
            {
                string[] tags = svm.SearchBoxContent.Split(',');
                svm.Tags = (List<TagBO>)_siteRepository.GetTagList();
                List<TagBO> tagList = (List<TagBO>)_siteRepository.GetTagListByNames(tags);
                foreach (TagBO tag in tagList)
                {
                    for (int i = objectList.Count - 1; i >= 0; i--) 
                    {
                        if (!_siteRepository.CheckAssociation(tag, objectList.ElementAt(i)))
                        {
                            Console.WriteLine("removed non-tagged" + objectList.ElementAt(i).Title);
                            objectList.Remove(objectList.ElementAt(i));                           
                        }                       
                    }
                }               
            }
            for (int i = objectList.Count - 1; i >= 0; i--)
            {
                if (svm.SelectedFileType != "all")
                {
                    if (svm.SelectedFileType != objectList.ElementAt(i).Filetype)
                    {
                        Console.WriteLine("removed" + objectList.ElementAt(i).Filetype + objectList.ElementAt(i).Title);
                        objectList.Remove(objectList.ElementAt(i));
                    }
                }
                else if (objectList.ElementAt(i).Filetype == "image")
                {
                    Console.WriteLine("removed" + objectList.ElementAt(i).Filetype + objectList.ElementAt(i).Title);
                    objectList.Remove(objectList.ElementAt(i));
                }
            }
            if (svm.SortOption == "oldest")
            {
                objectList = (List<ObjectBO>)objectList.OrderBy(o => o.CreatedDate);
            }

            svm.SelectedObjects = objectList;
            svm.Tags = (List<TagBO>)_siteRepository.GetTagList();
            return View("Search", svm);
        }
        public IActionResult ArticleView(int id) 
        {
            ObjectBO objectBO = _siteRepository.FetchArticle(id);
            objectBO.Tags = (List<TagBO>)_siteRepository.GetObjectTags(id);
            objectBO.AllTags = (List<TagBO>)_siteRepository.GetTagList();
            objectBO.AttachedObjects = (List<ObjectBO>)_siteRepository.FetchAssociatedObjects(id);
            objectBO.Images = from attached in objectBO.AttachedObjects
                              where attached.Filetype == "image"
                              select attached;
            objectBO.WebRootString = _environment.WebRootPath;
            int ? userID = null;
            if (HttpContext.Session.GetInt32("_UserID") != null)
            {
                userID = HttpContext.Session.GetInt32("_UserID");
            }
            if (userID != null)
            {
                objectBO.CurrentUser = _siteRepository.GetUser((int)userID);
            }
            return View(objectBO);
        }
        public IActionResult ModelDetails(int id)
        {
            ObjectBO objectBO = _siteRepository.FetchFileInfo(id);
            objectBO.Tags = (List<TagBO>)_siteRepository.GetObjectTags(id);
            objectBO.AllTags = (List<TagBO>)_siteRepository.GetTagList();
            objectBO.AttachedObjects = (List<ObjectBO>)_siteRepository.FetchAssociatedObjects(id);
            objectBO.Images = from attached in objectBO.AttachedObjects
                              where attached.Filetype == "image"
                              select attached;
            objectBO.WebRootString = _environment.WebRootPath;
            int? userID = null;
            if (HttpContext.Session.GetInt32("_UserID") != null)
            {
                userID = HttpContext.Session.GetInt32("_UserID");
            }
            if (userID != null)
            {
                objectBO.CurrentUser = _siteRepository.GetUser((int)userID);
            }
            return View(objectBO);
        }

        public IActionResult UploadFile()
        {
            return View();
        }
        public IActionResult ListFiles()
        {
            try
            {
                return View(_siteRepository.FetchFileList());
            }
            catch (Exception ex)
            {
                return View("UserMessage", "An error occurred, please try again." + ex.Message);
            }
        }
        public IActionResult ModelUpload(ModelBO modelBO)
        {
            modelBO.Author = _siteRepository.GetUser((int)HttpContext.Session.GetInt32("_UserID"));
            modelBO.CreatedDate = DateTime.Now;
            modelBO.Filename = modelBO.File.FileName;
            modelBO.Filetype = "model";            
                try
                {
                    InsertFile(modelBO);
                    return View("UserMessage", "Your file has been uploaded");
                }
                catch (Exception ex)
                {
                    return View("UserMessage", "An error occurred, please try again." + ex.Message);
                }
        }
        public IActionResult AttachImage(int id)
        {
            ObjectBO objectBO = _siteRepository.FetchFileInfo(id);
            objectBO.Tempid = id;
            return View(objectBO);
        }
        public IActionResult ImageUpload(ModelBO modelBO)
        {
            modelBO.Id = _siteRepository.GenerateFileID();
            modelBO.Author = _siteRepository.GetUser((int)HttpContext.Session.GetInt32("_UserID"));
            modelBO.Title = modelBO.File.FileName;
            modelBO.CreatedDate = DateTime.Now;
            modelBO.Filename = modelBO.File.FileName;
            modelBO.Filetype = "image";          
                try
                {
                    InsertImage(modelBO);
                    AddAttachment(modelBO.Id, modelBO.Tempid);
                    return View("UserMessage", "Your image has been uploaded");
                }
                catch (Exception ex)
                {
                    return View("UserMessage", "An error occurred, please try again." + ex.Message);
                }
        }
        public void InsertImage(ModelBO modelBO)
        {
            Authoredobj authoredobj = new Authoredobj();
            authoredobj.Id = modelBO.Id;
            authoredobj.Authorid = modelBO.Author.Id;
            authoredobj.Title = modelBO.Title;
            authoredobj.Createddate = modelBO.CreatedDate;
            authoredobj.Filetype = modelBO.Filetype;
            authoredobj.Objdescription = "placeholder";
            authoredobj.Objcontent = modelBO.Filename;
            context.Add(authoredobj);
            context.SaveChanges();

            string uploadPath = Path.Combine(_environment.WebRootPath, "images");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            string filePath = Path.Combine(uploadPath, modelBO.Filename);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                modelBO.File.CopyToAsync(stream);
            }
        }
        public void InsertFile(ModelBO modelBO)
        {
            Authoredobj authoredobj = new Authoredobj();
            authoredobj.Id = context.Authoredobj.MakeUniqueIdentifier();
            authoredobj.Authorid = modelBO.Author.Id;
            authoredobj.Title = modelBO.Title;
            authoredobj.Createddate = modelBO.CreatedDate;
            authoredobj.Filetype = modelBO.Filetype;
            authoredobj.Objdescription = modelBO.Description;
            authoredobj.Objcontent = modelBO.Filename;
            context.Add(authoredobj);
            context.SaveChanges();

            string uploadPath = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            string filePath = Path.Combine(uploadPath, modelBO.Filename);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                modelBO.File.CopyToAsync(stream);
            }
        }
        public IActionResult EditModel(int id)
        {
            ObjectBO objectBO = _siteRepository.FetchFileInfo(id);
            objectBO.Tags = (List<TagBO>)_siteRepository.GetObjectTags(id);
            objectBO.AllTags = (List<TagBO>)_siteRepository.GetTagList();
            objectBO.AttachedObjects = (List<ObjectBO>)_siteRepository.FetchAssociatedObjects(id);
            objectBO.Images = from attached in objectBO.AttachedObjects
                              where attached.Filetype == "image"
                              select attached;
            int? userID = null;
            if (HttpContext.Session.GetInt32("_UserID") != null)
            {
                userID = HttpContext.Session.GetInt32("_UserID");
            }
            if (userID != null)
            {
                objectBO.CurrentUser = _siteRepository.GetUser((int)userID);
            }
            return View(objectBO);
        }
        public void ChangeModel(ObjectBO objectBO)
        {
            try
            {
                _siteRepository.AlterArticle(objectBO);
                RedirectToAction("ModelDetails", new { id = objectBO.Id });
            }
            catch (Exception ex)
            {
                RedirectToAction("UserMessage", new { msg = "An error occurred, please try again." + ex.Message });
            }
        }

        public IActionResult DownloadModel(int id)
        {
            ObjectBO modelBO = _siteRepository.FetchFileInfo(id);
            string filename = modelBO.ObjContent;
            if (string.IsNullOrEmpty(filename))
            {
                return Content("Filename is not provided.");
            }
            string filePath = Path.Combine(_environment.WebRootPath, "uploads", filename);
            if (!System.IO.File.Exists(filePath))
            {
                return Content("File not found.");
            }
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/octet-stream", filename);
        }
        public IActionResult DeleteModel(int id)
        {
            ObjectBO objectBO = _siteRepository.FetchFileInfo(id);
            objectBO.AttachedObjects = (List<ObjectBO>)_siteRepository.FetchAssociatedObjects(id);
            objectBO.Images = from attached in objectBO.AttachedObjects
                              where attached.Filetype == "image"
                              select attached;
            string filename = objectBO.ObjContent;
            string filePath = Path.Combine(_environment.WebRootPath, "uploads", filename);
            FileInfo file = new FileInfo(filePath);
            if (file.Exists)
            {
                file.Delete();
                _siteRepository.RemoveAssociations(id);
                _siteRepository.RemoveFileAssociations(id);
                return View("UserMessage", "Your file has been deleted");
            }
            else
            {
                return View("UserMessage", "An error occurred, please try again.");
            }
        }
        public IActionResult CreateArticle()
        {
            return View();
        }
        public IActionResult SaveArticle(ObjectBO ObjectBO)
        {
            ObjectBO.Author = _siteRepository.GetUser((int)HttpContext.Session.GetInt32("_UserID"));
            ObjectBO.CreatedDate = DateTime.Now;
            ObjectBO.Filetype = "article";
            try 
            {
                _siteRepository.SaveText(ObjectBO);
                return View("UserMessage", "Your article has been uploaded");
            }
            catch (Exception ex)
            {
                return View("UserMessage", "An error occurred, please try again." + ex.Message);
            }
        }
        public IActionResult EditArticle(int id) 
        {
            ObjectBO objectBO = _siteRepository.FetchArticle(id);
            objectBO.Tags = (List<TagBO>)_siteRepository.GetObjectTags(id);
            objectBO.AllTags = (List<TagBO>)_siteRepository.GetTagList();
            objectBO.AttachedObjects = (List<ObjectBO>)_siteRepository.FetchAssociatedObjects(id);
            objectBO.Images = from attached in objectBO.AttachedObjects
                              where attached.Filetype == "image"
                              select attached;
            int? userID = null;
            if (HttpContext.Session.GetInt32("_UserID") != null)
            {
                userID = HttpContext.Session.GetInt32("_UserID");
            }
            if (userID != null)
            {
                objectBO.CurrentUser = _siteRepository.GetUser((int)userID);
            }
            return View(objectBO);
        }
        public IActionResult ChangeArticle(ObjectBO objectBO)
        {
            try
            {
                _siteRepository.AlterArticle(objectBO);
                return RedirectToAction("ArticleView", new { id = objectBO.Id });
            }
            catch (Exception ex)
            {
                return RedirectToAction("UserMessage", new { msg = "An error occurred, please try again." + ex.Message });
            }
        }
        public IActionResult DeleteUserArticle(int id)
        {
            try
            {
                _siteRepository.DeleteArticle(id);
                _siteRepository.RemoveAssociations(id);
                _siteRepository.RemoveFileAssociations(id);
                return View("UserMessage", "Your article has been deleted");
            }
            catch (Exception ex)
            {
                return View("UserMessage", "An error occurred, please try again." + ex.Message);
            }
        }
        public IActionResult AddTag(int objid, int tagid)
        {
            ObjectBO modelBO = _siteRepository.FetchFileInfo(objid);
            TagBO tagBO = _siteRepository.GetTag(tagid);
            if(!_siteRepository.CheckAssociation(tagBO, modelBO))
            {
                _siteRepository.ApplyTag(tagBO, modelBO);
            }           
            if (modelBO.Filetype == "article")
            {
                return RedirectToAction("ArticleView", new { id = objid });
            }
            else
            {
                return RedirectToAction("ModelDetails", new { id = objid });
            }
        }
        
        public IActionResult RemoveTag(int objid, int tagid)
        {
            ObjectBO modelBO = _siteRepository.FetchFileInfo(objid);
            _siteRepository.RemoveTag(tagid, objid);
            if (modelBO.Filetype == "article")
            {
                return RedirectToAction("ArticleView", new { id = objid });
            }
            else
            {
                return RedirectToAction("ModelDetails", new { id = objid });
            }
        }

        public IActionResult AttachFile(int id)
        {
            ObjectBO modelBO = _siteRepository.FetchFileInfo(id);
            modelBO.Objects = _siteRepository.FetchFileList();
            int? userID = null;
            if (HttpContext.Session.GetInt32("_UserID") != null)
            {
                userID = HttpContext.Session.GetInt32("_UserID");
            }
            if (userID != null)
            {
                modelBO.CurrentUser = _siteRepository.GetUser((int)userID);
            }
            return View(modelBO);
        }
        public IActionResult AddAttachment(int parentid, int childid)
        {
            ObjectBO parent = _siteRepository.FetchFileInfo(parentid);
            ObjectBO child = _siteRepository.FetchFileInfo(childid);
            if (!_siteRepository.CheckFileAssociation(parent, child))
            {
                try
                {
                    _siteRepository.CreateAssociation(parent, child);
                    return RedirectToAction("ModelDetails", new { id = parentid });
                }
                catch (Exception ex)
                {
                    return RedirectToAction("UserMessage", new { msg = "An error occurred, please try again." + ex.Message });
                }

            }
            else return RedirectToAction("UserMessage", new { msg = "This file is already attached to your article."});
        }
        public IActionResult RemoveAttachment(int parentid, int childid)
        {
            try
            {
               _siteRepository.RemoveSingleAssociation(parentid, childid);
               return RedirectToAction("ModelDetails", new { id = parentid });
            }
            catch (Exception ex)
            {
               return RedirectToAction("UserMessage", new { msg = "An error occurred, please try again." + ex.Message });
            }
        }
        
    }
}
