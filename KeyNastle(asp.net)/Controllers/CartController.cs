using KeyNastle.Interface;
//using KeyNastle.Models;
using KeyNastle.Resources.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;
using System.Net.Http;

namespace KeyNastle.Controllers
{
	public class CartController : Controller
	{
        private readonly ILogger<CartController> logger;
        private readonly IUserClient client;
        private readonly IHttpContextAccessor httpContextAccessor;

        public CartController(ILogger<CartController> logger, IUserClient client, IHttpContextAccessor httpContextAccessor)
        {
            this.logger = logger;
            this.client = client;
            this.httpContextAccessor = httpContextAccessor;
        }

        [Authorize]
		public async Task<ActionResult<List<UserBusket>>> Index()
		{
            var token = httpContextAccessor.HttpContext.Request.Cookies["Authorization"];

            var result = await client.GetCart(token);

            return View(result.Content);
        }

        [Authorize]
        public async Task<IActionResult> BuyFullBusket()
        {
            var token = httpContextAccessor.HttpContext.Request.Cookies["Authorization"];

            var response = await client.BuyFullBusket(token);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Товар отправлен вам на почту регистрации";
            }
            else
            {
                TempData["DanderMessage"] = "Товар закончился";
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> DeleteProduct(int ProductId)
        {
            var token = httpContextAccessor.HttpContext.Request.Cookies["Authorization"];

            await client.DeleteFromBusket(token, ProductId);

            return RedirectToAction(nameof(Index));
        }
    }
}
