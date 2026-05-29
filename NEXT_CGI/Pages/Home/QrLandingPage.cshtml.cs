using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NERA.Models;

namespace NEXT.Pages.Home
{
    public class QrLandingPageModel : PageModel
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEventRegistrationRepository _eventRegistrationRepository;
        private readonly IUserRepository _userRepository;

        public Event? EventItem { get; set; }
        public string Message { get; set; } = "";
        public bool IsSuccess { get; set; }

        public QrLandingPageModel(
            IEventRepository eventRepository,
            IEventRegistrationRepository eventRegistrationRepository,
            IUserRepository userRepository)
        {
            _eventRepository = eventRepository;
            _eventRegistrationRepository = eventRegistrationRepository;
            _userRepository = userRepository;
        }

        public IActionResult OnGet(int? eventId)
        {
            if (eventId == null)
            {
                Message = "Geen event geselecteerd.";
                IsSuccess = false;
                return Page();
            }

            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Auth/LoginRedirect", new { eventId = eventId.Value });
            }

            EventItem = _eventRepository.GetEventById(eventId.Value);

            var registrations = _eventRegistrationRepository.GetRegisteredByEventId(eventId.Value);
            var userRegistration = registrations.FirstOrDefault(r => r.UserId == userId.Value);

            if (userRegistration == null)
            {
                Message = "Je bent niet aangemeld voor dit event.";
                IsSuccess = false;
            }
            else if (userRegistration.Attended)
            {
                Message = "Je aanwezigheid was al eerder geregistreerd.";
                IsSuccess = false;
            }
            else
            {
                _eventRegistrationRepository.UpdateAttendance(userRegistration.Id, true);
                Message = "Je aanwezigheid is bevestigd.";
                IsSuccess = true;
            }

            return Page();
        }
    }
}
