using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace dipwebapp.Models
{
    public partial class Authoredobj
    {
        public Authoredobj()
        {
            FileassociationsAssociated = new HashSet<Fileassociations>();
            FileassociationsParentfile = new HashSet<Fileassociations>();
            Tagassociations = new HashSet<Tagassociations>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Objdescription { get; set; }
        public string Objcontent { get; set; }
        public DateTime Createddate { get; set; }
        public int Authorid { get; set; }
        public string Filetype { get; set; }

        public virtual Appuser Author { get; set; }
        public virtual ICollection<Fileassociations> FileassociationsAssociated { get; set; }
        public virtual ICollection<Fileassociations> FileassociationsParentfile { get; set; }
        public virtual ICollection<Tagassociations> Tagassociations { get; set; }
    }
}
