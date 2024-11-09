using API1.Models;
using API1.Resources.Classes;
using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace API1.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class IdentificationController : ControllerBase
	{
        private readonly ILogger<IdentificationController> _logger;
        public IdentificationController(ILogger<IdentificationController> logger) { _logger = logger; }

        ClassContext _context = new ClassContext();

        [HttpPost(template: "Registrations")]
		public ActionResult<UserT> Registrations(UserT userget)
		{
			if (userget == null)
			{
				ConsoleHelper.PrintFail("Сервер получил пустой файл");
				return BadRequest("Сервер получил пустой файл");
			}
			else if (_context.UserTs.FirstOrDefault(x => x.Login == userget.Login) != null)
			{
                ConsoleHelper.PrintFail("Данный логин уже зарегистрирован");
                return BadRequest("Данный логин уже зарегистрирован");
			}
			else if (_context.UserTs.FirstOrDefault(x => x.Email == userget.Email) != null)
			{
                ConsoleHelper.PrintFail("Данная почта уже зарегистрирована");
                return BadRequest("Данная почта уже зарегистрирована");
			}

            string salt = BCrypt.Net.BCrypt.GenerateSalt();

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userget.Password, salt);

			_logger.LogInformation("Salt: " + salt);

			userget.Password = hashedPassword;
			userget.Salt = salt;
			userget.RoleId = 1;

			userget.RegistrationDate = DateOnly.ParseExact(DateTime.Now.ToString("yyyy-MM-dd"), "yyyy-MM-dd", null);

            try
            {
                ConsoleHelper.PrintSuccessfull("Пользователь добавлен. Пользователь: " + userget.Login);

                _context.UserTs.Add(userget);
                _context.SaveChanges();
            }
            catch (Exception ex) 
            {
                ConsoleHelper.PrintFail("Ошибка добавление пользователя. " + ex.Message.ToString());
            }
            

			return Ok();
		}

		[HttpGet(template: "Authorization")]
		public ActionResult<UserT> Authorization(string Login, string Password)
		{
			if (Login == null || Password == null)
			{
                ConsoleHelper.PrintFail("Заполните поля");
                return BadRequest("Заполните поля");
			}

            var SearthAndCheckLogin = _context.UserTs.FirstOrDefault(x => x.Login == Login);

			if (SearthAndCheckLogin == null)
			{
                ConsoleHelper.PrintFail("Неверные данные авторизации");
                return NotFound("Неверные данные");
            }

			string HashedPassword = BCrypt.Net.BCrypt.HashPassword(Password, SearthAndCheckLogin.Salt);

            var SearthAndCheckUser = _context.UserTs.FirstOrDefault(x => x.Login == Login && x.Password == HashedPassword);

            if (SearthAndCheckUser == null)
			{
                ConsoleHelper.PrintFail("Неверные данные авторизации");
                return NotFound("Неверные данные");
            }

            var token = Generate(SearthAndCheckUser);

            var result = new UserTokenResponse
            {
                Token = token,
                User = SearthAndCheckUser
            };

            ConsoleHelper.PrintSuccessfull("Успешная авторизация. Пользователь: " + SearthAndCheckUser.Login);

            return Ok(result);
		}

        private string Generate(UserT user) // генерация токена для пользователя
        {
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("mysupersecret_secretsecretsecretkey!123"));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Login),
                new Claim(ClaimTypes.Name, user.NickName),
                new Claim(ClaimTypes.Role, user.RoleId.ToString())
            };

            var token = new JwtSecurityToken("http://localhost/", "http://localhost/",
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
