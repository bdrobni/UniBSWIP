﻿using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace dipwebapp.Models
{
    public partial class Tag
    {
        public Tag()
        {
            Tagassociations = new HashSet<Tagassociations>();
        }

        public int Id { get; set; }
        public string Tagname { get; set; }
        public string Tagdescription { get; set; }

        public virtual ICollection<Tagassociations> Tagassociations { get; set; }
    }
}
