using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.Data.SqlClient;
using Dapper;
using static NuGet.Packaging.PackagingConstants;

namespace RastonRealm.Pages
{
    public class ViewTourInfoModel : PageModel
    {
        public List<OrderAdminData> orders = new List<OrderAdminData>();
        public string TourID { get; set; }
        public TourData tourData { get; set; }
        public string errormessage = "";
        private readonly string _connectionString = "Data Source=.\\sqlexpress;Initial Catalog=travel;Integrated Security=True;TrustServerCertificate=True";
        public void OnGet()
        {
            var isAdminSignedIn = HttpContext.Request.Cookies["Admin"];

            if (isAdminSignedIn != "true")
            {
                Response.Redirect("SignInAdmin");
                return;
            }
            TourID = Request.Query["TourID"];
            if (string.IsNullOrEmpty(TourID))
            {
                Response.Redirect("AdminDashboard");
                return;
            }
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "SELECT * FROM Tours WHERE TourID = @TourID";
                    tourData = connection.QueryFirstOrDefault<TourData>(query, new { TourID });
                }
            }
            catch (Exception ex)
            {
                errormessage = $"An error occurred: {ex.Message}";
            }
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var query = @"
                SELECT o.OrderID, o.TourID, o.UserID, o.TotalSeats, o.TourDate, u.Name as UserName
                FROM [Order] o
                JOIN UserAccounts u ON o.UserID = u.userID
                WHERE o.TourID = @TourID";
                    var parameters = new { TourID };

                    orders = connection.Query<OrderAdminData>(query, parameters).ToList();

                    if (orders.Count == 0)
                    {
                        // Handle case when no orders are found for the given TourID
                        errormessage = "No orders found.";
                    }
                }
            }
            catch (Exception ex)
            {
                errormessage = $"An error occurred: {ex.Message}";
            }
        }

    }
    public class OrderAdminData
    {
        public int OrderID { get; set; }
        public int TourID { get; set; }
        public int UserID { get; set; }
        public int TotalSeats { get; set; }
        public DateTime TourDate { get; set; }
        public string UserName { get; set; }
    }
}
