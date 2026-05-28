using System.Security.Claims;
using DAL.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DAL.Models;
using Microsoft.AspNetCore.Authentication;

namespace NEXT.Pages.Auth
{
    public class LoginRedirectModel : PageModel
    {
        private readonly IUserRepository _userRepository;

        public LoginRedirectModel(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [BindProperty]
        public string? Email { get; set; }

        public string? Auth0Id { get; set; }
        public string? Message { get; set; }

        public IActionResult OnGet(int? eventId)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                var returnUrl = Url.Page("/LoginRedirect", new { eventId });

                return Challenge(new AuthenticationProperties
                {
                    RedirectUri = returnUrl
                }, "Auth0"); 
            }

            Message = "redirecting";

            Email = User.FindFirst(ClaimTypes.Email)?.Value
                 ?? User.FindFirst("email")?.Value;

            Auth0Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Auth0Id))
            {
                Message = "Er is een fout opgetreden tijdens het ophalen van je accountgegevens.";
                return Page();
            }

            var user = _userRepository.GetUserByEmail(Email);

            if (user == null)
            {
                User newUser = new User
                {
                    Name = "temp",
                    Email = Email,
                    Auth0Id = Auth0Id,
                    Role = "member"
                };

                bool success = _userRepository.CreateUser(newUser);

                if (!success)
                {
                    Message = "Gebruiker kon niet worden aangemaakt.";
                    return Page();
                }

                user = _userRepository.GetUserByEmail(Email);

                if (user == null)
                {
                    Message = "Gebruiker aangemaakt, maar niet teruggevonden.";
                    return Page();
                }
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.Name ?? user.Email ?? "Gebruiker");
            HttpContext.Session.SetString("UserRole", user.IsAdmin ? "Admin" : "User");

            if (eventId.HasValue)
            {
                return RedirectToPage("/Home/QrLandingPage", new { eventId = eventId.Value });
            }

            return RedirectToPage("/Home/Homepage");
        }
    }
}

