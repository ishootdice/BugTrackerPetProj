using System;
using Microsoft.AspNetCore.Identity;

namespace BugTrackerPetProj.Models
{
    public class User : IdentityUser
    {
        public virtual ICollection<Project> Projects { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }

        public Company? Company { get; set; }

        public User()
        {
            this.Projects = new List<Project>();
            this.Tickets = new List<Ticket>();
        }
    }
}

