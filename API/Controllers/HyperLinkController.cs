using API1.Models;
using API1.Resources.Classes;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Tracing;

namespace API1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HyperLinkController : Controller
    {
        ClassContext _context = new ClassContext();

        [HttpGet(template: "GetSellerInfo")]
        public IActionResult GetSellerInfo(string Login) 
        {

            var GetInformationSeller = new
            {
                informationseller = GetInformation(Login),
                productlist = GetProductsList(Login),
                previewslist = GetPreviewsList(Login)
            };
            return Ok(GetInformationSeller);
        }

        //[NonAction]
        //public void GetInformation(string Login)
        //{
        //    int[] ProductsId = _context.UserProductTs.Where(x => x.UserLogin == Login).Select(x => x.ProductId).ToArray();

        //    int CountSelles = 0;
        //    int CountPositivePreviews = 0;
        //    int CountNegativePreviews = 0;
        //    int CountPreviews = 0;


        //    foreach (var item in ProductsId)
        //    {
        //        CountSelles += _context.ProductKeyTs.Where(x => x.ProductId == item && x.Key.StatusId == 1).Count();
        //    }

        //    ConsoleHelper.PrintSuccessfull("Кол-во продаж: " + CountSelles.ToString());
        //    ConsoleHelper.PrintSuccessfull("Кол-во товаров: " + ProductsId.Count().ToString());


        //    foreach (var item in ProductsId)
        //    {
        //        var PreviewsRating = _context.ProductPreviewTs
        //            .Where(x => x.ProductId == item)
        //            .GroupBy(x => x.Preview.PreviewRating)
        //            .Select(g => new
        //            {
        //                PreviewRating = g.Key,
        //                Count = g.Count()
        //            })
        //            .ToList();

        //        CountPositivePreviews += PreviewsRating.Where(x => x.PreviewRating == 1).FirstOrDefault()?.Count ?? 0;
        //        CountNegativePreviews += PreviewsRating.Where(x => x.PreviewRating == 0).FirstOrDefault()?.Count ?? 0;
        //    }

        //    double One = CountPositivePreviews * 5 + CountNegativePreviews * 1;
        //    double Two = CountPositivePreviews + CountNegativePreviews;
        //    double result = One / Two;
        //    result = Math.Round(result * 10.0) / 10.0;

        //    CountPreviews = CountPositivePreviews + CountNegativePreviews;

        //    ConsoleHelper.PrintSuccessfull("Рейтинг: " + result.ToString());
        //    ConsoleHelper.PrintSuccessfull("Позитив: " + CountPositivePreviews.ToString());
        //    ConsoleHelper.PrintSuccessfull("Негитив: " + CountNegativePreviews.ToString());
        //    ConsoleHelper.PrintSuccessfull("Кол-во отзывов: " + CountPreviews.ToString());
        //}


        public class Information
        {
            public string Login { get; set; }
            public int countSelles { get; set; }
            public int countProduct { get;set; }
            public double Rating { get; set; }
            public int countPreviews { get; set; }
        }

        [NonAction]
        public Information GetInformation(string Login)
        {
            var products = _context.UserProductTs
                            .Where(x => x.UserLogin == Login)
                            .Select(x => new { x.ProductId, x.Product });

            var productIds = products.Select(x => x.ProductId).ToArray();

            var countSelles = products
                               .Join(_context.ProductKeyTs, p => p.ProductId, pk => pk.ProductId, (p, pk) => pk)
                               .Where(pk => pk.Key.StatusId == 1)
                               .Count();

            var previews = products
                           .Join(_context.ProductPreviewTs, p => p.ProductId, pp => pp.ProductId, (p, pp) => pp)
                           .GroupBy(pp => pp.Preview.PreviewRating)
                           .Select(g => new { PreviewRating = g.Key, Count = g.Count() })
                           .ToList();

            var countPositivePreviews = previews.Where(x => x.PreviewRating == 1).Sum(x => x.Count);
            var countNegativePreviews = previews.Where(x => x.PreviewRating == 0).Sum(x => x.Count);
            var countPreviews = countPositivePreviews + countNegativePreviews;

            var result = (countPositivePreviews * 5 + countNegativePreviews * 1) / (double)countPreviews;
            result = Math.Round(result * 10.0) / 10.0;

            ConsoleHelper.PrintSuccessfull("Кол-во продаж: " + countSelles.ToString());
            ConsoleHelper.PrintSuccessfull("Кол-во товаров: " + productIds.Count().ToString());
            ConsoleHelper.PrintSuccessfull("Рейтинг: " + result.ToString());
            ConsoleHelper.PrintSuccessfull("Кол-во отзывов: " + countPreviews.ToString());

            return new Information
            {
                Login = Login,
                countSelles = countSelles,
                countProduct = productIds.Count(),
                Rating = result,
                countPreviews = countPreviews
            };
        }

        [NonAction]
        public object GetProductsList(string Login)
        {
            var GetProducts = _context.ProductTs
                .Where(x => x.UserProductTs.Any(x => x.UserLogin == Login))
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Price = x.Price,
                    NameShop = x.UserProductTs.FirstOrDefault().UserLogin,
                    Image = x.Image,
                    genre = x.ProductGenreTs.Select(x => x.Genre.GenreName)
                });

            return GetProducts;
        }

        [NonAction]
        public object GetPreviewsList(string Login)
        {
            int[] ProductsId = _context.UserProductTs.Where(x => x.UserLogin == Login).Select(x => x.Id).ToArray();

            var PreviewsContent = _context.ProductPreviewTs
                   .Where(x => ProductsId.Contains(x.ProductId))
                   .Select(x => new
                   {
                       ProductId = x.ProductId,
                       ProductName = x.Product.Name,
                       NickName = x.UserLoginNavigation.NickName,
                       Content = x.Preview.PreviewContent,
                       RatingContent = x.Preview.PreviewRating
                   })
                   .ToList();

            return PreviewsContent;
        }
    }
}
