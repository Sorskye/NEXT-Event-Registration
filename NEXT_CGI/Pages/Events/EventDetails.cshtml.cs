using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NERA.Models;

namespace NEXT.Pages.Events
{
    public class EventDetailsModel : PageModel
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEventRegistrationRepository _eventRegistrationRepository;
        public Event? SelectedEvent { get; set; }
        public bool IsRegistered { get; set; }
        public bool IsCreator { get; set; }

        public EventDetailsModel(
            IEventRepository eventRepository,
            IEventRegistrationRepository eventRegistrationRepository)
        {
            this._eventRepository = eventRepository;
            this._eventRegistrationRepository = eventRegistrationRepository;
        }

        public IActionResult OnGet(int id)
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");

            if (!currentUserId.HasValue)
            {
                return RedirectToPage("/Auth/Login");
            }

            SelectedEvent = _eventRepository.GetEventById(id);

            IsRegistered = _eventRegistrationRepository.IsUserRegistered(currentUserId.Value, id);
            IsCreator = SelectedEvent?.OrganizerUserId == currentUserId.Value;

            return Page();
        }

        public IActionResult OnPostDelete(int id)
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");

            if (!currentUserId.HasValue)
                return RedirectToPage("/Auth/Login");

            Event? ev = _eventRepository.GetEventById(id);

            if (ev == null || ev.OrganizerUserId != currentUserId.Value)
                return Forbid();

            _eventRepository.DeleteEvent(id);

            return RedirectToPage("/Home/Homepage");
        }
    }
}
