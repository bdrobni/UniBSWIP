using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace dipwebapp.Models
{
    public partial class Appuser
    {
        public Appuser()
        {
            Authoredobj = new HashSet<Authoredobj>();
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Pass { get; set; }
        public string Userrole { get; set; }

        public virtual ICollection<Authoredobj> Authoredobj { get; set; }
    }
}
