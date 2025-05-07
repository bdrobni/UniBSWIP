using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace dipwebapp.Models
{
    public partial class Tagassociations
    {
        public int Tagid { get; set; }
        public int Objectid { get; set; }

        public virtual Authoredobj Object { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
