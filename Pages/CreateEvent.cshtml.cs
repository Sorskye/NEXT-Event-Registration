using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NERA.Models;

namespace NEXT.Pages
{
    public class CreateEventModel : PageModel
    {
        private readonly IWebHostEnvironment _environment;

        public CreateEventModel(IWebHostEnvironment environment)
        {
            _environment = environment;
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
        public IFormFile ImageFile { get; set; }

        public IActionResult OnGet()
        {
            Date = DateTime.Now;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string imagePath = "/images/default.jpg";

            if (ImageFile != null && ImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "events");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }

                imagePath = "/images/events/" + uniqueFileName;
            }

            EventRepository_SQLServer repo = new EventRepository_SQLServer();

            repo.CreateEvent(new Event
            {
                Title = Title,
                Description = Description,
                Date = Date,
                Location = Location,
                Organizer = "Admin",
                ImageUrl = imagePath,
                CurrentParticipants = 0,
                MaxParticipants = MaxParticipants
            });

            return RedirectToPage("/Homepage");
        }
    }
}