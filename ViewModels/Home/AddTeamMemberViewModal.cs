using System;
using System.ComponentModel.DataAnnotations;

namespace BugTrackerPetProj.ViewModels.Home
{
    public class AddTeamMemberViewModal
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public int ProjectId { get; set; }
    }
}

