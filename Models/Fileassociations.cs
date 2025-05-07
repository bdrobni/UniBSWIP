using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace dipwebapp.Models
{
    public partial class Fileassociations
    {
        public int Parentfileid { get; set; }
        public int Associatedid { get; set; }

        public virtual Authoredobj Associated { get; set; }
        public virtual Authoredobj Parentfile { get; set; }
    }
}
