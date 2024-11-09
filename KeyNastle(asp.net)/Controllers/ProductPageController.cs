using Humanizer;
using KeyNastle.Interface;
using KeyNastle.Resources.Classes.ProductInfoFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KeyNastle.Controllers
{
    public class ProductPageController : Controller
    {

        private readonly ILogger<ProductPageController> logger;
        private readonly IUserClient client;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ProductPageController(ILogger<ProductPageController> logger, IUserClient client, IHttpContextAccessor httpContextAccessor)
        {
            this.logger = logger;
            this.client = client;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Index(int Id)
        {
            var result = await client.GetProductInfo(Id);

            if (result.Content == null || result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("Error");
            }

            return View(result.Content);
        }

        [AllowAnonymous]
        public async void AddPreview(int Rating, string TextPreview, int ProductId)
        {
            var token = httpContextAccessor.HttpContext.Request.Cookies["Authorization"];
            
            await client.AddPreview(token, Rating, TextPreview, ProductId);
        }
    }
}
