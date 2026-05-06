using DAL.Models;
using DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NERA.Models;

namespace NEXT.Pages.Dashboards
{
    public class UserDashboardModel : PageModel
    {
        private readonly EventRepository_SQLServer eventRepo;
        private readonly UserRepository_SQLServer userRepo;
        private int currentUserId = 1;

        public UserDashboardModel(EventRepository_SQLServer eventRepo, UserRepository_SQLServer userRepo)
        {
            this.eventRepo = eventRepo;
            this.userRepo = userRepo;
        }

        public bool IsAdmin { get; set; }

        public List<Event> RegisteredEvents { get; set; }
        public List<Event> UpcomingEvents { get; set; }

        public void OnGet()
        {

            RegisteredEvents = eventRepo.GetRegisteredEventsByUser(currentUserId);
            UpcomingEvents = eventRepo.GetUpcomingEventsByUser(currentUserId);
            

            IsAdmin = userRepo.IsUserAdmin(currentUserId);
        }

        public IActionResult OnPostRegister(int id)
        {
            eventRepo.RegisterUserForEvent(currentUserId, id);

            return RedirectToPage();
        }

        public IActionResult OnPostUnregister(int id)
        {
            eventRepo.UnregisterUserFromEvent(currentUserId, id);

            return RedirectToPage();
        }
    }
}
