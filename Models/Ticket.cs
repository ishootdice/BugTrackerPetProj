using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTrackerPetProj.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required]
        [MinLength(5, ErrorMessage = "Ticket Title must have at least 5 characters")]
        public string Title { get; set; }

        [MinLength(10, ErrorMessage = "Ticket Title must have at least 10 characters")]
        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime TimeEstimate { get; set; }

        [Required]
        public TicketType Type { get; set; }

        [Required]
        public TicketPriority Priority { get; set; }

        [Required]
        public TicketStatus Status { get; set; }

        public string Author { get; set; }

        [ValidateNever]
        public Project Project { get; set; }

        public int ProjectId { get; set; }

        [NotMapped]
        [ValidateNever]
        public List<SelectListItem> UsersToAssign { get; set; }

        [NotMapped]
        public string[] UsersIds { get; set; }

        [NotMapped]
        [ValidateNever]
        public virtual ICollection<Ticket> Tickets { get; set; }

        public virtual ICollection<User> Users { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public Ticket()
        {
            this.Users = new List<User>();
            this.Comments = new List<Comment>();
        }
    }
}

