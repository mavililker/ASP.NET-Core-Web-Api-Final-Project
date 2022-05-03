using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace EventProjectFinal.Models
{
    public partial class Event
    {
        public Event()
        {
            EventUsers = new HashSet<EventUser>();
        }

        public int EventId { get; set; }
        public string EventName { get; set; }
        public DateTime FinalDate { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public int Capacity { get; set; }
        public string IsTicket { get; set; }
        public int CityId { get; set; }
        public int CategoryId { get; set; }
        public string Price { get; set; }
        public string Address { get; set; }
        public string IsCanceled { get; set; }
        public string OrganizatorID { get; set; }



        public virtual Category Category { get; set; }
        public virtual City City { get; set; }
        public virtual ICollection<EventUser> EventUsers { get; set; }

        //Conditional Validation
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (IsTicket == "Yes" && Price == null)
            {
                yield return new ValidationResult("You have to enter price.");
            }
        }
    }
}
