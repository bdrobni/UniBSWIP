using System.Reflection.Metadata;
using XAct;
using XAct.Users;

namespace dipwebapp.Models.Repository
{
    public class AdminRepository
    {

        diplomskidbContext context = new diplomskidbContext();
        SiteRepository _siteRepository = new SiteRepository();

        #region users
        public IEnumerable<UserBO> ShowUsers()
        {
            List<UserBO> users = new List<UserBO>();
            foreach(Appuser u in context.Appuser)
            {
                UserBO userBO = new UserBO();
                userBO.Id = u.Id;
                userBO.Username = u.Username;
                userBO.Email = u.Email;
                userBO.UserRole = u.Userrole;

                users.Add(userBO);
            }
            return users;
        }

        public void AlterUser(UserBO userBO)
        {
            var user = context.Appuser.FirstOrDefault(u => u.Id == userBO.Id);
            if (user == null)
                throw new InvalidOperationException($"User with Id={userBO.Id} not found");

            user.Username = userBO.Username;
            user.Email = userBO.Email;
            user.Userrole = userBO.UserRole;

            context.SaveChanges();
        }
        
        public void DeleteUser(int id)
        {
            Appuser user = context.Appuser.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                foreach(Authoredobj obj in context.Authoredobj)
                {
                    if(obj.Authorid == id && obj.Filetype == "article")
                    {
                        _siteRepository.DeleteArticle(obj.Id);
                    }
                    else if (obj.Authorid == id)
                    {
                        _siteRepository.DeleteFile(obj.Id); 
                    }
                }
                context.Appuser.Remove(user);
                context.SaveChanges();
            }
        }
        #endregion

        #region tags
        public bool CheckTagExists(TagBO tagBO)
        {
            foreach(Tag tag in context.Tag)
            {
                if(tagBO.Name == tag.Tagname)
                    return true;
                else return false;
            }
            return false;
        }

        public void CreateTag(TagBO tagBO) 
        {
            Tag tag = new Tag();
            tag.Id = context.Tag.MakeUniqueIdentifier();
            tag.Tagname = tagBO.Name;
            tag.Tagdescription = tagBO.Description;
            context.Add(tag);
            context.SaveChanges();
        }

        public void DeleteTag(int id) 
        {
            Tag tag = context.Tag.FirstOrDefault(t => t.Id == id);
            if (tag != null) 
            {
                foreach (Tagassociations assoc in context.Tagassociations) 
                {
                    if (assoc.Tagid == id) 
                    {
                        context.Tagassociations.Remove(assoc);
                    }
                }
                context.Tag.Remove(tag);
                context.SaveChanges();
            }
        }

        public void AlterTag(TagBO tagBO)
        {
            Tag tag = context.Tag.FirstOrDefault(t => t.Id == tagBO.Id);
            if (tag != null) 
            {
                tag.Tagname = tagBO.Name;
                tag.Tagdescription = tagBO.Description;
            }
        }

        public void AddTagAssociation(int tagid, int objectid)
        {           
            Tagassociations assoc = new Tagassociations();
            assoc.Tagid = tagid;
            assoc.Objectid = objectid;
            context.Add(assoc);
            context.SaveChanges();
        }

        #endregion
        
    }
        
}
