using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
namespace RastonRealm.Pages
{
    public class SignUpModel : PageModel
    {
        public string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=travel;Integrated Security=True";
        public SignUpInfo SignUpObject = new SignUpInfo();
        public string errormessage = "";
        public List<SignUpInfo> userAccounts = new List<SignUpInfo>();
        public List<LogInData> users = new List<LogInData>();

        public void FillLogIndata()
        {
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
        public string CheckCredentials(string xemail, string xpassword)
        {
            foreach (LogInData obj in users)
            {
                if ((obj.email == xemail) && (obj.password == xpassword))
                {
                    return obj.id;
                }
            }
            return "null";
        }
        public bool checkAvailable(string email)
        {
            foreach (SignUpInfo user in userAccounts)
            {
                if (user.email == email)
                {
                    return false;
                }
            }
            return true;
        }

        public void FillData()
        {
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
                                SignUpInfo user = new SignUpInfo();
                                {
                                    user.name = reader["name"].ToString();
                                    user.email = reader["email"].ToString();
                                    user.password = reader["password"].ToString();
                                };
                                userAccounts.Add(user);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            { 
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        public void createEntry()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "INSERT INTO UserAccounts " +
                                 "(name, email, password) " +
                                 "VALUES (@Name, @Email, @Password)";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Name", SignUpObject.name);
                        command.Parameters.AddWithValue("@Email", SignUpObject.email);
                        command.Parameters.AddWithValue("@Password", SignUpObject.password);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        public void OnPost()
        {
            FillData();
            SignUpObject.name = Request.Form["name"];
            SignUpObject.email = Request.Form["email"];
            SignUpObject.password = Request.Form["password"];
            if(!checkAvailable(SignUpObject.email))
            {
                errormessage = "Email Already In Use";
                return;
            }
            createEntry();
            FillLogIndata();
            string returnedID = CheckCredentials(SignUpObject.email, SignUpObject.password);
            if (returnedID != "null")
            {
                HttpContext.Response.Cookies.Append("UserId", returnedID);
                Response.Redirect("Index");
            }
            else
            {
                errormessage = "Error";
                return;
            }
            Response.Redirect("ClientDashboard");
        }
    }

    public class SignUpInfo
    {
        public string name="";
        public string email = "";
        public string password = "";
    }
}
