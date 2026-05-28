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
        public string UserName { get; set; }
        public bool IsAdmin { get; set; }
        public List<Event> UserMadeEvents { get; set; }
        public List<Event> RegisteredEvents { get; set; }
        public List<Event> UpcomingEvents { get; set; }
        public List<Event> EndedEvents { get; set; }
        public DateOnly? Beginning_date { get; set; }

        public DateOnly? Ending_date { get; set; }

        public TimeOnly? Beginning_time { get; set; }

        public TimeOnly? Ending_time { get; set; }




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

            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            if (!currentUserId.HasValue)
            {
                return RedirectToPage("/Auth/Login");
            }

            UserName = HttpContext.Session.GetString("UserName");

            UserMadeEvents = _eventRepository.GetEventsByUser(currentUserId.Value);

            UpcomingEvents = new List<Event>();
            EndedEvents = new List<Event>();

            foreach (var ev in UserMadeEvents)
            {
                ev.IsRegistered = _eventRegistrationRepository
                    .IsUserRegistered(currentUserId.Value, ev.Id);

                if (ev.Ending_date == null || ev.Ending_time == null)
                {
                    UpcomingEvents.Add(ev);
                    continue;
                }

                var ending = ev.Ending_date.Value.ToDateTime(ev.Ending_time.Value);

                if (ending > DateTime.Now)
                    UpcomingEvents.Add(ev);
                else
                    EndedEvents.Add(ev);
            }

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
