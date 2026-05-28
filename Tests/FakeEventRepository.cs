using DAL.Repositories.Interfaces;
using NERA.Models;

namespace NEXT.Tests.Fakes
{
    public class FakeEventRepository : IEventRepository
    {
        public bool CreateEventCalled { get; private set; }
        public Event? CreatedEvent { get; private set; }
        public int CreatedByUserId { get; private set; }

        public void CreateEvent(Event ev, int userId)
        {
            CreateEventCalled = true;
            CreatedEvent = ev;
            CreatedByUserId = userId;
        }

        public Event GetEventById(int id)
        {
            throw new NotImplementedException();
        }
        public List<Event> GetEventsByUser(int userId)
        {
            throw new NotImplementedException();
        }
    } 
}