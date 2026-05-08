using DAL.Models;
using DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NERA.Models;

namespace NEXT.Pages.Dashboards
{
    public class AdminDashboardModel : PageModel
    {
        
        private readonly string connectionString;
        public List<Event> AllEvents { get; set; } = new();
        public int TotalEvents { get; set; }
        public string UserName { get; set; }

        private int currentUserId = 1;
        public bool IsAdmin { get; set; }

        public List<Event> RegisteredEvents { get; set; }
        public List<Event> UpcomingEvents { get; set; }
        
        //const
        public AdminDashboardModel(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IActionResult OnGet()
        {

            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return RedirectToPage("/UserDashboard");
            }

            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            UserName = HttpContext.Session.GetString("UserName");

            EventRepository_SQLServer eventRepo = new EventRepository_SQLServer(this.connectionString);
            UserRepository_SQLServer userRepo = new UserRepository_SQLServer(this.connectionString);

            RegisteredEvents = eventRepo.GetRegisteredEventsByUser(currentUserId.Value);
            UpcomingEvents = eventRepo.GetUpcomingEventsByUser(currentUserId.Value);

            AllEvents = eventRepo.GetAllEvents();
            TotalEvents = AllEvents.Count;

            return Page();
        }


        public IActionResult OnPostRegister(int id)
        {
            EventRepository_SQLServer repo = new EventRepository_SQLServer(this.connectionString);
            repo.RegisterUserForEvent(currentUserId, id);

            return RedirectToPage();
        }

        public IActionResult OnPostUnregister(int id)
        {
            EventRepository_SQLServer repo = new EventRepository_SQLServer(this.connectionString);
            repo.UnregisterUserFromEvent(currentUserId, id);

            return RedirectToPage();
        }
    }
}