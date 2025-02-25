using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Dapper;

namespace RastonRealm.Pages
{
    public class MarketPlaceModel : PageModel
    {
        string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=travel;Integrated Security=True;TrustServerCertificate=True";
        public List<TourData> Tours { get; set; } = new List<TourData>();
        public void OnGet()
        {
            var user = HttpContext.Request.Cookies["UserId"];
            if (user == null)
            {
                // Redirect to the desired page if the UserId cookie is not set
                Response.Redirect("SignInClient");
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
}
