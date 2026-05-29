using NERA.Models;

namespace DAL.Repositories.Interfaces;

public interface IEventRepository // Verantwoordelijk voor [EVENTS] ophalen, events van organisator ophalen, event aanmaken.
{
    Event GetEventById(int id);
    List<Event> GetEventsByUser(int userId);
    void CreateEvent(Event ev, int creatorUserId);
    void DeleteEvent(int eventId);
}
