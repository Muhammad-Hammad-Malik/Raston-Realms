using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;


namespace RastonRealm.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            HttpContext.Response.Cookies.Delete("UserId");
            HttpContext.Response.Cookies.Append("UserId", "", new CookieOptions
            {
                Expires = DateTime.Now.AddYears(-1)
            });
            var userId = HttpContext.Request.Cookies["UserId"];

            HttpContext.Response.Cookies.Delete("Admin");
            HttpContext.Response.Cookies.Append("Admin", "", new CookieOptions
            {
                Expires = DateTime.Now.AddYears(-1)
            });

        }
    }
}