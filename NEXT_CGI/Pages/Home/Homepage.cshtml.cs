using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NERA.Models;

namespace NEXT.Pages.Home
{
    public class HomepageModel : PageModel
    {
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;

        public HomepageModel(IEventRepository eventRepository, IUserRepository userRepository)
        {
            this._eventRepository = eventRepository;
            this._userRepository = userRepository;
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

            RegisteredEvents = _eventRepository.GetRegisteredEventsByUser(currentUserId.Value);
            UpcomingEvents = _eventRepository.GetUpcomingEventsByUser(currentUserId.Value);

            IsAdmin = _userRepository.IsUserAdmin(currentUserId.Value);
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

            _eventRepository.RegisterUserForEvent(currentUserId.Value, id);

            return RedirectToPage();
        }

        public IActionResult OnPostUnregister(int id)
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");

            if (!currentUserId.HasValue)
            {
                return RedirectToPage("/Auth/Login");
            }

            _eventRepository.UnregisterUserFromEvent(currentUserId.Value, id);

            return RedirectToPage();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Auth/Login");
        }
    }
}
