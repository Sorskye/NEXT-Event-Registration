using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NERA.Models;

namespace NEXT.Pages.Events
{
    public class EventQrModel : PageModel
    {
        private readonly IEventRepository _eventRepository;

        public EventQrModel(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public Event? EventItem { get; set; }
        public string QrTargetUrl { get; set; } = string.Empty;

        public IActionResult OnGet(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return RedirectToPage("/Home/Homepage");
            }

            EventItem = _eventRepository.GetEventById(id);

            if (EventItem == null)
            {
                return NotFound();
            }

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            //QrTargetUrl = $"{baseUrl}/Auth/LoginRedirect?eventId={EventItem.Id}";

            QrTargetUrl = Url.Page(
            "/Auth/LoginRedirect",
            pageHandler: null,
            values: new { eventId = EventItem.Id },
            protocol: Request.Scheme
            ) ?? "";

            return Page();
        }
    }
}
