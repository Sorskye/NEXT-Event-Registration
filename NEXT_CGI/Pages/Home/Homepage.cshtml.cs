using DAL.Models;
using DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NERA.Models;

namespace NEXT.Pages.Home
{
    public class HomepageModel : PageModel
    {
        public bool IsAdmin { get; set; }
        public string CurrentUserName { get; set; }
        public List<Event> RegisteredEvents { get; set; }
        public List<Event> UpcomingEvents { get; set; }
        

        public IActionResult OnGet()
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");

            if (!currentUserId.HasValue)
            {
                return RedirectToPage("/Auth/Login");
            }

            EventRepository_SQLServer repo = new EventRepository_SQLServer();
            UserRepository_SQLServer userRepo = new UserRepository_SQLServer();

            RegisteredEvents = repo.GetRegisteredEventsByUser(currentUserId.Value);
            UpcomingEvents = repo.GetUpcomingEventsByUser(currentUserId.Value);

            IsAdmin = userRepo.IsUserAdmin(currentUserId.Value);
            CurrentUserName = HttpContext.Session.GetString("UserName") ?? "Gebruiker";

            return Page();
        }

        public IActionResult OnPostRegister(int id)
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");

            if (!currentUserId.HasValue)
            {
                return RedirectToPage("/Auth/Login");
            }

            EventRepository_SQLServer repo = new EventRepository_SQLServer();
            repo.RegisterUserForEvent(currentUserId.Value, id);

            return RedirectToPage();
        }

        public IActionResult OnPostUnregister(int id)
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");

            if (!currentUserId.HasValue)
            {
                return RedirectToPage("/Auth/Login");
            }

            EventRepository_SQLServer repo = new EventRepository_SQLServer();
            repo.UnregisterUserFromEvent(currentUserId.Value, id);

            return RedirectToPage();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Auth/Login");
        }
    }
}

