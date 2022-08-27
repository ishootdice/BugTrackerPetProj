using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;

namespace BugTrackerPetProj.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        public bool RememberMe { get; set; }

        public IList<AuthenticationScheme>? ExternalLogins { get; set; }
    }
}


