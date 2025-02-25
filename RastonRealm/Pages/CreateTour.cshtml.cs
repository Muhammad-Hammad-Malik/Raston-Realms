using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using System;
using System.Data.SqlClient;
using System.IO;

namespace RastonRealm.Pages
{
    public class CreateTourModel : PageModel
    {
        public string errormessage = "";
        private readonly IWebHostEnvironment _environment;
        private readonly string _connectionString = "Data Source=.\\sqlexpress;Initial Catalog=travel;Integrated Security=True;TrustServerCertificate=True";
        public CreateTourModel(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        [BindProperty]
        public string TourName { get; set; }
        [BindProperty]
        public string TourArea { get; set; }
        [BindProperty]
        public DateTime TourDate { get; set; }
        [BindProperty]
        public string TourDescription { get; set; }
        [BindProperty]
        public decimal TourPrice { get; set; }
        [BindProperty]
        public int TourSeats { get; set; }
        [BindProperty]
        public IFormFile TourPhoto { get; set; }
        public void OnGet()
        {
            var isAdminSignedIn = HttpContext.Request.Cookies["Admin"];

            if (isAdminSignedIn != "true")
            {
                Response.Redirect("SignInAdmin");
                return;
            }
        }
        public void OnPost()
        {
            if (TourPhoto != null && TourPhoto.Length > 0)
            {
                try
                {
                    // Save file to wwwroot/Images directory
                    var uniqueFileName = $"{Guid.NewGuid().ToString()}_{TourPhoto.FileName}";
                    var imagePath = Path.Combine(_environment.WebRootPath, "Images", uniqueFileName);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        TourPhoto.CopyTo(stream);
                    }

                    // Store file path in the database
                    using (var connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();

                        var sql = @"INSERT INTO Tours 
                                    (TourName, TourArea, TourDate, TourDescription, TourPrice, TourSeats, TourPhoto) 
                                    VALUES 
                                    (@TourName, @TourArea, @TourDate, @TourDescription, @TourPrice, @TourSeats, @TourPhoto)";

                        using (var command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@TourName", TourName);
                            command.Parameters.AddWithValue("@TourArea", TourArea);
                            command.Parameters.AddWithValue("@TourDate", TourDate);
                            command.Parameters.AddWithValue("@TourDescription", TourDescription);
                            command.Parameters.AddWithValue("@TourPrice", TourPrice);
                            command.Parameters.AddWithValue("@TourSeats", TourSeats);
                            command.Parameters.AddWithValue("@TourPhoto", $"~/Images/{uniqueFileName}");

                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions (log or display an error message)
                    Console.WriteLine(ex.ToString());
                    return;
                }
            }
            else
            {
                errormessage = "Image not found";
                return;
            }
            Response.Redirect("AdminDashboard");
        }

    }
}