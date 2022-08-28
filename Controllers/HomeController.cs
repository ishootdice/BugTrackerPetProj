using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BugTrackerPetProj.Models;
using BugTrackerPetProj.Interfaces;
using BugTrackerPetProj.ViewModels.Home;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Org.BouncyCastle.Crypto.Tls;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.ComponentModel;

namespace BugTrackerPetProj.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly IRepository _repository;

    private readonly IEmailService _emailService;

    private readonly AppDbContext _context;

    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly UserManager<User> _userManager;

    private readonly SignInManager<User> _signInManager;

    public HomeController(ILogger<HomeController> logger, IRepository repository, AppDbContext context, IEmailService emailService,
        IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _logger = logger;
        _repository = repository;
        _context = context;
        _emailService = emailService;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model = new Project();
        User user = await _userManager.GetUserAsync(User);
        Company? company = _repository.GetCompanyByUserId(user.Id);
        model.Company = new Company();
        bool isCompanyMember = false;

        if(company != null)
        {
            isCompanyMember = true;
            model.Company = company;
            model.UsersToAssign = GetSelectListItems(company.Users.ToList(), user.Id, false);
        }

        model.Projects = _repository.GetUserProjects(user.Id);

        if(model.Projects != null)
        {
            model.PieChartForType = JsonConvert.SerializeObject(GetPieChartByType(model.Projects.ToList()));
            model.PieChartForStatus = JsonConvert.SerializeObject(GetPieChartByStatus(model.Projects.ToList()));
            model.PieChartForPriority = JsonConvert.SerializeObject(GetPieChartByPriority(model.Projects.ToList()));
        }

        ViewBag.IsCompanyMember = isCompanyMember;
        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddProject(Project model)
    {
        if (ModelState.IsValid)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Project project = new Project { ProjectName = model.ProjectName, ProjectDescription = model.ProjectDescription };

            if (User.IsInRole("Admin")) project.Company = _repository.GetCompanyByUserId(userId);

            if (model.UsersIds.Any())
            {
                foreach(var id in model.UsersIds)
                {
                    User user = _repository.GetUserById(id);
                    _repository.AddProjectToUser(user.Id, project);
                }
            }

            _repository.AddProjectToUser(userId, project);
            return RedirectToAction("Index", "Home");
        }

        return View();
    }


    public IActionResult DeleteProject(int id)
    {
        _repository.DeleteProject(id);
        return RedirectToAction("Index", "Home");
    }


    [HttpGet]
    public IActionResult ProjectInformation(int id)
    {
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Project project = _repository.GetProject(id);
        Ticket model = new Ticket { Project = project, ProjectId = project.Id};

        if(project.Users != null) model.UsersToAssign = GetSelectListItems(project.Users.ToList(), userId, true);

        model.Tickets = new List<Ticket>();
        model.Users = new List<User>();

        if (project.Tickets != null) model.Tickets = project.Tickets;

        return View(model);
    }

    [HttpPost]
    public IActionResult TicketInformation(int id)
    {
        Ticket ticket = _repository.GetTicketById(id);
        ticket.TimePassedFromCreation = GetTimePassedFromCreation(ticket.CreationTime);

        return PartialView("_ticketInfo", ticket);
    }

    [HttpPost]
    public IActionResult TicketComments(int id)
    {
        Ticket ticket = _repository.GetTicketById(id);

        return PartialView("_commentsSection", ticket);
    }

    [HttpPost]
    public IActionResult ProjectTickets(int ticketsPageIndex, int projectId)
    {
        var elementsToSkip = ticketsPageIndex - 1;
        var project = _repository.GetProject(projectId);

        //List<Ticket> tickets = project.Tickets.Skip(elementsToSkip * 7).Take(7).ToList();
        List<Ticket> tickets = project.Tickets.ToList();

        ViewBag.StartIndex = elementsToSkip;
        ViewBag.LastIndex = elementsToSkip + 7;

        return PartialView("_projectTickets", tickets);
    }



    [HttpPost]
    public IActionResult AddTicket(Ticket model)
    {
        if (ModelState.IsValid)
        {
            Ticket ticket = new Ticket
            {
                ProjectId = model.ProjectId,
                Title = model.Title,
                Description = model.Description,
                Author = model.Author,
                Priority = model.Priority,
                Status = model.Status,
                Type = model.Type,
                CreationTime = DateTime.Now
                //TimeEstimate = model.TimeEstimate
            };

            if(model.UsersIds != null)
            {
                foreach(var id in model.UsersIds)
                {
                    ticket.Users.Add(_repository.GetUserById(id));
                }
            }

            var project = _repository.GetProject(model.ProjectId);
            project.Tickets.Add(ticket);
            _repository.UpdateProject(project);

            return RedirectToAction("ProjectInformation", "Home", new { id = model.ProjectId });
        }

        return RedirectToAction("ProjectInformation", "Home", new { id = model.ProjectId });
    }



    [HttpPost]
    public IActionResult TicketInfo(int id, int project)
    {
        var proj = _repository.GetProject(project);
        var ticket = proj.Tickets.FirstOrDefault(t => t.Id == id);
        //ticket.TimePassedFromCreation = GetTimePassedFromCreation(DateTime.Now);
        return PartialView("_ticketInfo", ticket);
    }

    [HttpPost]
    public IActionResult AddComment(int id, string message, string author)
    {
        var ticket = _repository.GetTicketById(id);

        var comment = new Comment { Text = message, Author = author, Ticket = ticket, TicketId = ticket.Id, Date = DateTime.Now };
        if(comment == null) return View();

        ticket.Comments.Add(comment);
        Ticket ticketToPass = _repository.UpdateTicket(ticket);
        
        return PartialView("_commentsSection", ticketToPass);
    }

    [HttpGet]
    public JsonResult EditProject(int id)
    {
        var project = _repository.GetProject(id);
        Project projectModelForView = new Project { Id = project.Id, ProjectName = project.ProjectName, ProjectDescription = project.ProjectDescription };
        return Json(projectModelForView);
    }

    [HttpPost]
    public JsonResult UpdateProject([Bind("Id, ProjectName, ProjectDescription")]Project model)
    {
        _repository.UpdateProject(model);
        return Json(true);
    }

    [HttpGet]
    public JsonResult UpdateTicket(int id)
    {
        var ticket = _repository.GetTicketById(id);
        var ticketModelForView = new Ticket
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            Author = ticket.Author,
            Status = ticket.Status,
            Type = ticket.Type,
            Priority = ticket.Priority,
            ProjectId = ticket.ProjectId
        };

        return Json(ticketModelForView);
    }

    [HttpPost]
    public IActionResult UpdateTicket([Bind("Id, Title, Description, Status, Type, Priority, Author, ProjectId")]Ticket model)
    {
        _repository.UpdateTicket(model);
        return RedirectToAction("Tickets", "Home");
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


    [HttpPost]
    public IActionResult ShowAddTicketForm(int projectId)
    {
        var model = new AddTicketViewModel { ProjectId = projectId };
        return PartialView("_addTicketPartialView", model);
    }

    [HttpGet]
    public IActionResult CreateCompany()
    {
        Company model = new Company();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCompany(Company model)
    {
        if (ModelState.IsValid)
        {
            User user = await _userManager.GetUserAsync(User);
            var company = _repository.AddCompany(user.Id, model);
            var result = await _userManager.AddToRoleAsync(user, "Admin");

            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(user, isPersistent:false);

            if (result.Succeeded) return RedirectToAction("Index", "Administration");

            foreach (var err in result.Errors)
            {
                ModelState.AddModelError("", err.Description);
            }

            return View("Error");
        }

        return View(model);
    }

    [AcceptVerbs("Post", "Get")]
    [AllowAnonymous]
    public IActionResult IsCompanyEmailInUse(string email)
    {
        var company = _repository.GetCompanyByEmail(email);

        if(company == null) return Json(true);

        return Json($"Email {email} is already in use");
    }

    [HttpGet]
    public async Task<IActionResult> Tickets()
    {
        User user = await _userManager.GetUserAsync(User);
        Ticket model = new Ticket();
        model.Tickets = _repository.GetUserTickets(user.Id);

        foreach(var ticket in model.Tickets)
        {
            var project = _repository.GetProject(ticket.ProjectId);
            ticket.Project = project;
        }

        return View(model);
    }

    public static List<SelectListItem> GetSelectListItems(List<User> users, string adminId, bool withAdmin)
    {
        
        List<SelectListItem> companyMembers = new List<SelectListItem>();
        foreach(User user in users)
        {
            companyMembers.Add(new SelectListItem
            {
                Text = user.UserName,
                Value = user.Id
            });

        }
        if (!withAdmin) companyMembers.Remove(companyMembers.Where(x => x.Value == adminId).Single());

        return companyMembers;
    }


    private PieChart GetPieChartByType(List<Project> projects)
    {
        var model = new PieChart();
        var childModel = new ChildPieChart();

        TicketType[] types = (TicketType[])Enum.GetValues(typeof(TicketType));
        foreach (var type in types)
        {
            model.labels.Add(GetDescriptionFromEnum(type));
        }

        int bugCount = 0;
        int issueCount = 0;
        int featureRequestCount = 0;

        foreach (var project in projects)
        {
            if (project.Tickets != null)
            {
                foreach (var ticket in project.Tickets)
                {
                    switch (ticket.Type)
                    {
                        case TicketType.Bug:
                            bugCount++;
                            break;
                        case TicketType.Issue:
                            issueCount++;
                            break;
                        case TicketType.FeatureRequest:
                            featureRequestCount++;
                            break;
                    }
                }
            }
        }

        foreach (var label in model.labels)
        {
            switch (label)
            {
                case "Bug":
                    childModel.backgroundColor.Add("rgba(67, 132, 178, 0.7)");
                    childModel.borderColor.Add("#181d2f");
                    childModel.data.Add(bugCount);
                    break;
                case "Issue":
                    childModel.backgroundColor.Add("rgba(95, 191, 228, 0.7)");
                    childModel.borderColor.Add("#181d2f");
                    childModel.data.Add(issueCount);
                    break;
                case "Feature request":
                    childModel.backgroundColor.Add("rgba(181, 246, 233, 0.7)");
                    childModel.borderColor.Add("#181d2f");
                    childModel.data.Add(featureRequestCount);
                    break;
            }
        }

        model.datasets.Add(childModel);
        return model;
    }

    private PieChart GetPieChartByStatus(List<Project> projects)
    {
        var model = new PieChart();
        var childModel = new ChildPieChart();

        TicketStatus[] statuses = (TicketStatus[])Enum.GetValues(typeof(TicketStatus));
        foreach (var status in statuses)
        {
            model.labels.Add(GetDescriptionFromEnum(status));
        }

        int resolvedCount = 0;
        int newCount = 0;
        int inProgressCount = 0;

        foreach (var project in projects)
        {
            if (project.Tickets != null)
            {
                foreach (var ticket in project.Tickets)
                {
                    switch (ticket.Status)
                    {
                        case TicketStatus.Resolved:
                            resolvedCount++;
                            break;
                        case TicketStatus.New:
                            newCount++;
                            break;
                        case TicketStatus.InProgress:
                            inProgressCount++;
                            break;
                    }
                }
            }
        }

        foreach (var label in model.labels)
        {
            switch (label)
            {
                case "Resolved":

                    childModel.backgroundColor.Add("rgba(115, 62, 186, 0.7)");
                    childModel.borderColor.Add("#181d2f");
                    childModel.data.Add(resolvedCount);
                    break;
                case "New":
                    childModel.backgroundColor.Add("rgba(203, 86, 174, 0.7)");
                    childModel.borderColor.Add("#181d2f");
                    childModel.data.Add(newCount);
                    break;
                case "In progress":
                    childModel.backgroundColor.Add("rgba(219, 122, 109, 0.7)");
                    childModel.borderColor.Add("#181d2f");
                    childModel.data.Add(inProgressCount);
                    break;
            }
        }

        model.datasets.Add(childModel);
        return model;
    }

    private PieChart GetPieChartByPriority(List<Project> projects)
    {
        var model = new PieChart();
        var childModel = new ChildPieChart();

        TicketPriority[] priorities = (TicketPriority[])Enum.GetValues(typeof(TicketPriority));
        foreach (var priority in priorities)
        {
            model.labels.Add(priority.ToString());
        }

        int highPriorityCount = 0;
        int mediumPriorityCount = 0;
        int lowPriorityCount = 0;
        int immediatePriorityCount = 0;

        foreach (var project in projects)
        {
            if (project.Tickets != null)
            {
                foreach (var ticket in project.Tickets)
                {
                    switch (ticket.Priority)
                    {
                        case TicketPriority.High:
                            highPriorityCount++;
                            break;
                        case TicketPriority.Medium:
                            mediumPriorityCount++;
                            break;
                        case TicketPriority.Low:
                            lowPriorityCount++;
                            break;
                        case TicketPriority.Immediate:
                            immediatePriorityCount++;
                            break;
                    }
                }
            }
        }

        foreach (var label in model.labels)
        {
            switch (label)
            {
                case "High":
                    childModel.backgroundColor.Add("rgba(176, 52, 43, 0.7)");
                    childModel.borderColor.Add("#181d2f");
                    childModel.data.Add(highPriorityCount);
                    break;
                case "Medium":
                    childModel.backgroundColor.Add("rgba(213, 99, 61, 0.7)");
                    childModel.borderColor.Add("#181d2f");
                    childModel.data.Add(mediumPriorityCount);
                    break;
                case "Low":
                    childModel.backgroundColor.Add("rgba(227, 191, 85, 0.7)");
                    childModel.borderColor.Add("#181d2f");
                    childModel.data.Add(lowPriorityCount);
                    break;
                case "Immediate":
                    childModel.backgroundColor.Add("rgba(229, 227, 104, 0.7)");
                    childModel.borderColor.Add("#181d2f");
                    childModel.data.Add(immediatePriorityCount);
                    break;
            }
        }

        model.datasets.Add(childModel);
        return model;
    }

    public static string GetDescriptionFromEnum(Enum value)
    {
        DescriptionAttribute attribute = value.GetType()
        .GetField(value.ToString())
        .GetCustomAttributes(typeof(DescriptionAttribute), false)
        .SingleOrDefault() as DescriptionAttribute;
        return attribute == null ? value.ToString() : attribute.Description;
    }

    public static string GetTimePassedFromCreation(DateTime creationTime)
    {
        TimeSpan span = (DateTime.Now - creationTime);

        List<int> timeArray = new List<int>();
        timeArray.Add(span.Days);
        timeArray.Add(span.Hours);
        timeArray.Add(span.Minutes);
        timeArray.Add(span.Seconds);

        int count = 0;
        for(int i = 0; i < timeArray.Count; i++)
        {
            if (timeArray[i] == 0) count++;
            else break;
        }

        string timePassed = "";
        switch (count)
        {
            case 0:
                timePassed = $"{timeArray[0]} days ago";
                break;
            case 1:
                timePassed = $"{timeArray[1]} hours ago";
                break;
            case 2:
                timePassed = $"{timeArray[2]} minutes ago";
                break;
            case 3:
                timePassed = $"{timeArray[3]} seconds ago";
                break;
        }

        return timePassed;
    }

}

