using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Dapper;

namespace RastonRealm.Pages
{
    public class CheckOutModel : PageModel
    {
        public string TourID { get; set; }
        public TourData tourData { get; set; }
        public int SeatsLeft = 0;
        public string errormessage = "";
        private readonly string _connectionString = "Data Source=.\\sqlexpress;Initial Catalog=travel;Integrated Security=True;TrustServerCertificate=True";

        public void OnGet()
        {
            var userId = HttpContext.Request.Cookies["UserId"];

            if (userId == null)
            {
                Response.Redirect("SignInClient");
                return;
            }
            TourID = Request.Query["TourID"];
            if(TourID == null) 
            {
                Response.Redirect("ClientDashboard");
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
                return;
            }
            SeatsLeft = tourData.TourSeats - tourData.FilledSeats;
        }

        public void OnPost()
        {
            string TourID = Request.Query["TourID"];
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

            SeatsLeft = tourData.TourSeats - tourData.FilledSeats;
            string inputSeats = Request.Form["inputSeats"];
            TourID = Request.Query["TourID"];
            string UserID = HttpContext.Request.Cookies["UserId"].ToString();
            int inputSeatsInt;

            try
            {
                inputSeatsInt = Convert.ToInt32(inputSeats);

                if (inputSeatsInt > SeatsLeft)
                {
                    errormessage = "Not enough seats available.";
                    return; // Exit the method if there are not enough seats
                }

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Insert data into [Order] table
                    var insertQuery = @"
                        INSERT INTO [Order] (TourID, UserID , TotalSeats, TourDate)
                        VALUES (@TourID, @UserID, @TotalSeats, @TourDate)";

                    var parameters = new
                    {
                        TourID = int.Parse(TourID),
                        UserID = UserID,
                        TotalSeats = inputSeatsInt,
                        TourDate = tourData.TourDate
                    };

                    connection.Execute(insertQuery, parameters);
                }
            }
            catch (Exception ex)
            {
                errormessage = $"An error occurred while adding order: {ex.Message}";
                return;
            }

            try
            {
                inputSeatsInt = Convert.ToInt32(inputSeats);
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Update FilledSeats in the Tours table
                    var updateQuery = @"
                        UPDATE Tours
                        SET FilledSeats = FilledSeats + @InputSeats
                        WHERE TourID = @TourID";

                    var updateParameters = new
                    {
                        InputSeats = inputSeatsInt,
                        TourID = int.Parse(TourID)
                    };

                    connection.Execute(updateQuery, updateParameters);

                }
            }
            catch (Exception ex)
            {
                errormessage = $"An error occurred while adding order: {ex.Message}";
                return;
            }
            Response.Redirect("ClientDashboard");
        }
    }
}
