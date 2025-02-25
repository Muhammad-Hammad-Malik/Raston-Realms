using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RastonRealm.Pages
{
    public class SignInAdminModel : PageModel
    {
        public string errormessage = "";
        public void OnGet()
        {
        }
        bool CheckCredentials(string email, string password)
        {
            if((email == "admin@gmail.com") && (password=="awan2002"))
            {
                return true;
            }
            return false;
        }
        public void OnPost()
        {
            string xemail = Request.Form["email"];
            string xpassword = Request.Form["password"];
            if (CheckCredentials(xemail, xpassword))
            {
                HttpContext.Response.Cookies.Append("Admin", "true");
                Response.Redirect("AdminDashboard");
            }
            else
            {
                errormessage = "Incorrect Credentials";
            }
        }
    }
}
