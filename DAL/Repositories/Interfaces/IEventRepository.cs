using NERA.Models;

namespace DAL.Repositories.Interfaces;

public interface IEventRepository
{
    Event GetEventById(int id);
    void RegisterUserForEvent(int userId, int eventId);
    void UnregisterUserFromEvent(int userId, int eventId);
    List<Event> GetRegisteredEventsByUser(int userId);
    List<Event> GetUpcomingEventsByUser(int userId);
    bool IsUserRegistered(int userId, int eventId);
    void CreateEvent(Event ev);
}
