using System;
namespace BugTrackerPetProj.ViewModels.Home
{
    public class ProjectViewModel
    {
        public int ProjectId { get; set; }

        public List<ProjectViewModel> Projects { get; set; }
    }
}

