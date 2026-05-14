using Microsoft.Data.SqlClient;
using NERA.Models;

namespace DAL.Repositories.SqlServer
{
    internal static class SqlServerEventMapper
    {
        internal const string EventSelect = @"
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

        internal static Event ReadEvent(SqlDataReader reader)
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
    }
}
