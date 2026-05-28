using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NERA.Models;

namespace NEXT.Pages.Dashboards
{
    public class AdminDashboardModel : PageModel
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEventRegistrationRepository _eventRegistrationRepository;
        private readonly IUserRepository _userRepository;
        
        public int TotalEvents { get; set; }
        public string UserName { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public List<Event> UserMadeEvents { get; set; } = new();
        public List<Event> RegisteredEvents { get; set; } = new();
        public List<Event> UpcomingEvents { get; set; } = new();
        public List<Event> EndedEvents => UserMadeEvents.Where(e => e.DateTime < System.DateTime.Now).ToList();

        
        public AdminDashboardModel(
            IEventRepository eventRepository,
            IEventRegistrationRepository eventRegistrationRepository,
            IUserRepository userRepository)
        {
            this._eventRepository = eventRepository;
            this._eventRegistrationRepository = eventRegistrationRepository;
            this._userRepository = userRepository;
        }


        public IActionResult OnGet()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return RedirectToPage("/Home/Homepage");
            }

            IsAdmin = true;

            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            if (!currentUserId.HasValue)
            {
                return RedirectToPage("/Auth/Login");
            }

            UserName = HttpContext.Session.GetString("UserName");

            UserMadeEvents = _eventRepository.GetEventsByUser(currentUserId.Value);
            RegisteredEvents = _eventRegistrationRepository.GetRegisteredEventsByUser(currentUserId.Value);
            UpcomingEvents = _eventRegistrationRepository.GetUpcomingEventsByUser(currentUserId.Value);

            IsAdmin = _userRepository.IsUserAdmin(currentUserId.Value);

            TotalEvents = UserMadeEvents.Count;

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
