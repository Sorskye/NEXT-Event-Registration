using DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NEXT.Pages.Auth
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string? Email { get; set; }

        [BindProperty]
        public string? Password { get; set; }

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Vul zowel je email als wachtwoord in.";
                return Page();
            }

            UserRepository_SQLServer userRepo = new UserRepository_SQLServer();
            var user = userRepo.GetUserByEmailAndPassword(Email, Password);

            if (user == null)
            {
                ErrorMessage = "Onjuiste inloggegevens.";
                return Page();
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.SetString("UserRole", user.Role ? "Admin" : "User");

            return RedirectToPage("/Home/Homepage");
        }
    }
}

