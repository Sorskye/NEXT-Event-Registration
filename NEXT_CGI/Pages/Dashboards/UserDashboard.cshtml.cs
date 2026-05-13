using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NERA.Models;

namespace NEXT.Pages.Dashboards
{
    public class UserDashboardModel : PageModel
    {
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;
        private int currentUserId = 1;

        public UserDashboardModel(IEventRepository eventRepository, IUserRepository userRepository)
        {
            this._eventRepository = eventRepository;
            this._userRepository = userRepository;
        }

        public bool IsAdmin { get; set; }

        public List<Event> RegisteredEvents { get; set; }
        public List<Event> UpcomingEvents { get; set; }

        public void OnGet()
        {
            RegisteredEvents = _eventRepository.GetRegisteredEventsByUser(currentUserId);
            UpcomingEvents = _eventRepository.GetUpcomingEventsByUser(currentUserId);

            IsAdmin = _userRepository.IsUserAdmin(currentUserId);
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
