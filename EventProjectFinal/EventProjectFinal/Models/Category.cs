using System;
using System.Collections.Generic;

#nullable disable

namespace EventProjectFinal.Models
{
    public partial class Category
    {
        public Category()
        {
            Events = new HashSet<Event>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}
