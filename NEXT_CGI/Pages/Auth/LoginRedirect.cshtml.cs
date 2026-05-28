using System.Security.Claims;
using DAL.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DAL.Models;

namespace NEXT.Pages.Auth
{
    public class LoginRedirectModel : PageModel
    {
        private readonly IUserRepository _userRepository;

        public LoginRedirectModel(IUserRepository userRepository)
        {
            this._userRepository = userRepository;
        }

        [BindProperty]
        public string? Email { get; set; }
        public string? Auth0Id { get; set; }
        

        public string? message { get; set; }

        public IActionResult OnGet()
        {
            message = "redirecting";
            Console.WriteLine("redirecting..");
            
            // check if values are empty
            if (HttpContext.Session.GetString("UserId") == null)
            {
                Email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;
                Auth0Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            }

            if (Email == null || Auth0Id == null)
            {
                message = "Er is een fout opgetreden tijdens het achterhalen van jou account. Probeer het opnieuw. 'NO_DATA_FROM_AUTH_PROVIDER'";
                return Page();
            }
            
            Console.WriteLine(Email);
            
            var user = _userRepository.GetUserByEmail(Email);
            if (user == null)
            {
                message = "NO_DB_MATCH_FROM_AUTH_USER::" +Auth0Id;
                User NewUser = new User();
                NewUser.Name = "temp";
                NewUser.Email = Email;
                NewUser.Auth0Id = Auth0Id;
                NewUser.Role = "member";
                
                bool success = _userRepository.CreateUser(NewUser);
                if (success == false)
                {
                    if (Auth0Id == null)
                    {
                        Auth0Id = "?";}

                    if (user.Id == null)
                    {
                        user.Id = 0;
                    }
                    message = "ERROR::DB_RET_FALSE::AUTH0ID="+Auth0Id+"::USERID="+user.Id;
                    return Page();
                }
                message = "OK";
                
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", user.Name ?? user.Email ?? "Gebruiker");
                HttpContext.Session.SetString("UserRole", user.IsAdmin ? "Admin" : "User");
                // create user
            }else
            {
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", user.Name ?? user.Email ?? "Gebruiker");
                HttpContext.Session.SetString("UserRole", user.IsAdmin ? "Admin" : "User");
            }


            return RedirectToPage("/Home/Homepage");


            return Page();
            // if user is in database, set values and continue

            // if user is not in database, create user, set values and continue
        }

    }
}
