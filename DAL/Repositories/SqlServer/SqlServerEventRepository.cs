using DAL.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using NERA.Models;

namespace DAL.Repositories.SqlServer
{
    public class SqlServerEventRepository : IEventRepository
    {
        private readonly string connectionString;
        private readonly ILocationRepository locationRepository;

        public SqlServerEventRepository(
            string connectionString,
            ILocationRepository locationRepository)
        {
            this.connectionString = connectionString;
            this.locationRepository = locationRepository;
        }

        public Event GetEventById(int id)
        {
            using SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            using SqlCommand cmd = new SqlCommand(SqlServerEventMapper.EventSelect + " WHERE e.ID = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);

            using SqlDataReader dr = cmd.ExecuteReader();

            if (!dr.Read())
            {
                return null;
            }

            return SqlServerEventMapper.ReadEvent(dr);
        }

        public List<Event> GetEventsByUser(int userId)
        {
            List<Event> events = new List<Event>();

            using SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            string sqlQuery = SqlServerEventMapper.EventSelect + @"
                INNER JOIN User_Event ue ON e.ID = ue.EventID
                WHERE ue.UserID = @UserID
                ORDER BY e.Beginning_date ASC, e.Beginning_time ASC";

            using SqlCommand cmd = new SqlCommand(sqlQuery, con);
            cmd.Parameters.AddWithValue("@UserID", userId);

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                events.Add(SqlServerEventMapper.ReadEvent(dr));
            }

            return events;
        }

        public void CreateEvent(Event ev, int creatorUserId)
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            int? locationId = ev.LocationId;

            if (locationId == null && !string.IsNullOrWhiteSpace(ev.LocationName))
            {
                locationId = locationRepository.GetOrCreateLocationId(ev.LocationName);
            }

            using SqlTransaction transaction = conn.BeginTransaction();

            try
            {
                string insertEventQuery = @"
                INSERT INTO Event
                (LocationID, Name, Description, Photo, Cost, Max_participants, Lottery_prize,
                 Beginning_date, Ending_date, Beginning_time, Ending_time)
                OUTPUT INSERTED.ID
                VALUES
                (@LocationID, @Name, @Description, @Photo, @Cost, @MaxParticipants, @LotteryPrize,
                 @BeginningDate, @EndingDate, @BeginningTime, @EndingTime)";

                using SqlCommand eventCommand = new SqlCommand(insertEventQuery, conn, transaction);

                eventCommand.Parameters.AddWithValue("@LocationID", locationId ?? (object)DBNull.Value);
                eventCommand.Parameters.AddWithValue("@Name", ev.Name ?? (object)DBNull.Value);
                eventCommand.Parameters.AddWithValue("@Description", ev.Description ?? (object)DBNull.Value);
                eventCommand.Parameters.AddWithValue("@Photo", ev.Photo ?? (object)DBNull.Value);
                eventCommand.Parameters.AddWithValue("@Cost", ev.Cost ?? (object)DBNull.Value);
                eventCommand.Parameters.AddWithValue("@MaxParticipants", ev.MaxParticipants ?? (object)DBNull.Value);
                eventCommand.Parameters.AddWithValue("@LotteryPrize", ev.LotteryPrize ?? (object)DBNull.Value);
                eventCommand.Parameters.AddWithValue("@BeginningDate", ev.Beginning_date?.ToDateTime(TimeOnly.MinValue) ?? (object)DBNull.Value);
                eventCommand.Parameters.AddWithValue("@EndingDate", ev.Ending_date?.ToDateTime(TimeOnly.MinValue) ?? (object)DBNull.Value);
                eventCommand.Parameters.AddWithValue("@BeginningTime", ev.Beginning_time?.ToTimeSpan() ?? (object)DBNull.Value);
                eventCommand.Parameters.AddWithValue("@EndingTime", ev.Ending_time?.ToTimeSpan() ?? (object)DBNull.Value);

                int eventId = Convert.ToInt32(eventCommand.ExecuteScalar());

                string linkUserQuery = @"
                INSERT INTO User_Event (UserID, EventID)
                VALUES (@UserID, @EventID)";

                using SqlCommand userEventCommand = new SqlCommand(linkUserQuery, conn, transaction);

                userEventCommand.Parameters.AddWithValue("@UserID", creatorUserId);
                userEventCommand.Parameters.AddWithValue("@EventID", eventId);

                userEventCommand.ExecuteNonQuery();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}