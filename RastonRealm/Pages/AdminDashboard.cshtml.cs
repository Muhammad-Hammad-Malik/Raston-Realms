using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Dapper;
namespace RastonRealm.Pages
{
    public class AdminDashboardModel : PageModel
    {
        string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=travel;Integrated Security=True;TrustServerCertificate=True";
        public List<TourData> Tours { get; set; } = new List<TourData>();
        public void OnGet()
        {
            var isAdminSignedIn = HttpContext.Request.Cookies["Admin"];

            if (isAdminSignedIn != "true")
            {
                Response.Redirect("SignInAdmin");
                return;
            }
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var query = "SELECT * FROM Tours";
                Tours = connection.Query<TourData>(query).AsList();
            }
        }
    }
    public class TourData
    {
        public int TourID { get; set; } // INT
        public string TourName { get; set; } // NVARCHAR(255)
        public string TourArea { get; set; } // NVARCHAR(255)
        public DateTime TourDate { get; set; } // DATE
        public string TourDescription { get; set; } // NVARCHAR(MAX)
        public decimal TourPrice { get; set; } // DECIMAL(10,2)
        public int TourSeats { get; set; } // INT
        public int FilledSeats { get; set; } // INT
        public string TourPhoto { get; set; } // NVARCHAR(255)
    }

}
