using System;
using System.ComponentModel;

namespace BugTrackerPetProj.Models
{
    public enum TicketStatus
    {
        Resolved,
        New,
        [Description("In progress")]
        InProgress
    }
}

