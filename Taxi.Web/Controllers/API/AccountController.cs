using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Taxi.Common.Enums;
using Taxi.Common.Models;
using Taxi.Web.Data;
using Taxi.Web.Data.Entities;
using Taxi.Web.Helpers;
using Taxi.Web.Resources;

namespace Taxi.Web.Controllers.API
{
    [Route("api/[Controller]")]
    public class AccountController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IImageHelper _imageHelper;

        public AccountController(
            DataContext dataContext,
            IUserHelper userHelper,
            IMailHelper mailHelper,
            IImageHelper imageHelper)
        {
            _dataContext = dataContext;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _imageHelper = imageHelper;
        }

        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] UserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "Bad request",
                    Result = ModelState
                });
            }

            CultureInfo cultureInfo = new CultureInfo(request.CultureInfo);
            Resource.Culture = cultureInfo;

            UserEntity user = await _userHelper.GetUserAsync(request.Email);
            if (user != null)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = Resource.UserAlreadyExists
                });
            }

            string picturePath = string.Empty;
            if (request.PictureArray != null && request.PictureArray.Length > 0)
            {
                picturePath = _imageHelper.UploadImage(request.PictureArray, "Users");
            }

            user = new UserEntity
            {
                Address = request.Address,
                Document = request.Document,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.Phone,
                UserName = request.Email,
                PicturePath = picturePath,
                UserType = request.UserTypeId == 1 ? UserType.User : UserType.Driver
            };

            IdentityResult result = await _userHelper.AddUserAsync(user, request.Password);
            if (result != IdentityResult.Success)
            {
                return BadRequest(result.Errors.FirstOrDefault().Description);
            }

            UserEntity userNew = await _userHelper.GetUserAsync(request.Email);
            await _userHelper.AddUserToRoleAsync(userNew, user.UserType.ToString());

            string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            string tokenLink = Url.Action("ConfirmEmail", "Account", new
            {
                userid = user.Id,
                token = myToken
            }, protocol: HttpContext.Request.Scheme);

            _mailHelper.SendMail(request.Email, Resource.EmailConfirmationSubject, $"<h1>{Resource.EmailConfirmationSubject}</h1>" +
                $"{Resource.EmailConfirmationBody}</br></br><a href = \"{tokenLink}\">{Resource.ConfirmEmail}</a>");

            return Ok(new Response
            {
                IsSuccess = true,
                Message = Resource.EmailConfirmationSent
            });
        }
    }
}
