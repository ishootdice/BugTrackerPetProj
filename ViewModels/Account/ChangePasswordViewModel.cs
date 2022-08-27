using System;
using System.ComponentModel.DataAnnotations;

namespace BugTrackerPetProj.ViewModels.Account
{
	public class ChangePasswordViewModel
	{
        public string Id { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage =
            "The new password and confirmation password do not much")]
        public string ConfirmNewPassword { get; set; }
    }
}

