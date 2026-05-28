using System;
using System.Net.Mail;

namespace DAL.Models
{
    public class Registration
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool Attended { get; set; }
    }
}
