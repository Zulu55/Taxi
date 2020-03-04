using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Taxi.Web.Helpers;
using Taxi.Web.Models;

namespace Taxi.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IImageHelper _imageHelper;
        private readonly ICombosHelper _combosHelper;

        public AccountController(
            IUserHelper userHelper,
            IImageHelper imageHelper,
            ICombosHelper combosHelper)
        {
            _userHelper = userHelper;
            _imageHelper = imageHelper;
            _combosHelper = combosHelper;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _userHelper.LoginAsync(model);
                if (result.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(Request.Query["ReturnUrl"].First());
                    }

                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Username or password not valid.");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }

        public IActionResult Register()
        {
            AddUserViewModel model = new AddUserViewModel
            {
                UserTypes = _combosHelper.GetComboRoles()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                string path = string.Empty;

                if (model.PictureFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.PictureFile, "Users");
                }

                Data.Entities.UserEntity user = await _userHelper.AddUserAsync(model, path);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "This email is already used.");
                    model.UserTypes = _combosHelper.GetComboRoles();
                    return View(model);
                }

                LoginViewModel loginViewModel = new LoginViewModel
                {
                    Password = model.Password,
                    RememberMe = false,
                    Username = model.Username
                };

                Microsoft.AspNetCore.Identity.SignInResult result2 = await _userHelper.LoginAsync(loginViewModel);

                if (result2.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            model.UserTypes = _combosHelper.GetComboRoles();
            return View(model);
        }
    }
}
