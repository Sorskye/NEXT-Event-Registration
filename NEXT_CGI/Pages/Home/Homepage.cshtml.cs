using DAL.Models;
using DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NERA.Models;

namespace NEXT.Pages.Home
{
    public class HomepageModel : PageModel
    {
        private readonly EventRepository_SQLServer eventRepo;
        private readonly UserRepository_SQLServer userRepo;

        public HomepageModel(EventRepository_SQLServer eventRepo, UserRepository_SQLServer userRepo)
        {
            this.eventRepo = eventRepo;
            this.userRepo = userRepo;
        }

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

            RegisteredEvents = eventRepo.GetRegisteredEventsByUser(currentUserId.Value);
            UpcomingEvents = eventRepo.GetUpcomingEventsByUser(currentUserId.Value);

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

            eventRepo.RegisterUserForEvent(currentUserId.Value, id);

            return RedirectToPage();
        }

        public IActionResult OnPostUnregister(int id)
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");

            if (!currentUserId.HasValue)
            {
                return RedirectToPage("/Auth/Login");
            }

            eventRepo.UnregisterUserFromEvent(currentUserId.Value, id);

            return RedirectToPage();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Auth/Login");
        }
    }
}

