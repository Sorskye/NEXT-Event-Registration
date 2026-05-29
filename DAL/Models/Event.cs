using System;

namespace NERA.Models
{
    public class Event
    {
        public int Id { get; set; }
        public int? LocationId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public byte[]? Photo { get; set; }
        public DateOnly? Beginning_date { get; set; }
        
        public DateOnly? Ending_date { get; set; }
        
        public TimeOnly? Beginning_time { get; set; }
        
        public TimeOnly? Ending_time { get; set; }
        public decimal? Cost { get; set; }
        public int? MaxParticipants { get; set; }
        public decimal? LotteryPrize { get; set; }
        public string? LocationName { get; set; }
        public int CurrentParticipants { get; set; }
        public string? Title
        {
            get => Name;
            set => Name = value;
        }
        public DateOnly Date
        {
            get => Beginning_date ?? System.DateOnly.MinValue;
            set => Beginning_date = value;
        }
        public string? Location
        {
            get => LocationName;
            set => LocationName = value;
        }
        public string? ImageUrl { get; set; }
        public string? Organizer { get; set; }
        public decimal? Costs
        {
            get => Cost;
            set => Cost = value;
        }

        public bool IsRegistered { get; set; }
        public int? OrganizerUserId { get; set; }
    }

}
