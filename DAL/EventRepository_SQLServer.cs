using Microsoft.Data.SqlClient;
using NERA.Models;

namespace DAL.Models
{
    public class EventRepository_SQLServer
    {

        private readonly string connectionString = "Data Source=mssqlstud.fhict.local;Persist Security Info=True;User ID=dbi563611;Password=FontysHosting123456@;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name=\"SQL Server Management Studio\";Command Timeout=0";

        public Event GetEventById(int id)
        {
            using SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            string sqlQuery = "SELECT * FROM Event WHERE Id = @Id";

            using SqlCommand cmd = new SqlCommand(sqlQuery, con);
            cmd.Parameters.AddWithValue("@Id", id);

            using SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                Event ev = new Event
                {
                    Id = Convert.ToInt32(dr["Id"]),
                    Title = dr["Title"].ToString(),
                    Description = dr["Description"].ToString(),
                    Date = Convert.ToDateTime(dr["Date"]),
                    Location = dr["Location"].ToString(),
                    Organizer = dr["Organizer"].ToString(),
                    ImageUrl = dr["ImageUrl"].ToString(),
                    MaxParticipants = Convert.ToInt32(dr["MaxParticipants"]),
                    Costs = Convert.ToDecimal(dr["Costs"])

                };

                dr.Close();

                ev.CurrentParticipants = GetParticipantCountByEventId(id);

                return ev;
            }

            return null;
        }
        public void RegisterUserForEvent(int userId, int eventId)
        {
            using SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            string sqlQuery = @"
                INSERT INTO Registration (UserId, EventId, RegistrationDate)
                VALUES (@UserId, @EventId, GETDATE())";

            using SqlCommand cmd = new SqlCommand(sqlQuery, con);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@EventId", eventId);

            cmd.ExecuteNonQuery();
        }
        public void UnregisterUserFromEvent(int userId, int eventId)
        {
            using SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            string sqlQuery = @"
                DELETE FROM Registration
                WHERE UserId = @UserId AND EventId = @EventId";

            using SqlCommand cmd = new SqlCommand(sqlQuery, con);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@EventId", eventId);

            cmd.ExecuteNonQuery();
        }
        public List<Event> GetRegisteredEventsByUser(int userId)
        {
            List<Event> events = new List<Event>();

            using SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            string sqlQuery = @"
            SELECT e.*
            FROM Event e
            INNER JOIN Registration r ON e.Id = r.EventId
            WHERE r.UserId = @UserId";

            using SqlCommand cmd = new SqlCommand(sqlQuery, con);
            cmd.Parameters.AddWithValue("@UserId", userId);

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                Event ev = new Event
                {
                    Id = Convert.ToInt32(dr["Id"]),
                    Title = dr["Title"].ToString(),
                    Description = dr["Description"].ToString(),
                    Date = Convert.ToDateTime(dr["Date"]),
                    Location = dr["Location"].ToString(),
                    Organizer = dr["Organizer"].ToString(),
                    ImageUrl = dr["ImageUrl"].ToString(),
                    CurrentParticipants = Convert.ToInt32(dr["CurrentParticipants"]),
                    MaxParticipants = Convert.ToInt32(dr["MaxParticipants"])
                };

                events.Add(ev);
            }

            return events;
        }
        public List<Event> GetUpcomingEventsByUser(int userId)
        {
            List<Event> events = new List<Event>();

            using SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            string sqlQuery = @"
        SELECT *
        FROM Event
        WHERE Id NOT IN (
            SELECT EventId
            FROM Registration
            WHERE UserId = @UserId
        )";

            using SqlCommand cmd = new SqlCommand(sqlQuery, con);
            cmd.Parameters.AddWithValue("@UserId", userId);

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                Event ev = new Event
                {
                    Id = Convert.ToInt32(dr["Id"]),
                    Title = dr["Title"].ToString(),
                    Description = dr["Description"].ToString(),
                    Date = Convert.ToDateTime(dr["Date"]),
                    Location = dr["Location"].ToString(),
                    Organizer = dr["Organizer"].ToString(),
                    ImageUrl = dr["ImageUrl"].ToString(),
                    MaxParticipants = Convert.ToInt32(dr["MaxParticipants"])
                };

                ev.CurrentParticipants = GetParticipantCountByEventId(ev.Id);

                events.Add(ev);
            }

            return events;
        }
        public bool IsUserRegistered(int userId, int eventId)
        {
            using SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            string sqlQuery = @"
                SELECT COUNT(*) 
                FROM Registration
                WHERE UserId = @UserId AND EventId = @EventId";

            using SqlCommand cmd = new SqlCommand(sqlQuery, con);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@EventId", eventId);

            int count = (int)cmd.ExecuteScalar();

            return count > 0;
        }
        public int GetParticipantCountByEventId(int eventId)
        {
            using SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            string sqlQuery = @"
                SELECT COUNT(*)
                FROM Registration
                WHERE EventId = @EventId";

            using SqlCommand cmd = new SqlCommand(sqlQuery, con);
            cmd.Parameters.AddWithValue("@EventId", eventId);

            return (int)cmd.ExecuteScalar();
        }
        public void CreateEvent(Event ev)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"INSERT INTO Event 
                (Title, Description, Date, Location, Organizer, ImageUrl, CurrentParticipants, MaxParticipants)
                VALUES (@Title, @Description, @Date, @Location, @Organizer, @ImageUrl, @CurrentParticipants, @MaxParticipants)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Title", ev.Title);
                    cmd.Parameters.AddWithValue("@Description", ev.Description);
                    cmd.Parameters.AddWithValue("@Date", ev.Date);
                    cmd.Parameters.AddWithValue("@Location", ev.Location);
                    cmd.Parameters.AddWithValue("@Organizer", ev.Organizer);
                    cmd.Parameters.AddWithValue("@ImageUrl", ev.ImageUrl);
                    cmd.Parameters.AddWithValue("@CurrentParticipants", ev.CurrentParticipants);
                    cmd.Parameters.AddWithValue("@MaxParticipants", ev.MaxParticipants);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public List<Event> GetAllEvents()
        {
            List<Event> events = new List<Event>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT * FROM Event ORDER BY Date ASC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        events.Add(new Event
                        {
                            Id = (int)reader["Id"],
                            Title = reader["Title"].ToString(),
                            Description = reader["Description"].ToString(),
                            Date = (DateTime)reader["Date"],
                            Location = reader["Location"].ToString(),
                            Organizer = reader["Organizer"].ToString(),
                            ImageUrl = reader["ImageUrl"].ToString(),
                            CurrentParticipants = (int)reader["CurrentParticipants"],
                            MaxParticipants = (int)reader["MaxParticipants"]
                        });
                    }
                }
            }

            return events;
        }


    }
}

