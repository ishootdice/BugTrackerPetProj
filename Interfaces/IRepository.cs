using System;
using BugTrackerPetProj.Models;

namespace BugTrackerPetProj.Interfaces
{
    public interface IRepository
    {
        User GetUserById(string id);

        void UpdateUser(User updatedUser);

        Project AddProject(Project project);

        Project UpdateProject(Project project);

        Project GetProject(int id);

        List<Project> GetAllProjects();

        Project DeleteProject(int id);

        User GetUserByEmail(string email);

        Ticket GetTicketById(int id);

        Ticket UpdateTicket(Ticket ticketChanges);

        List<Project> GetUserProjects(string id);

        void AddProjectToUser(string id, Project project);

        Company AddCompany(string userId, Company company);

        Company GetCompanyByUserId(string id);

        Company GetCompanyById(int id);

        Company? GetCompanyByEmail(string email);

        void UpdateCompany(Company companyUpdates);

        List<Ticket> GetUserTickets(string userId);

        User GetUserByName(string name);
    }
}

