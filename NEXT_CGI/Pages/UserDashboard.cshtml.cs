using DAL.Models;
using DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NERA.Models;

namespace NEXT.Pages
{
    public class UserDashboardModel : PageModel
    {
        private int currentUserId = 1;
        public bool IsAdmin { get; set; }

        public List<Event> RegisteredEvents { get; set; }
        public List<Event> UpcomingEvents { get; set; }

        public void OnGet()
        {

            EventRepository_SQLServer repo = new EventRepository_SQLServer();
            UserRepository_SQLServer userRepo = new UserRepository_SQLServer();

            RegisteredEvents = repo.GetRegisteredEventsByUser(currentUserId);
            UpcomingEvents = repo.GetUpcomingEventsByUser(currentUserId);
            

            IsAdmin = userRepo.IsUserAdmin(currentUserId);
        }

        public IActionResult OnPostRegister(int id)
        {
            EventRepository_SQLServer repo = new EventRepository_SQLServer();
            repo.RegisterUserForEvent(currentUserId, id);

            return RedirectToPage();
        }

        public IActionResult OnPostUnregister(int id)
        {
            EventRepository_SQLServer repo = new EventRepository_SQLServer();
            repo.UnregisterUserFromEvent(currentUserId, id);

            return RedirectToPage();
        }
    }
}