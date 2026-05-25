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
                ORDER BY e.Date_time ASC";

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

            if (!locationId.HasValue && !string.IsNullOrWhiteSpace(ev.LocationName))
            {
                locationId = locationRepository.GetOrCreateLocationId(ev.LocationName);
            }

            using SqlTransaction transaction = conn.BeginTransaction();

            try
            {
                string query = @"
                INSERT INTO Event
                (LocationID, Name, Description, Photo, Date_time, Cost, Max_participants, Lottery_prize)
                OUTPUT INSERTED.ID
                VALUES
                (@LocationID, @Name, @Description, @Photo, @DateTime, @Cost, @MaxParticipants, @LotteryPrize)";

                using SqlCommand cmd = new SqlCommand(query, conn, transaction);
                cmd.Parameters.AddWithValue("@LocationID", (object?)locationId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Name", (object?)ev.Name ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", (object?)ev.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Photo", (object?)ev.Photo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DateTime", (object?)ev.DateTime ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Cost", (object?)ev.Cost ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MaxParticipants", (object?)ev.MaxParticipants ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@LotteryPrize", (object?)ev.LotteryPrize ?? DBNull.Value);

                int eventId = (int)cmd.ExecuteScalar();

                using SqlCommand userEventCmd = new SqlCommand(
                    "INSERT INTO User_Event (UserID, EventID) VALUES (@UserID, @EventID)",
                    conn,
                    transaction);
                userEventCmd.Parameters.AddWithValue("@UserID", creatorUserId);
                userEventCmd.Parameters.AddWithValue("@EventID", eventId);
                userEventCmd.ExecuteNonQuery();

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
