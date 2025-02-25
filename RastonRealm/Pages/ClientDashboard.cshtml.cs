using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.Data.SqlClient;
using Dapper;
namespace RastonRealm.Pages
{
    public class ClientDashboardModel : PageModel
    {
        private readonly string _connectionString = "Data Source=.\\sqlexpress;Initial Catalog=travel;Integrated Security=True;TrustServerCertificate=True";
        public SignUpInfo logInData { get; set; }
        public List<OrderData> orders { get; set; }
        public void OnGet()
        {

            var user = HttpContext.Request.Cookies["UserId"];
            if (user == null)
            {
                // Redirect to the desired page if the UserId cookie is not set
                Response.Redirect("SignInClient");
                return;
            }
            string UserID = user.ToString();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Retrieve LogInData from UserAccounts where userID matches
                    var query = "SELECT * FROM UserAccounts WHERE userID = @UserID";
                    logInData = connection.QueryFirstOrDefault<SignUpInfo>(query, new { UserID = int.Parse(UserID) });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Retrieve orders with TourName from Order and Tours tables
                    var orderQuery = @"
                SELECT o.OrderID, o.TourID, o.UserID, o.TotalSeats, o.TourDate, t.TourName
                FROM [Order] o
                JOIN Tours t ON o.TourID = t.TourID
                WHERE o.UserID = @UserID";

                    orders = connection.Query<OrderData>(orderQuery, new { UserID = int.Parse(UserID) }).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
    public class OrderData
    {
        public int OrderID { get; set;}
        public int TourID { get; set;}
        public int UserID { get; set;}
        public int TotalSeats { get; set;}
        public DateTime TourDate { get; set;}
        public string TourName { get; set;}
    }
}
