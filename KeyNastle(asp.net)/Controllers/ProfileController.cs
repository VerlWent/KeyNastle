using KeyNastle.Interface;
using KeyNastle.Resources.Classes;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using KeyNastle.Resources.Classes.DataUserInfoFolder;

namespace KeyNastle.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ILogger<ProfileController> logger;
        private readonly IUserClient client;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ProfileController(ILogger<ProfileController> logger, IUserClient client, IHttpContextAccessor httpContextAccessor)
        {
            this.logger = logger;
            this.client = client;
            this.httpContextAccessor = httpContextAccessor;
        }

        [Authorize]
        public async Task<ActionResult> Index()
        {
            var token = httpContextAccessor.HttpContext.Request.Cookies["Authorization"];

            var data = await client.GetDataUser(token);

            return View(data.Content);
        }

        [Authorize]
        public async Task<ActionResult<UserData>> PersonalData()
        {
            var token = httpContextAccessor.HttpContext.Request.Cookies["Authorization"];

            var data = await client.GetDataUser(token);

            return View(data.Content);
        }

        [Authorize]
        public async Task<ActionResult<UserPurchase>> PurchaseHistory()
        {
            var token = httpContextAccessor.HttpContext.Request.Cookies["Authorization"];

            var datahistory = await client.GetUserPurchases(token);

            return View(datahistory.Content);
        }

        [Authorize]
        public async Task<ActionResult<UserData>> SaveData(string OldPassword, string NewPassword, string NickName)
        {
            if (NickName.Length <= 4)
            {
                TempData["DanderMessage"] = "Имя пользователя должен быть больше 4 символов";
                return RedirectToAction("PersonalData", "Profile");
            }
            if (NewPassword == null && OldPassword != null)
            {
                TempData["DanderMessage"] = "Введите новый пароль";
                return RedirectToAction("PersonalData", "Profile");
            }
            if (NewPassword != null && OldPassword == null)
            {
                TempData["DanderMessage"] = "Введите действующий пароль";
                return RedirectToAction("PersonalData", "Profile");
            }
            
            var token = httpContextAccessor.HttpContext.Request.Cookies["Authorization"];

            logger.LogInformation(OldPassword);
            logger.LogInformation(NewPassword);
            logger.LogInformation(NickName);

            var result = await client.ChangeData(token, NickName, OldPassword, NewPassword);

            if (result.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                TempData["DanderMessage"] = "Указанный пароль недействителен";
            }
            else if (result.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Ваши данные успешно сохранены";
            }

            return RedirectToAction("PersonalData", "Profile");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
