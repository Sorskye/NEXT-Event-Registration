using DAL.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NEXT.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly IUserRepository _userRepository;

        public LoginModel(IUserRepository userRepository)
        {
            this._userRepository = userRepository;
        }

        [BindProperty]
        public string? Email { get; set; }

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Vul je email in.";
                return Page();
            }

            try
            {
                var user = _userRepository.GetUserByEmail(Email);

                if (user == null)
                {
                    ErrorMessage = "Geen gebruiker gevonden met dit emailadres.";
                    return Page();
                }

                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", user.Name ?? user.Email ?? "Gebruiker");
                HttpContext.Session.SetString("UserRole", user.IsAdmin ? "Admin" : "User");
            }
            catch (SqlException)
            {
                ErrorMessage = "Kan geen verbinding maken met de database. Controleer of je op het juiste netwerk/VPN zit en probeer opnieuw.";
                return Page();
            }

            return RedirectToPage("/Home/Homepage");
        }
    }
}
