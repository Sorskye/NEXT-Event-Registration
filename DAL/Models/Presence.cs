using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models
{
    public class PresenceViewModel
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public int TotalRegistrations { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public List<Registration> Participants { get; set; }
    }
}
