using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Twikker.Data.Models;

namespace Twikker.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            return View();
        }

        public async Task<ActionResult> LoginUser(string username, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, false, false);

            if (result.Succeeded)
                return Json("{\"type\": \"success\", \"message\": \"Login successful!\"}");

            return Json("{\"type\": \"error\", \"message\": \"Invalid password or username!\"}");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        public async Task<ActionResult> RegisterUser(string username, string email, string password, string confirmPassword)
        {
            if (password != confirmPassword)
                return Json("{\"class\": \"alert alert-danger\", \"description\": \"Password and Confirmation must match.\"}");

            var user = new ApplicationUser {UserName = username, Email = email};
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
                return Json("{\"class\": \"alert alert-success\", \"description\": \"Registration successful\"}");

            return Json($"{{\"class\": \"alert alert-danger\", \"description\": \"{result.Errors.First().Description}\"}}");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        //public async Task<IActionResult> Logout()
        //{
        //    await _signInManager.SignOutAsync();
        //    return Json("Success");
        //}
    }
}