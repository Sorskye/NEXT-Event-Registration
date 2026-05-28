using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NERA.Models;
using System.Security.Claims;
using static DAL.Repositories.SqlServer.SqlServerEventRegistrationRepository;

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
                return RedirectToPage("/LoginRedirect", new { eventId = eventId.Value });
            }

            var result = _eventRegistrationRepository.TryMarkAttendance(userId.Value, eventId.Value, true);

            switch (result)
            {
                case AttendanceUpdateResult.Success:
                    Message = "Je aanwezigheid is bevestigd.";
                    IsSuccess = true;
                    break;

                case AttendanceUpdateResult.NotRegistered:
                    Message = "Je bent niet aangemeld voor dit event.";
                    IsSuccess = false;
                    break;

                case AttendanceUpdateResult.AlreadyAttended:
                    Message = "Je aanwezigheid was al eerder geregistreerd.";
                    IsSuccess = false;
                    break;
            }

            return Page();
        }
    }
}
