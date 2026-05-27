using DAL.Repositories.Interfaces;
using System.ComponentModel.DataAnnotations;
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
        [Required(ErrorMessage = "Titel is verplicht.")]
        [StringLength(100, ErrorMessage = "Titel mag maximaal 100 tekens zijn.")]
        public string? Title { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Beschrijving is verplicht.")]
        [StringLength(1000, ErrorMessage = "Beschrijving mag maximaal 1000 tekens zijn.")]
        public string? Description { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Datum is verplicht.")]
        public DateTime? Date { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Locatie is verplicht.")]
        [StringLength(100, ErrorMessage = "Locatie mag maximaal 100 tekens zijn.")]
        public string? Location { get; set; }

        [BindProperty]
        [Range(1, 200, ErrorMessage = "Max deelnemers moet minimaal 1 zijn.")]
        public int MaxParticipants { get; set; }

        [BindProperty]
        [Range(0, 500, ErrorMessage = "Kosten mogen niet negatief zijn.")]
        public decimal? Cost { get; set; }

        [BindProperty]
        [Range(0, 500, ErrorMessage = "Loterijprijs mag niet negatief zijn.")]
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

            Date = DateTime.Now.AddHours(2);
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

            DateTime minimumEventDate = DateTime.Now.AddHours(2);

            if (Date.HasValue && Date.Value < minimumEventDate)
            {
                ModelState.AddModelError(nameof(Date), "Datum moet minimaal 2 uur vanaf nu zijn.");
            }

            byte[]? photoBytes = null;
            if (ImageFile != null && ImageFile.Length > 0)
            {
                if (ImageFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError(nameof(ImageFile), "De afbeelding mag maximaal 5 MB zijn.");
                    return Page();
                }

                string[] allowedContentTypes = ["image/jpeg", "image/png", "image/webp"];
                if (!allowedContentTypes.Contains(ImageFile.ContentType))
                {
                    ModelState.AddModelError(nameof(ImageFile), "Alleen JPG, PNG of WEBP bestanden zijn toegestaan.");
                    return Page();
                }
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!Date.HasValue)
            {
                return Page();
            }

            DateTime eventDate = Date.Value;

            if (ImageFile != null && ImageFile.Length > 0)
            {
                using MemoryStream memoryStream = new MemoryStream();
                ImageFile.CopyTo(memoryStream);
                photoBytes = memoryStream.ToArray();
            }
            _eventRepository.CreateEvent(new Event
            {
                Name = Title,
                Description = Description,
                DateTime_beginning = eventDate,
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
