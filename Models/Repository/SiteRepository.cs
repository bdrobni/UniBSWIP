using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using dipwebapp.Controllers;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using XAct;
using XAct.Users;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace dipwebapp.Models.Repository
{
    public class SiteRepository
    {

        diplomskidbContext context = new diplomskidbContext();
        private readonly IWebHostEnvironment _environment;
        #region users

        public UserBO GetUser(int id)
        {
            Appuser u = context.Appuser.FirstOrDefault(a => a.Id == id);
            UserBO userBO = new UserBO();
            if (u != null)
            {               
                userBO.Id = u.Id;
                userBO.Username = u.Username;
                userBO.Password = u.Pass;
                userBO.Email = u.Email;
                userBO.UserRole = u.Userrole;
            }
            return userBO;
        }
        public bool CheckUserExists(UserBO user)
        {
            foreach (Appuser u in context.Appuser)
            {
                if(user.Email == u.Email)
                {
                    return true;
                }                
            }
            return false;
        }
        public void RegisterUser(UserBO user) 
        {
            Appuser u = new Appuser();
            if (user != null) 
            {
                u.Id = user.Id;
                u.Username = user.Username;
                u.Email = user.Email;
                u.Pass = user.Password;
                u.Userrole = "user";
                context.Add(u);
                context.SaveChanges();
            }
        }
        public int GenerateUserId()
        {
            int newid = context.Appuser.MakeUniqueIdentifier();
            return newid;
        }

        #endregion

        #region files

        public int GenerateFileID()
        {
            int newid = context.Authoredobj.MakeUniqueIdentifier();
            return newid;
        }
        public byte[] ConvertIFormFileToByteArray(IFormFile file)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                file.CopyTo(ms);
                return ms.ToArray();
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
            // Create the folder if it doesn't exist
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            // Generate the file path
            string filePath = Path.Combine(uploadPath, modelBO.Filename);
            // Save the file to the specified location
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
               modelBO.File.CopyToAsync(stream);
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
            authoredobj.Objdescription = modelBO.Description;
            authoredobj.Objcontent = modelBO.Filename;
            context.Add(authoredobj);
            context.SaveChanges();

            string uploadPath = Path.Combine(_environment.WebRootPath, "images");
            // Create the folder if it doesn't exist
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            // Generate the file path
            string filePath = Path.Combine(uploadPath, modelBO.Filename);
            // Save the file to the specified location
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                modelBO.File.CopyToAsync(stream);
            }
        }

        public void EditFile(ObjectBO objectBO)
        {
            Authoredobj authoredobj = context.Authoredobj.FirstOrDefault(a => a.Id == objectBO.Id);
            if (authoredobj != null && authoredobj.Filetype == "model")
            {
                authoredobj.Title = objectBO.Title;
                authoredobj.Objdescription = objectBO.Description;
                context.SaveChanges();
            }
        }

        public ObjectBO FetchFileInfo(int id)
        {
            Authoredobj authoredobj = context.Authoredobj.FirstOrDefault(a => a.Id == id);
            ObjectBO objectBO = new ModelBO();
            if (authoredobj != null)
            {
                objectBO.Id = id;
                objectBO.Title = authoredobj.Title;
                objectBO.CreatedDate = authoredobj.Createddate;
                objectBO.Author = GetUser(authoredobj.Authorid);
                objectBO.Filetype = authoredobj.Filetype;
                objectBO.ObjContent = authoredobj.Objcontent;
                objectBO.Description = authoredobj.Objdescription;
            }
            return objectBO;
        }
        public IEnumerable<ObjectBO> FetchObjectList()
        {
            List<ObjectBO> models = new List<ObjectBO>();
            foreach (Authoredobj a in context.Authoredobj)
            {
                if (a.Filetype != "image")
                {
                    models.Add(FetchFileInfo(a.Id));
                }
            }
            return models;
        }
        public IEnumerable<ObjectBO> FetchFileList()
        {
            List<ObjectBO> models = new List<ObjectBO>();
            foreach (Authoredobj a in context.Authoredobj)
            {
                if (a.Filetype == "model")
                {
                    models.Add(FetchFileInfo(a.Id));
                }
            }
            return models;
        }
        public byte[] FetchFile(string filename)
        {
            byte[] fileBytes = null;
            if (!string.IsNullOrEmpty(filename))
            {
                string filePath = Path.Combine(_environment.WebRootPath, "uploads", filename);
                if (System.IO.File.Exists(filePath))
                {
                    fileBytes = File.ReadAllBytes(filePath);
                }
            }
            return fileBytes;
        }

        public void DeleteFile(int id)
        {
            Authoredobj authoredobj = context.Authoredobj.FirstOrDefault(a => a.Id == id);
            if (authoredobj != null)
            {
                string filePath = Path.Combine(_environment.WebRootPath, "uploads", authoredobj.Objcontent);
                FileInfo file = new FileInfo(filePath);
                if (file.Exists)
                {
                    file.Delete();
                    context.Authoredobj.Remove(authoredobj);
                }
                else
                {
                    context.Authoredobj.Remove(authoredobj);
                }
            }
            context.SaveChanges();
        }

        #endregion

        #region articles

        public ObjectBO FetchArticle(int id)
        {
            Authoredobj authoredobj = context.Authoredobj.FirstOrDefault(a => a.Id == id);
            ObjectBO articleBO = new ObjectBO();
            if (authoredobj != null && authoredobj.Filetype == "article") 
            {
                articleBO.Id = authoredobj.Id;
                articleBO.Title = authoredobj.Title;
                articleBO.Author = GetUser(authoredobj.Authorid);
                articleBO.CreatedDate = authoredobj.Createddate;
                articleBO.Description = authoredobj.Objdescription;
                articleBO.ObjContent = authoredobj.Objcontent;             
            }
            return articleBO;
        }
        public IEnumerable<ArticleBO> FetchArticles()
        {
            List<ArticleBO> articles = new List<ArticleBO>();
            foreach(Authoredobj authoredobj in context.Authoredobj)
            {
                ArticleBO articleBO = new ArticleBO();
                articleBO.Id = authoredobj.Id;
                articleBO.Title = authoredobj.Title;
                articleBO.Author = GetUser(authoredobj.Authorid);
                articleBO.CreatedDate = authoredobj.Createddate;
                articleBO.Description = authoredobj.Objdescription;
                articleBO.Text = authoredobj.Objcontent;
                articles.Add(articleBO);
            }
            return articles;
        }
        
        public void SaveText(ObjectBO objectBO)       
        {
            Authoredobj authoredobj = new Authoredobj();
            authoredobj.Id = context.Authoredobj.MakeUniqueIdentifier();
            authoredobj.Authorid = objectBO.Author.Id;
            authoredobj.Title = objectBO.Title;
            authoredobj.Createddate = objectBO.CreatedDate;
            authoredobj.Filetype = "article";
            authoredobj.Objdescription = objectBO.Description;
            authoredobj.Objcontent = objectBO.ObjContent;
            context.Add(authoredobj);
            context.SaveChanges();
        }

        public void AlterArticle(ObjectBO articleBO)
        {
            Authoredobj authoredobj = context.Authoredobj.FirstOrDefault(a=>a.Id == articleBO.Id);
            if (authoredobj == null)
                throw new InvalidOperationException($"File not found");
            else if (authoredobj != null)
            {
                Console.WriteLine(articleBO.Title);
                Console.WriteLine(articleBO.Description);
                authoredobj.Title = articleBO.Title;
                authoredobj.Objdescription = articleBO.Description;
                authoredobj.Objcontent = articleBO.ObjContent;
                context.SaveChanges();
            }
        }

        public void DeleteArticle(int id) 
        {
            Authoredobj authoredobj = context.Authoredobj.FirstOrDefault(a => a.Id == id);
            if (authoredobj != null)
            {
                context.Authoredobj.Remove(authoredobj);
            }
            context.SaveChanges();
        }

        #endregion

        #region tags

        public TagBO GetTag(int id) 
        {
            Tag tag = context.Tag.FirstOrDefault(i => i.Id == id);
            TagBO tagBO = new TagBO();
            tagBO.Id = id;
            tagBO.Name = tag.Tagname;
            tagBO.Description = tag.Tagdescription;
            return tagBO;
        }
        /*
        public TagBO GetTagByName(string name)
        {
            TagBO tagBO = new TagBO();
            foreach (Tag t in context.Tag)
            {
                if(t.Tagname == name && name != "")
                {
                    Tag tag = t;
                    if (tag != null)
                    {
                        tagBO.Id = tag.Id;
                        tagBO.Name = tag.Tagname;
                        tagBO.Description = tag.Tagdescription;
                    }
                }
            }            
            return tagBO;
        }
        */
        public IEnumerable<TagBO> GetObjectTags(int objectid)
        {
            List<TagBO> tags = new List<TagBO>();
            foreach (Tagassociations ta in context.Tagassociations) 
            {
                if(ta.Objectid == objectid)
                {
                    tags.Add(GetTag(ta.Tagid));
                }                
            }
            return tags;
        }
        public IEnumerable<TagBO> GetTagList()
        {
            List<TagBO> tags = new List<TagBO>();
            foreach (Tag tag in context.Tag)
            {
                TagBO tagBO = new TagBO();
                tagBO.Id = tag.Id;
                tagBO.Name = tag.Tagname;
                tagBO.Description = tag.Tagdescription;
                tags.Add(tagBO);
            }
            return tags;
        }
        public IEnumerable<TagBO> GetTagListByNames(string[] names)
        {
            List<TagBO> tags = new List<TagBO>();
            foreach(string name in names)
            {                    
                    foreach (Tag t in context.Tag)
                    {
                        if (t.Tagname == name && name != "")
                        {
                            if (t != null)
                            {
                                TagBO tagBO = new TagBO();
                                tagBO.Id = t.Id;
                                tagBO.Name = t.Tagname;
                                tagBO.Description = t.Tagdescription;
                                tags.Add(tagBO);
                                Console.WriteLine("added " + tagBO.Name);
                            }
                        }                                    
                    }                          
            }
            return tags;
        }
        public bool CheckDuplicateTag(TagBO tagBO)
        {
            foreach (Tag tag in context.Tag)
            {
                if(tagBO.Name == tag.Tagname)
                return true;
            }     
            return false;
        }
        public void CreateTag (TagBO tagBO)
        {
            if(!CheckDuplicateTag(tagBO))
            {
                Tag tag = new Tag();
                tag.Id = context.Tag.MakeUniqueIdentifier();
                tag.Tagname = tagBO.Name;
                tag.Tagdescription = tagBO.Description;
                context.Add(tag);
                context.SaveChanges();
            }
        }
        public void DeleteTag(TagBO tagBO)
        {
            Tag tag = context.Tag.FirstOrDefault(i => i.Id == tagBO.Id);
            foreach (Tagassociations ta in context.Tagassociations)
            {
                if(ta.Tagid == tag.Id)
                {
                    context.Remove(ta);
                }
            }
            context.Remove(tag);
            context.SaveChanges();  
        }
        public void AlterTag (TagBO tagBO)
        {
            Tag tag = context.Tag.FirstOrDefault(i => i.Id == tagBO.Id);
            if(tag != null)
            {
                tag.Tagname = tagBO.Name;
                tag.Tagdescription = tagBO.Description;
                context.SaveChanges();
            }           
        }
        public bool CheckAssociation(TagBO tagBO, ObjectBO objectBO)
        {
            foreach(Tagassociations ta in context.Tagassociations)
            {
                Console.WriteLine("checking " + tagBO.Id.ToString() + " " + objectBO.Id.ToString());
                if(ta.Tagid != tagBO.Id && ta.Objectid != objectBO.Id)
                { return false; }
                else {  return true; }
            }
            return true;
        }
        public void ApplyTag(TagBO tagBO, ObjectBO objectBO)
        {
            if(!CheckAssociation(tagBO, objectBO))
            {
                Tagassociations ta = new Tagassociations();
                ta.Tagid = tagBO.Id;
                ta.Objectid = objectBO.Id;
                context.Add(ta);
                context.SaveChanges();
            }
        }
        public void RemoveTag(int tagid, int objid) 
        {
            Tagassociations ta = context.Tagassociations.FirstOrDefault(i => i.Tagid == tagid);
            if(ta != null && ta.Objectid == objid)
            {
                context.Remove(ta);
                context.SaveChanges();
            }
        }
        public void RemoveAssociations(int objid)
        {
            foreach(Tagassociations ta in context.Tagassociations)
            {
                if(ta.Objectid == objid)
                {
                    context.Remove(ta);
                }
            }
            context.SaveChanges();
        }
        #endregion

        #region fileassociations

        public bool CheckFileAssociation(ObjectBO parent, ObjectBO child)
        {
            foreach (Fileassociations fa in context.Fileassociations)
            {
                if (fa.Parentfileid == parent.Id && fa.Associatedid == child.Id)
                    return true;
                else return false;
            }
            return true;
        }
        public IEnumerable<ObjectBO> FetchAssociatedObjects(int parentid)
        {
            List<ObjectBO> objects = new List<ObjectBO>();
            foreach(Fileassociations fa in context.Fileassociations)
            {
                if(fa.Parentfileid == parentid)
                {
                    ObjectBO objectBO = FetchFileInfo(fa.Associatedid);
                    objects.Add(objectBO);
                }
            }
            return objects;
        }
        public void CreateAssociation(ObjectBO parent, ObjectBO child)
        {
            if (!CheckFileAssociation(parent, child))
            {
                Fileassociations fa = new Fileassociations();
                fa.Parentfileid = parent.Id;
                fa.Associatedid = child.Id;
                context.Add(fa);
                context.SaveChanges();
            }
        }
        public void RemoveFileAssociations(int objid)
        {
            foreach (Fileassociations fa in context.Fileassociations)
            {
                if(fa.Parentfileid == objid || fa.Associatedid == objid)
                {
                    context.Remove(fa);
                }
            }
            context.SaveChanges();
        }
        public void RemoveSingleAssociation(int parentid, int childid) 
        {
            foreach (Fileassociations fa in context.Fileassociations)
            {
                if (fa.Parentfileid == parentid && fa.Associatedid == childid)
                {
                    context.Remove(fa);
                }
            }
            context.SaveChanges();
        }

        #endregion
    }

}
