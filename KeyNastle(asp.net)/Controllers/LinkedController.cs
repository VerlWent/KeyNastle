using KeyNastle.Interface;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;

namespace KeyNastle.Controllers
{
    public class LinkedController : Controller
    {
        private readonly ILogger<LinkedController> logger;
        private readonly IUserClient client;
        private readonly IHttpContextAccessor httpContextAccessor;

        public LinkedController(ILogger<LinkedController> logger, IUserClient client, IHttpContextAccessor httpContextAccessor)
        {
            this.logger = logger;
            this.client = client;
            this.httpContextAccessor = httpContextAccessor;
        }
        string login;
        public async Task<IActionResult> SellerProfile(string login)
        {
            this.login = login;
            var result = await client.GetSellerInfo(login);

            return View(result.Content);
        }

        public async Task<IActionResult> GetProducts(int Id)
        {
            var result = await client.GetSellerInfo(login);

            return PartialView("_Products", result.Content);
        }
    }
}
