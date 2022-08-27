using System;
using System.ComponentModel;

namespace BugTrackerPetProj.Models
{
    public enum TicketType
    {
        Bug,
        Issue,
        [Description("Feature request")]
        FeatureRequest
    }
}

