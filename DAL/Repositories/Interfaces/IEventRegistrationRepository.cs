using NERA.Models;

namespace DAL.Repositories.Interfaces;

public interface IEventRegistrationRepository // Verantwoordelijk voor [REGISTRATIE] aanmelden, afmelden, checken of iemand is aangemeld, en lijsten ophalen rond registraties.
{
    void RegisterUserForEvent(int userId, int eventId);
    void UnregisterUserFromEvent(int userId, int eventId);
    List<Event> GetRegisteredEventsByUser(int userId);
    List<Event> GetUpcomingEventsByUser(int userId);
    bool IsUserRegistered(int userId, int eventId);
    int GetParticipantCountByEventId(int eventId);
    public AttendanceUpdateResult TryMarkAttendance(int userId, int eventId, string attended);

}
