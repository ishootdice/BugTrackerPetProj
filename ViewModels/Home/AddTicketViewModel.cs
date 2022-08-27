using System;
using System.ComponentModel.DataAnnotations;
using BugTrackerPetProj.Models;

namespace BugTrackerPetProj.ViewModels.Home
{
    public class AddTicketViewModel
    {
        [Required]
        public int ProjectId { get; set; }

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
    }
}

