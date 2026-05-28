using DAL.Repositories.Interfaces;
using NERA.Models;

namespace NEXT.Tests.Fakes
{
    public class FakeEventRegistrationRepository : IEventRegistrationRepository
    {
        public bool RegisterUserForEventCalled { get; private set; }
        public int RegisteredUserId { get; private set; }
        public int RegisteredEventId { get; private set; }

        public bool UnregisterUserFromEventCalled { get; private set; }
        public int UnregisteredUserId { get; private set; }
        public int UnregisteredEventId { get; private set; }

        public List<Event> RegisteredEventsToReturn { get; set; } = new();
        public List<Event> UpcomingEventsToReturn { get; set; } = new();
        public bool IsUserRegisteredToReturn { get; set; }
        public int ParticipantCountToReturn { get; set; }

        public void RegisterUserForEvent(int userId, int eventId)
        {
            RegisterUserForEventCalled = true;
            RegisteredUserId = userId;
            RegisteredEventId = eventId;
        }

        public void UnregisterUserFromEvent(int userId, int eventId)
        {
            UnregisterUserFromEventCalled = true;
            UnregisteredUserId = userId;
            UnregisteredEventId = eventId;
        }

        public List<Event> GetRegisteredEventsByUser(int userId)
        {
            return RegisteredEventsToReturn;
        }

        public List<Event> GetUpcomingEventsByUser(int userId)
        {
            return UpcomingEventsToReturn;
        }

        public bool IsUserRegistered(int userId, int eventId)
        {
            return IsUserRegisteredToReturn;
        }

        public int GetParticipantCountByEventId(int eventId)
        {
            return ParticipantCountToReturn;
        }
    }
}