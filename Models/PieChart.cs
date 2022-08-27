using System;
namespace BugTrackerPetProj.Models
{
    public class PieChart
    {
        public PieChart()
        {
            labels = new List<string>();
            datasets = new List<ChildPieChart>();
        }

        public List<string> labels { get; set; }

        public List<ChildPieChart> datasets { get; set; }
    }

    public class ChildPieChart
    {
        public ChildPieChart()
        {
            backgroundColor = new List<string>();
            borderColor = new List<string>();
            data = new List<int>();
        }
        public string label { get; set; }

        public List<string> backgroundColor { get; set; }

        public List<string> borderColor { get; set; }

        public List<int> data { get; set; }
    }
}

