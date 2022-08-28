using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugTrackerPetProj.Interfaces;
using BugTrackerPetProj.Models;
using BugTrackerPetProj.ViewModels.Administration;
using BugTrackerPetProj.ViewModels.Home;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BugTrackerPetProj.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly UserManager<User> _userManager;

        private readonly SignInManager<User> _signInManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IEmailService _emailService;

        private readonly IRepository _repository;

        private readonly IEncryptionService _encryptionService;

        public AdministrationController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IEmailService emailService,
            IRepository repository, IEncryptionService encryptionService, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _repository = repository;
            _encryptionService = encryptionService;
            _signInManager = signInManager;
        }

        // GET: /<controller>/
        //public async Task<IActionResult> Index()
        //{
        //    var users = _userManager.Users;

        //    return View(users);
        //}

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            Tuple<List<User>, User> tuple;
            //Company company = _repository.GetCompanyBuId(companyId);
            User user = await _userManager.GetUserAsync(User);
            Company? company = _repository.GetCompanyByUserId(user.Id);
            if (company.Users.Any())
            {
                user = company.Users.ToList()[0];
            }

            ViewBag.CompanyId = company.Id;
            tuple = new Tuple<List<User>, User>(company.Users.ToList(), user);
            return View(tuple);
        }




        [HttpPost]
        public async Task<IActionResult> AddMember([Bind("Email, Id")]AddMemberViewModel model)
        {
            var user = _repository.GetUserByEmail(model.Email);
            var IdToString = Convert.ToString(model.Id);

            User sender = await _userManager.GetUserAsync(User);
            bool isAdmin = await _userManager.IsInRoleAsync(sender, "Admin");

            if (ModelState.IsValid && user != null)
            {
                var userEmail = _encryptionService.Encrypt(user.Email);
                var Id = _encryptionService.Encrypt(IdToString);
                string uniqueInviteUrl = $"{userEmail}-{Id}";
                _emailService.SendInviteEmail(model.Email, uniqueInviteUrl, isAdmin);

                if (isAdmin)
                {
                    return RedirectToAction("Index", "Administration");
                }

                return RedirectToAction("ProjectInformation", "Home", new { id = model.Id });
            }


            ModelState.AddModelError(string.Empty, "There are no such user");
            return View(model);
        }

        public IActionResult InviteConfirmation(string userData)
        {
            string userEmail = userData.Substring(0, userData.IndexOf("-"));
            string projectId = userData.Substring(userData.LastIndexOf("-") + 1);
            string decryptedEmail = _encryptionService.Decrypt(userEmail);
            string decryptedId = _encryptionService.Decrypt(projectId);

            int projectIdToInt = Convert.ToInt32(decryptedId);

            var project = _repository.GetProject(projectIdToInt);
            if(project != null)
            {
                var user = _repository.GetUserByEmail(decryptedEmail);
                if(user != null)
                {
                    project.Users.Add(user);
                    _repository.UpdateProject(project);
                    return RedirectToAction("ProjectInformation", "Home", new { id = projectIdToInt });
                }
                return View("Error");
            }

            return View("Error");
        }

        [HttpGet]
        public async Task<IActionResult> CompanyInviteConfirmation(string userData)
        {
            string userEmail = userData.Substring(0, userData.IndexOf("-"));
            string companyId = userData.Substring(userData.LastIndexOf("-") + 1);
            string decryptedEmail = _encryptionService.Decrypt(userEmail);
            string decryptedId = _encryptionService.Decrypt(companyId);

            int companyIdToInt = Convert.ToInt32(decryptedId);

            var company = _repository.GetCompanyById(companyIdToInt);
            if (company != null)
            {
                var user = _repository.GetUserByEmail(decryptedEmail);
                if (user != null)
                {
                    company.Users.Add(user);
                    _repository.UpdateCompany(company);

                    if (_signInManager.IsSignedIn(User)) await _signInManager.SignOutAsync();

                    return RedirectToAction("Login", "Account", new {isAfterConfirmation = true});
                }
                return View("Error");
            }

            return View("Error");
        }

        [HttpPost]
        public IActionResult MemberInformation(string id)
        {
            User? user = _repository.GetUserById(id);

            return PartialView("_userInformationPartialView", user);
        }

        [HttpPost]
        public IActionResult UpdateMember([Bind("Id, UserName, Email, PhoneNumber")]User updatedMember)
        {
            //User user = await _userManager.FindByIdAsync(updatedMember.Id);
            User user = _repository.GetUserById(updatedMember.Id);

            if (user != null)
            {
                user.UserName = updatedMember.UserName;
                user.Email = updatedMember.Email;
                user.PhoneNumber = updatedMember.PhoneNumber;
                _repository.UpdateUser(user);
                return RedirectToAction("Index", "Administration");
                //return PartialView("_userInformationPartialView", user);
            }


            return RedirectToAction("MemberInformation", "Administration", new {id = user.Id});
        }

        public async Task<IActionResult> RemoveMember(string id)
        {
            User userToRemove = _repository.GetUserById(id);
            Company company = _repository.GetCompanyByUserId(id);
            

            company.Users.Remove(userToRemove);
            _repository.UpdateCompany(company);

            return RedirectToAction("Index", "Administration");
        }
    }
}

