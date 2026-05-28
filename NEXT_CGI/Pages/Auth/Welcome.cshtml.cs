using DAL.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NEXT.Pages.Auth
{
    public class WelcomeModel : PageModel
    {
        private readonly IUserRepository _userRepository;

        public WelcomeModel(IUserRepository userRepository)
        {
            this._userRepository = userRepository;
        }

        [BindProperty]
        public string? Email { get; set; }

        public string? ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToPage("/Home/Homepage");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            
            return RedirectToPage("/Auth/Login");
        }
    }
}
