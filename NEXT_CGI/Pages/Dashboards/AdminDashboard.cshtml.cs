using DAL.Models;
using DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NERA.Models;

namespace NEXT.Pages.Dashboards
{
    public class AdminDashboardModel : PageModel
    {
        public List<Event> AllEvents { get; set; } = new();
        public int TotalEvents { get; set; }

        public IActionResult OnGet()
        {
            int currentUserId = 2; // tijdelijk admin

            UserRepository_SQLServer userRepo = new UserRepository_SQLServer();

            if (!userRepo.IsUserAdmin(currentUserId))
            {
                return RedirectToPage("/Dashboards/UserDashboard");
            }

            EventRepository_SQLServer eventRepo = new EventRepository_SQLServer();

            AllEvents = eventRepo.GetAllEvents();
            TotalEvents = AllEvents.Count;

            return Page();
        }
    }
}
