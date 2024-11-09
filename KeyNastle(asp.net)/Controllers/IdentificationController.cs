using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
//using KeyNastle.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Newtonsoft.Json;
using System.Diagnostics;
using KeyNastle.Resources.Classes;
using Microsoft.Extensions.Logging;
using KeyNastle.Interface;

namespace KeyNastle.Controllers
{
	public class IdentificationController : Controller
	{
        private readonly ILogger<IdentificationController> logger;
        private readonly IUserClient client;

        public IdentificationController(ILogger<IdentificationController> logger, IUserClient client)
        {
            this.logger = logger;
            this.client = client;
        }

        [AllowAnonymous]
		public IActionResult AuthorizationView()
		{
			return View();
		}
		[AllowAnonymous]
		public IActionResult RegistrationView()
		{
			return View();
		}

		[AllowAnonymous]
		[IgnoreAntiforgeryToken]
		public async Task<IActionResult> Register(UserInfo model)
		{
			if (model.Login == null || model.NickName == null || model.Email == null || model.Password == null || model.RepeatPassword == null)
			{
				TempData["DanderMessage"] = "Заполните все поля";
				return RedirectToAction("RegistrationView", "Identification");
			}
			else if (model.Password != model.RepeatPassword)
			{
				TempData["DanderMessage"] = "Пароли не совпадают";
				return RedirectToAction("RegistrationView", "Identification");
			}

			var result = await client.RegisterUser(model);

			if (!result.IsSuccessStatusCode)
			{
				if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
				{
					TempData["DanderMessage"] = "Логин или почта уже заняты";
                }

                return RedirectToAction("RegistrationView", "Identification");
            }
			
			return RedirectToAction("AuthorizationView", "Identification");
			
		}
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
		public async Task<IActionResult> Authorization(string Login, string Password)
		{
			if (Login == null || Login == "" || Password == null || Password == "")
			{
                TempData["DanderMessage"] = "Укажите данные";
                return RedirectToAction("AuthorizationView", "Identification");
            }

			var result = await client.GetUser(Login, Password);

			var response = result.Content;

			if (result.StatusCode != System.Net.HttpStatusCode.OK)
			{
				if (result.StatusCode == System.Net.HttpStatusCode.NotFound)
				{
                    TempData["DanderMessage"] = "Неверные данные";
                }
                
                return RedirectToAction("AuthorizationView", "Identification");
            }

			string token = "Bearer " + response.Token;

            UserInfo userinfo = response.User;

            Response.Cookies.Append("Authorization", token, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict
            });

            logger.LogInformation("-" + token + "-");

			await Authenticate(userinfo);

            return RedirectToAction("Index", "Home");
			
        }

		[AllowAnonymous]
		private async Task Authenticate(UserInfo model)
		{
			// создаем один claim
			var claims = new List<Claim>
			{
				new Claim(ClaimsIdentity.DefaultNameClaimType, "UserDataCookie"),
				new Claim("UserLogin", model.Login.ToString()),
				new Claim("UserPassword", model.Password.ToString())
			};
			// создаем объект ClaimsIdentity
			ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
            
        }
    }
}
