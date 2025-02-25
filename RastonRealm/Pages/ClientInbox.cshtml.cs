using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.Data.SqlClient;
using Dapper;
namespace RastonRealm.Pages
{
    public class ClientInboxModel : PageModel
    {
        private readonly string _connectionString = "Data Source=.\\sqlexpress;Initial Catalog=travel;Integrated Security=True;TrustServerCertificate=True";
        public List<ChatData> chats { get; set; } = new List<ChatData>();
        public void OnGet()
        {
            var user = HttpContext.Request.Cookies["UserId"];
            if (user == null)
            {
                Console.WriteLine("empty here");
                // Redirect to the desired page if the UserId cookie is not set
                Response.Redirect("SignInClient");
                return;
            }
            else
            {
                Console.WriteLine(user.ToString());
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
            var user = HttpContext.Request.Cookies["UserId"];
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
                        Type = "user"
                    };

                    connection.Execute(insertQuery, parameters);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Response.Redirect("ClientInbox");
        }

    }
    public class ChatData
    {
        public string UserID { get; set;}
        public string message { get; set;}
        public DateTime date { get; set;}
        public string type { get; set;}

    }
}
