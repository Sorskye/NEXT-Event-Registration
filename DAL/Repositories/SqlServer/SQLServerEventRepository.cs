using System;
using System.Collections.Generic;
using DAL.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using NERA.Models;

namespace DAL.Repositories.SqlServer
{
    public class SQLServerEventRepository : IEventRepository
    {
        private readonly string connectionString;

        private const string EventSelect = @"
            SELECT
                e.ID,
                e.LocationID,
                e.Name,
                e.Description,
                e.Photo,
                e.Date_time,
                e.Cost,
                e.Max_participants,
                e.Lottery_prize,
                l.Name AS LocationName,
                organizer.Name AS OrganizerName,
                organizer.Email AS OrganizerEmail
            FROM Event e
            LEFT JOIN Location l ON e.LocationID = l.ID
            OUTER APPLY (
                SELECT TOP 1 u.Name, u.Email
                FROM User_Event ue
                INNER JOIN Users u ON ue.UserID = u.ID
                WHERE ue.EventID = e.ID
                ORDER BY ue.UserID
            ) organizer";

        public SQLServerEventRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public Event GetEventById(int id)
        {
            using SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            using SqlCommand cmd = new SqlCommand(EventSelect + " WHERE e.ID = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);

            using SqlDataReader dr = cmd.ExecuteReader();

            if (!dr.Read())
            {
                return null;
            }

            Event ev = ReadEvent(dr);
            dr.Close();

            ev.CurrentParticipants = GetParticipantCountByEventId(id);
            return ev;
        }

        public void RegisterUserForEvent(int userId, int eventId)
        {
            using SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            using SqlTransaction transaction = con.BeginTransaction();

            try
            {
                using SqlCommand existsCmd = new SqlCommand(@"
                    SELECT COUNT(*)
                    FROM Registration_Event re
                    INNER JOIN Registration_User ru ON re.RegistrationID = ru.RegistrationID
                    WHERE ru.UserID = @UserID AND re.EventID = @EventID",
                    con,
                    transaction);
                existsCmd.Parameters.AddWithValue("@UserID", userId);
                existsCmd.Parameters.AddWithValue("@EventID", eventId);

                int existingRegistrations = (int)existsCmd.ExecuteScalar();
                if (existingRegistrations > 0)
                {
                    transaction.Commit();
                    return;
                }

                using SqlCommand registrationCmd = new SqlCommand(
                    "INSERT INTO Registration DEFAULT VALUES; SELECT CONVERT(int, SCOPE_IDENTITY());",
                    con,
                    transaction);

                int registrationId = (int)registrationCmd.ExecuteScalar();

                using SqlCommand userCmd = new SqlCommand(
                    "INSERT INTO Registration_User (RegistrationID, UserID) VALUES (@RegistrationID, @UserID)",
                    con,
                    transaction);
                userCmd.Parameters.AddWithValue("@RegistrationID", registrationId);
                userCmd.Parameters.AddWithValue("@UserID", userId);
                userCmd.ExecuteNonQuery();

                using SqlCommand eventCmd = new SqlCommand(
                    "INSERT INTO Registration_Event (EventID, RegistrationID) VALUES (@EventID, @RegistrationID)",
                    con,
                    transaction);
                eventCmd.Parameters.AddWithValue("@EventID", eventId);
                eventCmd.Parameters.AddWithValue("@RegistrationID", registrationId);
                eventCmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public void UnregisterUserFromEvent(int userId, int eventId)
        {
            using SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            string sqlQuery = @"
                SELECT ru.RegistrationID
                FROM Registration_User ru
                INNER JOIN Registration_Event re ON ru.RegistrationID = re.RegistrationID
                WHERE ru.UserID = @UserID AND re.EventID = @EventID";

            List<int> registrationIds = new List<int>();

            using (SqlCommand findCmd = new SqlCommand(sqlQuery, con))
            {
                findCmd.Parameters.AddWithValue("@UserID", userId);
                findCmd.Parameters.AddWithValue("@EventID", eventId);

                using SqlDataReader reader = findCmd.ExecuteReader();
                while (reader.Read())
                {
                    registrationIds.Add(Convert.ToInt32(reader["RegistrationID"]));
                }
            }

            foreach (int registrationId in registrationIds)
            {
                using SqlTransaction transaction = con.BeginTransaction();

                try
                {
                    using SqlCommand deleteEventCmd = new SqlCommand(
                        "DELETE FROM Registration_Event WHERE RegistrationID = @RegistrationID",
                        con,
                        transaction);
                    deleteEventCmd.Parameters.AddWithValue("@RegistrationID", registrationId);
                    deleteEventCmd.ExecuteNonQuery();

                    using SqlCommand deleteUserCmd = new SqlCommand(
                        "DELETE FROM Registration_User WHERE RegistrationID = @RegistrationID",
                        con,
                        transaction);
                    deleteUserCmd.Parameters.AddWithValue("@RegistrationID", registrationId);
                    deleteUserCmd.ExecuteNonQuery();

                    using SqlCommand deleteRegistrationCmd = new SqlCommand(
                        "DELETE FROM Registration WHERE ID = @RegistrationID",
                        con,
                        transaction);
                    deleteRegistrationCmd.Parameters.AddWithValue("@RegistrationID", registrationId);
                    deleteRegistrationCmd.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public List<Event> GetRegisteredEventsByUser(int userId)
        {
            List<Event> events = new List<Event>();

            using SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            string sqlQuery = EventSelect + @"
                INNER JOIN Registration_Event re ON e.ID = re.EventID
                INNER JOIN Registration_User ru ON re.RegistrationID = ru.RegistrationID
                WHERE ru.UserID = @UserID
                ORDER BY e.Date_time ASC";

            using SqlCommand cmd = new SqlCommand(sqlQuery, con);
            cmd.Parameters.AddWithValue("@UserID", userId);

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                Event ev = ReadEvent(dr);
                ev.CurrentParticipants = GetParticipantCountByEventId(ev.Id);
                events.Add(ev);
            }

            return events;
        }

        public List<Event> GetEventsByUser(int userId)
        {
            List<Event> events = new List<Event>();

            using SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            string sqlQuery = EventSelect + @"
                INNER JOIN User_Event ue ON e.ID = ue.EventID
                WHERE ue.UserID = @UserID
                ORDER BY e.Date_time ASC";

            using SqlCommand cmd = new SqlCommand(sqlQuery, con);
            cmd.Parameters.AddWithValue("@UserID", userId);

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                Event ev = ReadEvent(dr);
                ev.CurrentParticipants = GetParticipantCountByEventId(ev.Id);
                events.Add(ev);
            }

            return events;
        }

        public List<Event> GetUpcomingEventsByUser(int userId)
        {
            List<Event> events = new List<Event>();

            using SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            string sqlQuery = EventSelect + @"
                WHERE e.ID NOT IN (
                    SELECT re.EventID
                    FROM Registration_Event re
                    INNER JOIN Registration_User ru ON re.RegistrationID = ru.RegistrationID
                    WHERE ru.UserID = @UserID
                )
                ORDER BY e.Date_time ASC";

            using SqlCommand cmd = new SqlCommand(sqlQuery, con);
            cmd.Parameters.AddWithValue("@UserID", userId);

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                Event ev = ReadEvent(dr);
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
                FROM Registration_Event re
                INNER JOIN Registration_User ru ON re.RegistrationID = ru.RegistrationID
                WHERE ru.UserID = @UserID AND re.EventID = @EventID";

            using SqlCommand cmd = new SqlCommand(sqlQuery, con);
            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@EventID", eventId);

            int count = (int)cmd.ExecuteScalar();

            return count > 0;
        }

        public int GetParticipantCountByEventId(int eventId)
        {
            using SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            string sqlQuery = "SELECT COUNT(*) FROM Registration_Event WHERE EventID = @EventID";

            using SqlCommand cmd = new SqlCommand(sqlQuery, con);
            cmd.Parameters.AddWithValue("@EventID", eventId);

            return (int)cmd.ExecuteScalar();
        }

        public void CreateEvent(Event ev)
        {
            CreateEvent(ev, null);
        }

        public void CreateEvent(Event ev, int creatorUserId)
        {
            CreateEvent(ev, (int?)creatorUserId);
        }

        private void CreateEvent(Event ev, int? creatorUserId)
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            int? locationId = ev.LocationId;

            if (!locationId.HasValue && !string.IsNullOrWhiteSpace(ev.LocationName))
            {
                locationId = GetOrCreateLocationId(conn, ev.LocationName);
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

                if (creatorUserId.HasValue)
                {
                    using SqlCommand userEventCmd = new SqlCommand(
                        "INSERT INTO User_Event (UserID, EventID) VALUES (@UserID, @EventID)",
                        conn,
                        transaction);
                    userEventCmd.Parameters.AddWithValue("@UserID", creatorUserId.Value);
                    userEventCmd.Parameters.AddWithValue("@EventID", eventId);
                    userEventCmd.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public List<Event> GetAllEvents()
        {
            List<Event> events = new List<Event>();

            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            using SqlCommand cmd = new SqlCommand(EventSelect + " ORDER BY e.Date_time ASC", conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Event ev = ReadEvent(reader);
                ev.CurrentParticipants = GetParticipantCountByEventId(ev.Id);
                events.Add(ev);
            }

            return events;
        }

        private static Event ReadEvent(SqlDataReader reader)
        {
            byte[]? photo = reader["Photo"] == DBNull.Value ? null : (byte[])reader["Photo"];

            return new Event
            {
                Id = Convert.ToInt32(reader["ID"]),
                LocationId = reader["LocationID"] == DBNull.Value ? null : Convert.ToInt32(reader["LocationID"]),
                Name = reader["Name"] == DBNull.Value ? null : reader["Name"].ToString(),
                Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                Photo = photo,
                DateTime = reader["Date_time"] == DBNull.Value ? null : Convert.ToDateTime(reader["Date_time"]),
                Cost = reader["Cost"] == DBNull.Value ? null : Convert.ToDecimal(reader["Cost"]),
                MaxParticipants = reader["Max_participants"] == DBNull.Value ? null : Convert.ToInt32(reader["Max_participants"]),
                LotteryPrize = reader["Lottery_prize"] == DBNull.Value ? null : Convert.ToDecimal(reader["Lottery_prize"]),
                LocationName = reader["LocationName"] == DBNull.Value ? null : reader["LocationName"].ToString(),
                Organizer = GetOrganizerDisplayName(reader),
                ImageUrl = photo == null ? "/images/placeholder-image.png" : $"data:image/jpeg;base64,{Convert.ToBase64String(photo)}"
            };
        }

        private static string? GetOrganizerDisplayName(SqlDataReader reader)
        {
            if (reader["OrganizerName"] != DBNull.Value)
            {
                return reader["OrganizerName"].ToString();
            }

            if (reader["OrganizerEmail"] != DBNull.Value)
            {
                return reader["OrganizerEmail"].ToString();
            }

            return null;
        }

        private static int GetOrCreateLocationId(SqlConnection conn, string locationName)
        {
            using SqlCommand findCmd = new SqlCommand("SELECT ID FROM Location WHERE Name = @Name", conn);
            findCmd.Parameters.AddWithValue("@Name", locationName.Trim());

            object existingId = findCmd.ExecuteScalar();

            if (existingId != null)
            {
                return Convert.ToInt32(existingId);
            }

            using SqlCommand insertCmd = new SqlCommand(
                "INSERT INTO Location (Name) VALUES (@Name); SELECT CONVERT(int, SCOPE_IDENTITY());",
                conn);
            insertCmd.Parameters.AddWithValue("@Name", locationName.Trim());

            return (int)insertCmd.ExecuteScalar();
        }
    }
}
