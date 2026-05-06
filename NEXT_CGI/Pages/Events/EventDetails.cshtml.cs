using DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NERA.Models;


namespace NEXT.Pages.Events
{
    public class EventDetailsModel : PageModel
    {
        private readonly EventRepository_SQLServer eventRepo;

        public EventDetailsModel(EventRepository_SQLServer eventRepo)
        {
            this.eventRepo = eventRepo;
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

            SelectedEvent = eventRepo.GetEventById(id);

            IsRegistered = eventRepo.IsUserRegistered(currentUserId.Value, id);

            return Page();
        }
    }
}

