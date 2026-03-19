namespace NERA.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public string Location { get; set; }
        public string Organizer { get; set; }
        public string ImageUrl { get; set; }
        public int CurrentParticipants { get; set; }
        public int MaxParticipants { get; set; }
        public bool IsRegistered { get; set; }
    }
}