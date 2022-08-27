using System;
using System.ComponentModel.DataAnnotations;

namespace BugTrackerPetProj.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public string Text { get; set; }

        public Ticket Ticket { get; set; }

        public DateTime Date { get; set; }

        public int TicketId { get; set; }
    }
}

