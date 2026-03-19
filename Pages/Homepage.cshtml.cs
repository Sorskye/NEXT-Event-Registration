using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NERA.Models;

namespace NEXT.Pages
{
    public class HomepageModel : PageModel
    {
        public static List<Event> AllEvents = new List<Event>
{
    new Event
    {
        Id = 1,
        Title = "Team Innovation Meetup",
        Description = "Een interactieve sessie waarin professionals samenkomen om ideeën te delen, inzichten te krijgen en nieuwe oplossingen te ontwikkelen binnen een inspirerende werkomgeving.",
        Date = "27/03/2026",
        Location = "Eindhoven",
        Organizer = "User User",
        ImageUrl = "/images/event1.jpg",
        CurrentParticipants = 20,
        MaxParticipants = 40,
        IsRegistered = false
    },
    new Event
    {
        Id = 2,
        Title = "Digital Transformation Talk",
        Description = "Tijdens deze workshop krijg je waardevolle inzichten in actuele technologie en strategie, gepresenteerd door ervaren professionals uit het vakgebied.",
        Date = "02/04/2026",
        Location = "Utrecht",
        Organizer = "User User",
        ImageUrl = "/images/event2.jpg",
        CurrentParticipants = 12,
        MaxParticipants = 30,
        IsRegistered = false
    },
    new Event
        {
        Id = 3,
        Title = "Active Networking Event",
        Description = "Een energiek team building evenement waarbij samenwerking, communicatie en plezier centraal staan, ideaal om collega’s beter te leren kennen.",
        Date = "12/04/2026",
        Location = "Eindhoven , straat naam",
        Organizer = "User User",
        ImageUrl = "/images/event3.jpeg",
        CurrentParticipants = 18,
        MaxParticipants = 25,
        IsRegistered = false
                },
    new Event
                {
        Id = 4,
        Title = "NERA Outdoor Experience",
        Description = "Een informele en gezellige outdoor activiteit waar ontspanning en teamgevoel samenkomen in een unieke en actieve setting.",
        Date = "27/04/2026",
        Location = "Eindhoven , straat naam",
        Organizer = "User User",
        ImageUrl = "/images/event4.jpg",
        CurrentParticipants = 8,
        MaxParticipants = 20,
        IsRegistered = false
                }
};
        public List<Event> RegisteredEvents { get; set; }
        public List<Event> UpcomingEvents { get; set; }

        public void OnGet()
        {
            RegisteredEvents = AllEvents.Where(e => e.IsRegistered).ToList();
            UpcomingEvents = AllEvents.Where(e => !e.IsRegistered).ToList();
        }

        public IActionResult OnPostRegister(int id)
        {
            var ev = AllEvents.FirstOrDefault(e => e.Id == id);

            if (ev != null)
            {
                ev.IsRegistered = true;
            }

            return RedirectToPage();
        }
        public IActionResult OnPostUnregister(int id)
        {
            var ev = AllEvents.FirstOrDefault(e => e.Id == id);

            if (ev != null)
            {
                ev.IsRegistered = false;
            }

            return RedirectToPage();
        }
    }
}