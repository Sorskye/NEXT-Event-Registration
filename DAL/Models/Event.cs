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
        public DateTime? DateTime { get; set; }
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
            get => DateTime ?? System.DateTime.MinValue;
            set => DateTime = value;
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
    }

}
