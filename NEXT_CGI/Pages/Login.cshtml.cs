using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NEXT.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty] // Dit zorgt ervoor dat de input uit het formulier automatisch wordt gekoppeld aan deze properties
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            return RedirectToPage("/Homepage");
        }
    }
}