using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NERA.Models;
using NEXT.Pages;

namespace NERA.Pages
{
    public class EventDetailsModel : PageModel
    {
        public Event SelectedEvent { get; set; }

        public IActionResult OnGet(int id)
        {
            SelectedEvent = HomepageModel.AllEvents.FirstOrDefault(e => e.Id == id);

            if (SelectedEvent == null)
            {
                return RedirectToPage("/Homepage");
            }

            return Page();
        }
    }
}