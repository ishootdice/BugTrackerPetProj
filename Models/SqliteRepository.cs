using System;
using BugTrackerPetProj.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BugTrackerPetProj.Models
{
    public class SqliteRepository : IRepository
    {
        private readonly AppDbContext _db;

        public SqliteRepository(AppDbContext db)
        {
            _db = db;
        }

        public Project AddProject(Project project)
        {
            _db.Add(project);
            _db.SaveChanges();
            return project;
        }

        public Project DeleteProject(int id)
        {
            var project = _db.Projects.Find(id);
            if (project != null)
            {
                _db.Projects.Remove(project);
                _db.SaveChanges();
            }

            return project;
        }

        public User GetUserByEmail(string email)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            return user;
        }

        public List<Project> GetAllProjects()
        {
            return _db.Projects.ToList();
        }

        public Project GetProject(int id)
        {
            //var project = _db.Projects.FirstOrDefault(p => p.Id == id);
            var project = _db.Projects.Include(x => x.Users).Include(x => x.Tickets).FirstOrDefault(p => p.Id == id);

            return project;
        }

        public Project UpdateProject(Project projectChanges)
        {
            
            var project = _db.Projects.Attach(projectChanges);
            project.State = EntityState.Modified;
            _db.SaveChanges();
            return projectChanges;
        }

        public Ticket UpdateTicket(Ticket ticketChanges)
        {
            var ticket = _db.Attach(ticketChanges);
            //var ticketFromDb = _db.Tickets.Include(x => x.Comments).Include(x => x.Users).FirstOrDefault(t => t.Id == ticketChanges.Id);

            //ticketFromDb.Title = ticketChanges.Title;
            //ticketFromDb.Description = ticketChanges.Description;
            //ticketFromDb.Type = ticketChanges.Type;
            //ticketFromDb.Status = ticketChanges.Status;
            //ticketFromDb.Priority = ticketChanges.Priority;

            //var ticket = _db.Attach(ticketFromDb);
            ticket.State = EntityState.Modified;
            _db.SaveChanges();
            return ticketChanges;
        }

        public Ticket GetTicketById(int id)
        {
            return _db.Tickets.Include(x => x.Comments).Include(x => x.Users).FirstOrDefault(t => t.Id == id);
        }

        public List<Project> GetUserProjects(string id)
        {
            var user = _db.Users.Include(x => x.Projects).Include(x => x.Tickets).FirstOrDefault(u => u.Id == id);
            return user.Projects.ToList();
        }

        public void AddProjectToUser(string id, Project project)
        {
            var user = _db.Users.Include(x => x.Projects).FirstOrDefault(u => u.Id == id);
            user.Projects.Add(project);
            _db.SaveChanges();
        }

        public Company AddCompany(string userId, Company company)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == userId);
            company.Users.Add(user);
            user.Company = company;
            _db.Companies.Add(company);
            _db.SaveChanges();

            var companyFromDb = _db.Companies.FirstOrDefault(c => c.Name == company.Name);
            return companyFromDb;
        }

        public Company GetCompanyByUserId(string id)
        {
            User? user = _db.Users.Include(x => x.Company).FirstOrDefault(u => u.Id == id);
            Company? company = null;
            if(user.Company != null)
            {
                company = _db.Companies.Include(x => x.Users).FirstOrDefault(c => c.Id == user.Company.Id);
            }
            return company;
        }

        public Company GetCompanyById(int id)
        {
            return _db.Companies.FirstOrDefault(c => c.Id == id);
        }

        public void UpdateCompany(Company companyUpdates)
        {
            var company = _db.Attach(companyUpdates);
            company.State = EntityState.Modified;
            _db.SaveChanges();
        }

        public User GetUserById(string id)
        {
            User user = _db.Users.FirstOrDefault(u => u.Id == id);
            return user;
        }

        public void UpdateUser(User updatedUser)
        {
            var user = _db.Attach(updatedUser);
            user.State = EntityState.Modified;
            _db.SaveChanges();
        }

        public List<Ticket> GetUserTickets(string userId)
        {
            User? user = _db.Users.Include(x => x.Tickets).FirstOrDefault(u => u.Id == userId);
            
            return user.Tickets.ToList();
        }

        public User GetUserByName(string name)
        {
            string? firstName = null;
            string? lastName = null;
            if (name.Contains(" "))
            {
                firstName = name.Substring(0, name.IndexOf(" "));
                lastName = name.Substring(name.LastIndexOf(" ") + 1);
                name = firstName + lastName;
            }
            User? user = _db.Users.FirstOrDefault(u => u.UserName == name);
            return user;
        }

        public Company? GetCompanyByEmail(string email)
        {
            return _db.Companies.FirstOrDefault(c => c.Email == email);
        }
    }
}

