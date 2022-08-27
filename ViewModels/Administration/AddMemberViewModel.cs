using System;
using System.ComponentModel.DataAnnotations;

namespace BugTrackerPetProj.ViewModels.Administration
{
    public class AddMemberViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public int Id { get; set; }
    }
}

