using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Twikker.Data.Models;

namespace Twikker.Web.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ManageController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> ChangePassword(string oldPassword, string password, string confirmPassword)
        {
            if (oldPassword == null || password == null || confirmPassword == null)
                return Json("{\"class\": \"alert alert-danger\", \"description\": \"Please fill in every field.\"}");

            if (password != confirmPassword)
                return Json("{\"class\": \"alert alert-danger\", \"description\": \"Password and Confirmation must match.\"}");

            var user = await _userManager.GetUserAsync(User);
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, oldPassword, password);

            if (changePasswordResult.Succeeded)
                return Json("{\"class\": \"alert alert-success\", \"description\": \"Password changed successfully.\"}");

            return Json($"{{\"class\": \"alert alert-danger\", \"description\": \"{changePasswordResult.Errors.First().Description}\"}}");
        }
    }
}