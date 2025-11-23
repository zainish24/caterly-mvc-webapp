using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalCaterers { get; set; }
        public int TotalBookings { get; set; }
        public int TotalMessages { get; set; }
        public List<Activity> RecentActivities { get; set; }
    }

    public class Activity
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
}