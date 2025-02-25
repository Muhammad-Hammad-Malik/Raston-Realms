using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Dapper;

namespace RastonRealm.Pages
{
    public class AdminInboxModel : PageModel
    {
        private readonly string _connectionString = "Data Source=.\\sqlexpress;Initial Catalog=travel;Integrated Security=True;TrustServerCertificate=True";
        public List<InboxPreInfo> info { get; set; } = new List<InboxPreInfo>();

        public void OnGet()
        {
            var isAdminSignedIn = HttpContext.Request.Cookies["Admin"];

            if (isAdminSignedIn != "true")
            {
                Response.Redirect("SignInAdmin");
                return;
            }
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var query = @"
                        SELECT
                            c.userID,
                            MAX(u.name) AS username,
                            MAX(c.date) AS LatestDate,
                            MAX(CASE WHEN c.type = 'user' THEN c.message ELSE NULL END) AS LatestMessage
                        FROM
                            chats c
                        JOIN
                            UserAccounts u ON c.userID = u.userID
                        GROUP BY
                            c.userID";

                    info = connection.Query<InboxPreInfo>(query).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }

    public class InboxPreInfo
    {
        public int userID { get; set; }
        public string username { get; set; }
        public DateTime LatestDate { get; set; }
        public string LatestMessage { get; set; }
    }
}
