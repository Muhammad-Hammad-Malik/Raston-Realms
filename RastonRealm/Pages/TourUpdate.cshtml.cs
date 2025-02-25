using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.Data.SqlClient;
using Dapper;

namespace RastonRealm.Pages
{
    public class TourUpdateModel : PageModel
    {
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
            if(string.IsNullOrEmpty(TourID) ) 
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
        }
        public void OnPost()
        {
            TourID = Request.Query["TourID"];
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
                return;
            }
            string TourName = Request.Form["tourName"];
            string TourArea = Request.Form["tourArea"];
            DateTime.TryParse(Request.Form["tourDate"], out DateTime TourDate);
            string TourDescription = Request.Form["tourDescription"];
            string TourPrice = Request.Form["tourPrice"];
            string TourSeats = Request.Form["tourSeats"];

            try
            {
                int TourSeatsInt = Convert.ToInt32(TourSeats);
                if (TourSeatsInt < tourData.FilledSeats)
                {
                    errormessage = "Already more are booked you cant set lower than " + tourData.FilledSeats.ToString();
                    return;
                }
            }
            catch (Exception ex) 
            {
                TourSeats = tourData.TourSeats.ToString();
                return;
            }
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Update only specific columns (TourName, TourArea, TourDate, TourDescription, TourPrice, TourSeats)
                    var updateQuery = @"
                UPDATE Tours
                SET TourName = @TourName,
                    TourArea = @TourArea,
                    TourDate = @TourDate,
                    TourDescription = @TourDescription,
                    TourPrice = @TourPrice,
                    TourSeats = @TourSeats
                WHERE TourID = @TourID";

                    connection.Execute(updateQuery, new
                    {
                        TourID,
                        TourName,
                        TourArea,
                        TourDate,
                        TourDescription,
                        TourPrice,
                        TourSeats
                    });
                }
            }
            catch (Exception ex)
            {
                errormessage = $"An error occurred while updating: {ex.Message}";
                return;
            }
            Response.Redirect("AdminDashboard");
        }
    }
}
