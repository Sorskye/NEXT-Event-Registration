using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NERA.Models;

namespace NEXT.Pages.Events
{
    public class EventDetailsModel : PageModel
    {
        private readonly IEventRepository _eventRepository;

        public EventDetailsModel(IEventRepository eventRepository)
        {
            this._eventRepository = eventRepository;
        }

        public Event SelectedEvent { get; set; }
        public bool IsRegistered { get; set; }

        public IActionResult OnGet(int id)
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");

            if (!currentUserId.HasValue)
            {
                return RedirectToPage("/Auth/Login");
            }

            SelectedEvent = _eventRepository.GetEventById(id);

            IsRegistered = _eventRepository.IsUserRegistered(currentUserId.Value, id);

            return Page();
        }
    }
}
