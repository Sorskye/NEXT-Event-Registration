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
        private readonly IUserRepository _userRepository;

        //const
        public AdminDashboardModel(IEventRepository eventRepository, IUserRepository userRepository)
        {
            this._eventRepository = eventRepository;
            this._userRepository = userRepository;
        }

        private readonly string connectionString;
        public int TotalEvents { get; set; }
        public string UserName { get; set; }

        private int currentUserId = 1;
        public bool IsAdmin { get; set; }

        public List<Event> UserMadeEvents { get; set; }

        public IActionResult OnGet()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return RedirectToPage("/Home/Homepage");
            }

            IsAdmin = true;

            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            UserName = HttpContext.Session.GetString("UserName");

            UserMadeEvents = _eventRepository.GetRegisteredEventsByUser(currentUserId.Value);

            TotalEvents = UserMadeEvents.Count;

            return Page();
        }

        public IActionResult OnPostRegister(int id)
        {
            _eventRepository.RegisterUserForEvent(currentUserId, id);

            return RedirectToPage();
        }

        public IActionResult OnPostUnregister(int id)
        {
            _eventRepository.UnregisterUserFromEvent(currentUserId, id);

            return RedirectToPage();
        }
    }
}
