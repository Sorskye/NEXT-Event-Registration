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
            // Get the current user id from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId) || eventId == null)
            {
                Message = "Invalid request.";
                IsSuccess = false;
                return Page();
            }       

            var result = _eventRegistrationRepository.TryMarkAttendance(userId, eventId.Value, "Present");

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
