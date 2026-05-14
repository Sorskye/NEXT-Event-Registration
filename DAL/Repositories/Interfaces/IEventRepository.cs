using NERA.Models;

namespace DAL.Repositories.Interfaces;

public interface IEventRepository
{
    Event GetEventById(int id);
    List<Event> GetEventsByUser(int userId);
    void CreateEvent(Event ev, int creatorUserId);
}
