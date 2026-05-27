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
        public DateTime? DateTime_beginning { get; set; }
        
        public DateTime? DateTime_ending { get; set; }
        
        public TimeSpan? BeginningTime { get; set; }
        
        public TimeSpan? EndingTime { get; set; }
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
        public DateTime Date
        {
            get => DateTime_beginning ?? System.DateTime.MinValue;
            set => DateTime_beginning = value;
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
    }

}
