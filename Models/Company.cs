using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace BugTrackerPetProj.Models
{
    public class Company
    {
        public int Id { get; set; }

        [Required]
        [MinLength(5, ErrorMessage ="Company name must have at least 5 characters")]
        public string Name { get; set; }

        [Required]
        [Display(Name="Country of company location")]
        public string LocationCountry { get; set; }

        [Required]
        [Display(Name ="Company adress")]
        public string LocationAdress { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name ="Corporative email adress")]
        [Remote(action:"IsCompanyEmailInUse", controller:"Home")]
        public string Email { get; set; }

        public virtual ICollection<Project> Projects { get; set; }

        public virtual ICollection<User> Users { get; set; }

        public Company()
        {
            Users = new List<User>();
            Projects = new List<Project>();
        }
    }
}

