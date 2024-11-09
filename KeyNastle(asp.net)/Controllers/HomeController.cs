using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
//using KeyNastle.Models;
using System.Text;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using KeyNastle.Interface;
using KeyNastle.Resources.Classes;
using Microsoft.Extensions.Logging;
using KeyNastle.Resources.Classes.ProductInfoFolder;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Runtime.CompilerServices;
using KeyNastle.Models;
using System.Diagnostics;

namespace KeyNastle.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly IUserClient client;
        private readonly IHttpContextAccessor httpContextAccessor;

        public HomeController(ILogger<HomeController> logger, IUserClient client, IHttpContextAccessor httpContextAccessor)
        {
            this.client = client;
            this.httpContextAccessor = httpContextAccessor;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var result = await client.GetProduct();

            if (result.Content == null || result.Content.Count() <= 0 || result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("Error");
            }

            ViewData["ProductName"] = "Все товары";

            return View(result.Content);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Searth(string? NameProduct)
        {
            var result = await client.GetProductSearth(NameProduct);

            if (result.Content == null || result.Content.Count() <= 0 || result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("Error");
            }

            ViewData["ProductName"] = "Поиск по названию товара: " + NameProduct;

            return View(result.Content);
        }

        public async Task<IActionResult> Collection(string collection)
        {
            if (collection == null)
            {
                ViewData["ProductName"] = "Все товары";
                collection = "Все товары";
            }

            var result = await client.GetProductCollection(collection);

            if (result.Content == null || result.Content.Count() <= 0 || result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("Error");
            }

            ViewData["ProductName"] = collection;

            return PartialView("_Products", result.Content);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Catalog(string genre)
        {
            var result = await client.GetGenreProducts(genre);

            if (result.Content == null || result.Content.Count() <= 0 || result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("Error");
            }

            return View(result.Content);
        }

        [Authorize]
        public async Task<IActionResult> AddToCart(int ProductId)
		{
            var token = httpContextAccessor.HttpContext.Request.Cookies["Authorization"];

			await client.AddToCart(token, ProductId);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
