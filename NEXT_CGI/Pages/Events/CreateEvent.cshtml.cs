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
        public string Title { get; set; } = string.Empty;

        [BindProperty]
        public string Description { get; set; } = string.Empty;

        [BindProperty]
        public DateOnly? Beginning_date { get; set; }

        [BindProperty]
        public DateOnly? Ending_date { get; set; }

        [BindProperty]
        public TimeOnly? Beginning_time { get; set; }

        [BindProperty]
        public TimeOnly? Ending_time { get; set; }

        [BindProperty]
        public string Location { get; set; } = string.Empty;

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

            Beginning_date = DateOnly.FromDateTime(DateTime.Now);
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

            // Valideer dat begindatum niet in het verleden ligt
            if (Beginning_date.HasValue)
            {
                var beginDateTime = Beginning_date.Value.ToDateTime(Beginning_time ?? TimeOnly.MinValue);
                if (beginDateTime < DateTime.Now)
                {
                    ErrorMessage = "De begindatum en -tijd mogen niet in het verleden liggen.";
                    return Page();
                }
            }

            // Valideer dat einddatum niet voor begindatum ligt
            if (Beginning_date.HasValue && Ending_date.HasValue)
            {
                var beginDateTime = Beginning_date.Value.ToDateTime(Beginning_time ?? TimeOnly.MinValue);
                var endDateTime = Ending_date.Value.ToDateTime(Ending_time ?? TimeOnly.MinValue);
                if (endDateTime < beginDateTime)
                {
                    ErrorMessage = "De einddatum en -tijd mogen niet voor de begindatum liggen.";
                    return Page();
                }
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
                Beginning_date = Beginning_date,
                Ending_date = Ending_date,
                Beginning_time = Beginning_time,
                Ending_time = Ending_time,
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
