using System;
using System.Collections.Generic;

#nullable disable

namespace EventProjectFinal.Models
{
    public partial class EventUser
    {
        public int EventUserId { get; set; }
        public string UserId { get; set; }

        public int EventId { get; set; }
        public string IsAttend { get; set; }

        public virtual Event Event { get; set; }
    }
}
