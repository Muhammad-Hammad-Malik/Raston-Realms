using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Dapper;

namespace RastonRealm.Pages
{
    public class AdminInboxChatModel : PageModel
    {
        private readonly string _connectionString = "Data Source=.\\sqlexpress;Initial Catalog=travel;Integrated Security=True;TrustServerCertificate=True";
        public List<ChatData> chats { get; set; } = new List<ChatData>();
        public string Username { get; set; }

        public void OnGet()
        {
            var isAdminSignedIn = HttpContext.Request.Cookies["Admin"];

            if (isAdminSignedIn != "true")
            {
                Response.Redirect("SignInAdmin");
                return;
            }
            Username = Request.Query["UserName"].ToString();
            var user = Request.Query["UserID"];
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(user))
            {
                Response.Redirect("AdminInbox");
                return;
            }
            string UserID = user.ToString();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var query = @"
                        SELECT * 
                        FROM chats 
                        WHERE userID = @UserID 
                        ORDER BY date ASC";

                    chats = connection.Query<ChatData>(query, new { UserID }).AsList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void OnPost()
        {
            Username = Request.Query["UserName"].ToString();
            var user = Request.Query["UserID"];
            string userID = user.ToString();

            string message = Request.Form["Message"];

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Insert data into chats table
                    var insertQuery = @"
                        INSERT INTO chats (userID, message, date, type)
                        VALUES (@UserID, @Message, @Date, @Type)";

                    var parameters = new
                    {
                        UserID = userID,
                        Message = message,
                        Date = DateTime.Now,
                        Type = "admin"
                    };

                    connection.Execute(insertQuery, parameters);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Response.Redirect("AdminInboxChat?UserID=" + userID + "&UserName=" + Username);
        }
    }
}
