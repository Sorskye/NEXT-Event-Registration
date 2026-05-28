using DAL.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using NERA.Models;

namespace DAL.Repositories.SqlServer
{
    public class SqlServerEventRegistrationRepository : IEventRegistrationRepository
    {
        private readonly string connectionString;

        public SqlServerEventRegistrationRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public enum AttendanceUpdateResult
        {
            Success,
            NotRegistered,
            AlreadyAttended
        }

        public void RegisterUserForEvent(int userId, int eventId)
        {
            using SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            using SqlTransaction transaction = con.BeginTransaction();

            try
            {
                if (IsUserAlreadyRegistered(con, transaction, userId, eventId))
                {
                    transaction.Commit();
                    return;
                }

                int registrationId = CreateRegistration(con, transaction);

                LinkRegistrationToUser(con, transaction, registrationId, userId);
                LinkRegistrationToEvent(con, transaction, registrationId, eventId);

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

            string sqlQuery = SqlServerEventMapper.EventSelect + @"
                INNER JOIN Registration_Event re ON e.ID = re.EventID
                INNER JOIN Registration_User ru ON re.RegistrationID = ru.RegistrationID
                WHERE ru.UserID = @UserID
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

        public List<Event> GetUpcomingEventsByUser(int userId)
        {
            List<Event> events = new List<Event>();

            using SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            string sqlQuery = SqlServerEventMapper.EventSelect + @"
                WHERE e.ID NOT IN (
                    SELECT re.EventID
                    FROM Registration_Event re
                    INNER JOIN Registration_User ru ON re.RegistrationID = ru.RegistrationID
                    WHERE ru.UserID = @UserID
                )
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

        private static bool IsUserAlreadyRegistered(SqlConnection con, SqlTransaction transaction, int userId, int eventId)
        {
            string query = @"
                 SELECT CAST(
                 CASE WHEN EXISTS (
                 SELECT 1
                 FROM Registration_Event re
                 INNER JOIN Registration_User ru ON re.RegistrationID = ru.RegistrationID
                 WHERE ru.UserID = @UserID AND re.EventID = @EventID) THEN 1 ELSE 0 END
                 AS bit)";

            using SqlCommand cmd = new SqlCommand(query, con, transaction);
            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@EventID", eventId);

            return (bool)cmd.ExecuteScalar();
        }

        private static int CreateRegistration(SqlConnection con, SqlTransaction transaction)
        {
            using SqlCommand cmd = new SqlCommand(
                "INSERT INTO Registration (Registration_date, Attended) VALUES (GETDATE(), 0); SELECT CONVERT(int, SCOPE_IDENTITY());",
                con,
                transaction);

            return (int)cmd.ExecuteScalar();
        }

        private static void LinkRegistrationToUser(SqlConnection con, SqlTransaction transaction, int registrationId, int userId)
        {
            using SqlCommand cmd = new SqlCommand(
                "INSERT INTO Registration_User (RegistrationID, UserID) VALUES (@RegistrationID, @UserID)",
                con,
                transaction);
            cmd.Parameters.AddWithValue("@RegistrationID", registrationId);
            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.ExecuteNonQuery();
        }

        private static void LinkRegistrationToEvent(SqlConnection con, SqlTransaction transaction, int registrationId, int eventId)
        {
            using SqlCommand cmd = new SqlCommand(
                "INSERT INTO Registration_Event (EventID, RegistrationID) VALUES (@EventID, @RegistrationID)",
                con,
                transaction);
            cmd.Parameters.AddWithValue("@EventID", eventId);
            cmd.Parameters.AddWithValue("@RegistrationID", registrationId);
            cmd.ExecuteNonQuery();
        }

        public AttendanceUpdateResult TryMarkAttendance(int userId, int eventId, bool attended)
        {
            using SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            string selectQuery = @"
                SELECT r.ID, r.Attended
                FROM Registration r
                INNER JOIN Registration_User ru ON r.ID = ru.RegistrationID
                INNER JOIN Registration_Event re ON r.ID = re.RegistrationID
                WHERE ru.UserID = @UserID
                AND re.EventID = @EventID";

            using SqlCommand selectCmd = new SqlCommand(selectQuery, con);
            selectCmd.Parameters.AddWithValue("@UserID", userId);
            selectCmd.Parameters.AddWithValue("@EventID", eventId);

            using SqlDataReader reader = selectCmd.ExecuteReader();

            if (!reader.Read())
            {
                return AttendanceUpdateResult.NotRegistered;
            }

            int registrationId = Convert.ToInt32(reader["ID"]);
            bool? currentAttended = reader["Attended"] == DBNull.Value
                ? null
                : Convert.ToBoolean(reader["Attended"]);

            reader.Close();

            if (currentAttended == true)
            {
                return AttendanceUpdateResult.AlreadyAttended;
            }

            string updateQuery = @"
                UPDATE Registration
                SET Attended = @Attended
                WHERE ID = @RegistrationID";

            using SqlCommand updateCmd = new SqlCommand(updateQuery, con);
            updateCmd.Parameters.AddWithValue("@Attended", attended);
            updateCmd.Parameters.AddWithValue("@RegistrationID", registrationId);

            updateCmd.ExecuteNonQuery();

            return AttendanceUpdateResult.Success;
        }
    }
}
