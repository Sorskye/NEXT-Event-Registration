using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NERA.Models;

namespace NEXT.Pages.Dashboards
{
    public class UserDashboardModel : PageModel
    {
        private readonly IEventRegistrationRepository _eventRegistrationRepository;
        private readonly IUserRepository _userRepository;

        public UserDashboardModel(IEventRegistrationRepository eventRegistrationRepository, IUserRepository userRepository)
        {
            this._eventRegistrationRepository = eventRegistrationRepository;
            this._userRepository = userRepository;
        }

        public bool IsAdmin { get; set; }

        public List<Event> RegisteredEvents { get; set; } = new();
        public List<Event> UpcomingEvents { get; set; } = new();

        public IActionResult OnGet()
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            if (!currentUserId.HasValue)
            {
                return RedirectToPage("/Auth/Login");
            }

            RegisteredEvents = _eventRegistrationRepository.GetRegisteredEventsByUser(currentUserId.Value);
            UpcomingEvents = _eventRegistrationRepository.GetUpcomingEventsByUser(currentUserId.Value);

            IsAdmin = _userRepository.IsUserAdmin(currentUserId.Value);
            return Page();
        }

        public IActionResult OnPostRegister(int id)
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            if (!currentUserId.HasValue)
            {
                return RedirectToPage("/Auth/Login");
            }

            _eventRegistrationRepository.RegisterUserForEvent(currentUserId.Value, id);

            return RedirectToPage();
        }

        public IActionResult OnPostUnregister(int id)
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            if (!currentUserId.HasValue)
            {
                return RedirectToPage("/Auth/Login");
            }

            _eventRegistrationRepository.UnregisterUserFromEvent(currentUserId.Value, id);

            return RedirectToPage();
        }
    }
}
