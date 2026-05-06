using DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NERA.Models;


namespace NEXT.Pages.Events
{
    public class EventDetailsModel : PageModel
    {
        public Event SelectedEvent { get; set; }
        public bool IsRegistered { get; set; }

        public IActionResult OnGet(int id)
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");

            if (!currentUserId.HasValue)
            {
                return RedirectToPage("/Auth/Login");
            }

            EventRepository_SQLServer repo = new EventRepository_SQLServer();

            SelectedEvent = repo.GetEventById(id);

            IsRegistered = repo.IsUserRegistered(currentUserId.Value, id);

            return Page();
        }
    }
}

