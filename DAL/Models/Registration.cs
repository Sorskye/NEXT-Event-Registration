using System;

namespace DAL.Models
{
    public class Registration
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
