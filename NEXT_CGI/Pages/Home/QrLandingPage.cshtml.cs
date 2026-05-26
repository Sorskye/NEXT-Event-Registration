using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NERA.Models;
using System.Security.Claims;

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

        //public IActionResult OnGet(int eventId)
        //{
        //    EventItem = _eventRepository.GetEventById(eventId);

        //    if (EventItem == null)
        //    {
        //        Message = "Event niet gevonden.";
        //        IsSuccess = false;
        //        return Page();
        //    }

        //    string? externalId =
        //        User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
        //        User.FindFirst("sub")?.Value;

        //    if (string.IsNullOrWhiteSpace(externalId))
        //    {
        //        Message = "Ingelogde gebruiker kon niet worden bepaald.";
        //        IsSuccess = false;
        //        return Page();
        //    }

            //var user = _userRepository.GetUserByExternalId(externalId);
            //if (user == null)
            //{
            //    Message = "Er is geen gekoppelde gebruiker gevonden in het systeem.";
            //    IsSuccess = false;
            //    return Page();
            //}

        //    bool isRegistered = _eventRegistrationRepository.IsUserRegistered(user.Id, eventId);

        //    if (isRegistered)
        //    {
        //        IsSuccess = true;
        //        Message = "Bevestiging: je was aangemeld voor dit event.";
        //    }
        //    else
        //    {
        //        IsSuccess = false;
        //        Message = "Je was niet aangemeld voor dit event.";
        //    }

        //    return Page();
        //}
    }
}