using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTrackerPetProj.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        [Display(Name ="Project name")]
        [MinLength(5, ErrorMessage = "Project name must have at least 5 characters")]
        public string ProjectName { get; set; }

        [Required]
        [Display(Name ="Project description")]
        [MinLength(10, ErrorMessage = "Project description name must have at least 10 characters")]
        public string ProjectDescription { get; set; }

        public Company? Company { get; set; }

        public int? CompanyId { get; set; }

        [NotMapped]
        public List<SelectListItem> UsersToAssign { get; set; }

        [NotMapped]
        public string[] UsersIds { get; set; }

        [NotMapped]
        public string PieChartForType { get; set; }

        [NotMapped]
        public string PieChartForStatus { get; set; }

        [NotMapped]
        public string PieChartForPriority { get; set; }

        public virtual ICollection<User>? Users { get; set; }

        public virtual ICollection<Ticket>? Tickets { get; set; }

        [NotMapped]
        public virtual ICollection<Project>? Projects { get; set; } 

        public Project()
        {
            Users = new List<User>();
            Tickets = new List<Ticket>();
        }
    }
}

