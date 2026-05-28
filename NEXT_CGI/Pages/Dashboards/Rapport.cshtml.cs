using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NERA.Models;


namespace NEXT.Pages.Dashboards
{
    public class RapportModel : PageModel
    {
        private readonly IEventRepository _eventrepository;
        private readonly IEventRegistrationRepository _eventregister;

        public RapportModel(IEventRegistrationRepository eventregister, IEventRepository eventrepository)
        {
            _eventrepository = eventrepository;
            _eventregister = eventregister;
        }

        [BindProperty(SupportsGet = true, Name = "id")]
        public int EventId { get; set; }


        [BindProperty(SupportsGet = true)]
        public int RegistrationId { get; set; }


        [BindProperty(SupportsGet = true)]
        public bool Attended { get; set; }

        public PresenceViewModel PresenceData { get; set; }

        public IActionResult OnGet()
        {
            // Event ophalen
            Event eventInfo = _eventrepository.GetEventById(EventId);

            if (eventInfo == null)
            {
                return NotFound();
            }


            // Registraties ophalen
            List<Registration> registrations =
                _eventregister.GetRegisteredByEventId(EventId);

            // Statistics berekenen
            int totalRegistrations = registrations.Count;

            int presentCount = registrations.Count(r => r.Attended);

            int absentCount = totalRegistrations - presentCount;

            // ViewModel vullen
            PresenceData = new PresenceViewModel
            {
                EventId = EventId,
                EventName = eventInfo.Name,

                TotalRegistrations = totalRegistrations,
                PresentCount = presentCount,
                AbsentCount = absentCount,

                Participants = registrations
            };
            return Page();
        }

        public IActionResult OnPost()
        {
            Console.WriteLine($"RegistrationId: {RegistrationId}, Attended: {Attended}");

            _eventregister.UpdateAttendance(RegistrationId, Attended);

            return RedirectToPage("/Dashboards/Rapport", new { id = EventId });
           
        }
    }
}
