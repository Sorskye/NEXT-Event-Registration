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
        [Required(ErrorMessage = "Begindatum is verplicht.")]
        public DateOnly? Beginning_date { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Einddatum is verplicht.")]
        public DateOnly? Ending_date { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Begintijd is verplicht.")]
        public TimeOnly? Beginning_time { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Eindtijd is verplicht.")]
        public TimeOnly? Ending_time { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Locatie is verplicht.")]
        [StringLength(100, ErrorMessage = "Locatie mag maximaal 100 tekens zijn.")]
        public string? Location { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Max deelnemers is verplicht.")]
        [Range(1, 200, ErrorMessage = "Max deelnemers moet minimaal 1 zijn.")]
        public int MaxParticipants { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Kosten is verplicht.")]
        [Range(0, 500, ErrorMessage = "Kosten mogen niet negatief zijn.")]
        public decimal? Cost { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Loterijprijs is verplicht.")]
        [Range(0, 500, ErrorMessage = "Loterijprijs mag niet negatief zijn.")]
        public decimal? LotteryPrize { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Afbeelding is verplicht.")]
        public IFormFile? ImageFile { get; set; }

        public string? ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            if (!IsCurrentUserAdmin())
            {
                return RedirectToPage("/Home/Homepage");
            }

            Beginning_date = DateOnly.FromDateTime(DateTime.Now);
            Beginning_time = TimeOnly.FromDateTime(DateTime.Now.AddHours(2));
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

            DateOnly minimumDate = DateOnly.FromDateTime(DateTime.Now);
            TimeOnly minimumTime = TimeOnly.FromDateTime(DateTime.Now.AddHours(2));

            if (Beginning_date.HasValue && Beginning_time.HasValue)
            {
                if (Beginning_date.Value < minimumDate ||
                   (Beginning_date.Value == minimumDate && Beginning_time.Value < minimumTime))
                {
                    ModelState.AddModelError(nameof(Beginning_time), "Begintijd moet minimaal 2 uur vanaf nu zijn.");
                }
            }

            if (Beginning_date.HasValue && Ending_date.HasValue)
            {
                if (Ending_date.Value < Beginning_date.Value)
                {
                    ModelState.AddModelError(nameof(Ending_date), "Einddatum mag niet voor de begindatum liggen.");
                }

                if (Ending_date.Value == Beginning_date.Value &&
                    Beginning_time.HasValue && Ending_time.HasValue)
                {
                    if (Ending_time.Value <= Beginning_time.Value)
                    {
                        ModelState.AddModelError(nameof(Ending_time), "Eindtijd moet later zijn dan de begintijd.");
                    }
                }
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
