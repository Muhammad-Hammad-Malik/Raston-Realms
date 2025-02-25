using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;
using System.Diagnostics;

namespace RastonRealm.Pages
{
    public class SignInClientModel : PageModel
    {
        public string errormessage = "";
        public List<LogInData> users = new List<LogInData>();
        public string CheckCredentials(string xemail, string xpassword)
        {
            foreach(LogInData obj in users)
            {
                if((obj.email == xemail)&&(obj.password == xpassword))
                {
                    return obj.id;
                }
            }
            return "null";
        }
        public void FillLogIndata()
        {
            string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=travel;Integrated Security=True";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM UserAccounts";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                LogInData user = new LogInData();
                                {
                                    user.email = reader["email"].ToString();
                                    user.password = reader["password"].ToString();
                                    user.id = reader["userID"].ToString();
                                };
                                users.Add(user);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions or log errors as needed
                // You may want to throw or return an error message
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        public void OnPost()
        {
            string xemail = Request.Form["email"];
            string xpassword = Request.Form["password"];
            FillLogIndata();
            string returnedID = CheckCredentials(xemail, xpassword);
            if (returnedID!="null")
            {
                HttpContext.Response.Cookies.Append("UserId", returnedID);
                Response.Redirect("ClientDashboard");
            }
            else
            {
                errormessage = "Incorrect Password or Email";
            }
        }
    }
    public class LogInData
    {
        public string email = "";
        public string password = "";
        public string id = "";
    }
}
