using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NERA.Models;

namespace NEXT.Pages.Home
{
    public class HomepageModel : PageModel
    {
        private readonly IEventRegistrationRepository _eventRegistrationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEventRepository _eventRepository;

        public HomepageModel(IEventRegistrationRepository eventRegistrationRepository, IUserRepository userRepository, IEventRepository eventRepository)
        {
            this._eventRegistrationRepository = eventRegistrationRepository;
            this._userRepository = userRepository;
            this._eventRepository = eventRepository;
        }

        public bool IsAdmin { get; set; }
        public string CurrentUserName { get; set; }
        public List<Event> RegisteredEvents { get; set; }
        public List<Event> UpcomingEvents { get; set; }

        public IActionResult OnGet()
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            
            if (currentUserId == null)
            {
                return RedirectToPage("/Auth/Welcome");
            }

            RegisteredEvents = _eventRegistrationRepository.GetRegisteredEventsByUser(currentUserId.Value);

            // Filter out ended events from RegisteredEvents
            RegisteredEvents = RegisteredEvents
                .Where(ev =>
                {
                    if (ev.Ending_date == null || ev.Ending_time == null)
                        return true;

                    var ending = ev.Ending_date.Value.ToDateTime(ev.Ending_time.Value);
                    return ending > DateTime.Now;
                })
                .ToList();

            UpcomingEvents = _eventRegistrationRepository.GetUpcomingEventsByUser(currentUserId.Value);

            // Filter out ended events from UpcomingEvents
            UpcomingEvents = UpcomingEvents
                .Where(ev =>
                {
                    if (ev.Ending_date == null || ev.Ending_time == null)
                        return true;

                    var ending = ev.Ending_date.Value.ToDateTime(ev.Ending_time.Value);
                    return ending > DateTime.Now;
                })
                .ToList();

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

            var ev = _eventRepository.GetEventById(id);

            if (ev != null && ev.Beginning_date.HasValue && ev.Beginning_time.HasValue)
            {
                var beginDateTime = ev.Beginning_date.Value.ToDateTime(ev.Beginning_time.Value);
                if (beginDateTime < DateTime.Now)
                {
                    return RedirectToPage();
                }
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

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            var domain = "dev-xaeav3wbqayvz1av.us.auth0.com";
            var clientId = "eB6NyuBwvR3XUB5uSwQgmhfytfg7asUE";
            var returnTo = $"{Request.Scheme}://{Request.Host}/Welcome";
            
            // deauthenticate
            var logoutUrl =
                $"https://{domain}/v2/logout" +
                $"?client_id={clientId}" +
                $"&returnTo={Uri.EscapeDataString(returnTo)}";

            return Redirect(logoutUrl);
        }
    }
}
