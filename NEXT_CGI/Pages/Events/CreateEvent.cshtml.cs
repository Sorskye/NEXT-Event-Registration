using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NERA.Models;

namespace NEXT.Pages.Events
{
    public class CreateEventModel : PageModel
    {
        private readonly IEventRepository _eventRepository;

        public CreateEventModel(IEventRepository eventRepository)
        {
            this._eventRepository = eventRepository;
        }

        [BindProperty]
        public string Title { get; set; }

        [BindProperty]
        public string Description { get; set; }

        [BindProperty]
        public DateTime Date { get; set; }

        [BindProperty]
        public string Location { get; set; }

        [BindProperty]
        public int MaxParticipants { get; set; }

        [BindProperty]
        public decimal? Cost { get; set; }

        [BindProperty]
        public decimal? LotteryPrize { get; set; }

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public string? ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            if (!IsCurrentUserAdmin())
            {
                return RedirectToPage("/Home/Homepage");
            }

            Date = DateTime.Now;
            return Page();
        }

        public IActionResult OnPost()
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            if (!currentUserId.HasValue)
            {
                return RedirectToPage("/Auth/Login");
            }

            if (!IsCurrentUserAdmin())
            {
                return RedirectToPage("/Home/Homepage");
            }

            byte[]? photoBytes = null;
            if (ImageFile != null && ImageFile.Length > 0)
            {
                if (ImageFile.Length > 5 * 1024 * 1024)
                {
                    ErrorMessage = "De afbeelding mag maximaal 5 MB zijn.";
                    return Page();
                }

                using MemoryStream memoryStream = new MemoryStream();
                ImageFile.CopyTo(memoryStream);
                photoBytes = memoryStream.ToArray();
            }

            _eventRepository.CreateEvent(new Event
            {
                Name = Title,
                Description = Description,
                DateTime = Date,
                LocationName = Location,
                Photo = photoBytes,
                Cost = Cost,
                LotteryPrize = LotteryPrize,
                MaxParticipants = MaxParticipants
            }, currentUserId.Value);

            return RedirectToPage("/Home/Homepage");
        }

        private bool IsCurrentUserAdmin()
        {
            return string.Equals(HttpContext.Session.GetString("UserRole"), "Admin", StringComparison.OrdinalIgnoreCase);
        }
    }
}
