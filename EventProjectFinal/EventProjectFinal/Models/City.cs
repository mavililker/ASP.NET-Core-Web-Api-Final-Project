using System;
using System.Collections.Generic;

#nullable disable

namespace EventProjectFinal.Models
{
    public partial class City
    {
        public City()
        {
            Events = new HashSet<Event>();
        }

        public int CityId { get; set; }
        public string CityName { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}
