using DAL.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NERA.Models;


namespace NERA.Pages
{
    public class EventDetailsModel : PageModel
    {
        private int currentUserId = 1;
        public Event SelectedEvent { get; set; }
        public bool IsRegistered { get; set; }

        public void OnGet(int id)
        {
            EventRepository_SQLServer repo = new EventRepository_SQLServer();

            SelectedEvent = repo.GetEventById(id);

            IsRegistered = repo.IsUserRegistered(currentUserId, id);
        }
    }
}