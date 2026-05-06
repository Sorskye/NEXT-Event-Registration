using DAL.Models;
using DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NERA.Models;

namespace NEXT.Pages.Dashboards
{
    public class AdminDashboardModel : PageModel
    {
        private readonly EventRepository_SQLServer eventRepo;
        private readonly UserRepository_SQLServer userRepo;

        public AdminDashboardModel(EventRepository_SQLServer eventRepo, UserRepository_SQLServer userRepo)
        {
            this.eventRepo = eventRepo;
            this.userRepo = userRepo;
        }

        public List<Event> AllEvents { get; set; } = new();
        public int TotalEvents { get; set; }

        public IActionResult OnGet()
        {
            int currentUserId = 2; // tijdelijk admin

            if (!userRepo.IsUserAdmin(currentUserId))
            {
                return RedirectToPage("/Dashboards/UserDashboard");
            }

            AllEvents = eventRepo.GetAllEvents();
            TotalEvents = AllEvents.Count;

            return Page();
        }
    }
}
