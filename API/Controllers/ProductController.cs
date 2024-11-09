using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API1.Models;
using Microsoft.EntityFrameworkCore;

using Castle.Components.DictionaryAdapter.Xml;
using API1.Resources.Classes;
using System.Security.Claims;

using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;

namespace API1.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductController : ControllerBase
	{
		ClassContext _context = new ClassContext();

        [NonAction]
        public UserT GetCurrectUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaims = identity.Claims;

                return new UserT
                {
                    Login = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value,
                    NickName = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Name)?.Value,
                };
            }
            return null;
        }


        [HttpGet(template:"GetProduct")]
		public ActionResult GetProduct()
		{
            var GetproductDefualt = _context.ProductTs
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

			ConsoleHelper.PrintSuccessfull("Успешный вывод всех товаров");

            return Ok(GetproductDefualt);
        }

        [HttpGet(template: "GetProductSearth")]
        public ActionResult GetProductSearth(string NameProduct)
        {
            var GetproductDefualt = _context.ProductTs
                        .Select(x => new
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Description = x.Description,
                            Price = x.Price,
                            NameShop = x.UserProductTs.FirstOrDefault().UserLogin,
                            Image = x.Image,
                            genre = x.ProductGenreTs.Select(x => x.Genre.GenreName)
                        }).Where(x => x.Name.Contains(NameProduct));

            ConsoleHelper.PrintSuccessfull("Успешный вывод товаров по имени");

            return Ok(GetproductDefualt);
        }

        [HttpGet(template: "GetProductCollection")]
        public ActionResult GetProductCollection(string collection)
        {
            if (collection != null)
            {
                switch (collection)
                {
                    case "Все товары":
                        var GetproductAll = _context.ProductTs
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

                        ConsoleHelper.PrintSuccessfull("Все товары");

                        return Ok(GetproductAll);

                    case "Новинки":
                        var GetproductNovelty = _context.ProductTs
                            .Where(x => x.DateAdded > DateOnly.FromDateTime(DateTime.Now.AddDays(-7)))
                           .Select(x => new
                           {
                               Id = x.Id,
                               Name = x.Name,
                               Description = x.Description,
                               Price = x.Price,
                               NameShop = x.UserProductTs.FirstOrDefault().UserLogin,
                               Image = x.Image,
                               genre = x.ProductGenreTs.Select(x => x.Genre.GenreName),
                           });

                        ConsoleHelper.PrintSuccessfull("Новинки");
                        return Ok(GetproductNovelty);

                    case "Лидеры продаж":
                        var GetproductLeader = _context.ProductTs
                            .Select(x => new
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Description = x.Description,
                                Price = x.Price,
                                NameShop = x.UserProductTs.FirstOrDefault().UserLogin,
                                Image = x.Image,
                                genre = x.ProductGenreTs.Select(x => x.Genre.GenreName),
                                CountSeller = x.ProductKeyTs.Count(x => x.Key.StatusId == 1)
                            }).OrderByDescending(x => x.CountSeller);

                        ConsoleHelper.PrintSuccessfull("Лидеры продаж");
                        return Ok(GetproductLeader);

                    case "Популярное":

                        var GetproductPopular = _context.ProductTs
                            .Select(x => new
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Description = x.Description,
                                Price = x.Price,
                                NameShop = x.UserProductTs.FirstOrDefault().UserLogin,
                                Image = x.Image,
                                genre = x.ProductGenreTs.Select(x => x.Genre.GenreName),
                                CountPreviews = x.ProductPreviewTs.Count()
                            }).OrderByDescending(x => x.CountPreviews);

                        ConsoleHelper.PrintSuccessfull("Популярное");
                        return Ok(GetproductPopular);
                }
            }

            var GetproductDefualt = _context.ProductTs
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

            ConsoleHelper.PrintSuccessfull("Все товары");

            return Ok(GetproductDefualt);

        }

       [HttpGet(template:"GetGenreProducts")]
        public ActionResult GetGenreProducts(string genre)
        {
            var Getproduct = _context.ProductTs
                .Where(x => x.ProductGenreTs.Any(x => x.Genre.GenreName == genre))
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Price = x.Price,
                    NameShop = x.UserProductTs.FirstOrDefault().UserLogin,
                    Image = x.Image
                });

            ConsoleHelper.PrintSuccessfull("Успешный вывод товаров c жанром: " + genre);

            return Ok(Getproduct);
        }

        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
		[HttpGet(template: "GetProductInfo")]
		public ActionResult GetProductInfo(int Id)
		{
            bool ReviewAvailable = false;

            UserT userT = new UserT();
            userT = GetCurrectUser();

            if (userT != null)
            {
                ReviewAvailable = _context.UserPurchaseTs.Any(x => x.ProductKey.ProductId == Id);
            }

            var resultCountProduct = _context.ProductKeyTs
                .Where(x => x.ProductId == Id)
                .GroupBy(x => x.Key.StatusId)
                .Select(g => new
                {
                    StatusId = g.Key,
                    Count = g.Count()
                })
                .ToList();

            int CountInStock = resultCountProduct.Where(x => x.StatusId == 2).FirstOrDefault()?.Count ?? 0;
            int CountSales = resultCountProduct.Where(x => x.StatusId == 1).FirstOrDefault()?.Count ?? 0;

            ConsoleHelper.PrintSuccessfull("Просчёт данных для товара прошёл успешно - Успех");


            var getPreviewsRating = GetPreviewsRating(Id);

            int CountPositivePreviews = 0;
            int CountNegativePreviews = 0;
            double result = 0;

            if (getPreviewsRating != null)
            {
                CountPositivePreviews = getPreviewsRating.CountPositivePreviews;
                CountNegativePreviews = getPreviewsRating.CountNegativePreviews;
                result = getPreviewsRating.Result;
            }

            var getProductInfo = _context.ProductTs.Select(x => new
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                NameShop = x.UserProductTs.FirstOrDefault().UserLogin,
                Image = x.Image,
                Genre = x.ProductGenreTs.Select(x => x.Genre.GenreName),
                CountPositiveReviews = CountPositivePreviews,
                CountNegativeReviews = CountNegativePreviews,
                RatingProduct = result,
                CountSales = CountSales,
                CountInStock = CountInStock,
                ReviewAvailable = ReviewAvailable

            }).Where(x => x.Id == Id);


            var getPreviewsInfo = GetPreviewsContent(Id);

            if (getPreviewsInfo == null)
            {
                var Finalresult = new
                {
                    Productinfo = getProductInfo
                };

                ConsoleHelper.PrintSuccessfull("Получение данных о товаре прошло успешно (Без отзыва) - Успех");

                return Ok(Finalresult);
            }
            else
            {
                var Finalresult = new
                {
                    Productinfo = getProductInfo,
                    PreviewsInfo = getPreviewsInfo
                };

                ConsoleHelper.PrintSuccessfull("Получение данных о товаре прошло успешно - Успех");

                return Ok(Finalresult);
            }
        }

        public class PreviewsRatingResult
        {
            public double Result { get; set; }
            public int CountPositivePreviews { get; set; }
            public int CountNegativePreviews { get; set; }
        }

        [NonAction]
        public PreviewsRatingResult GetPreviewsRating(int Id)
        {
            var PreviewsRating = _context.ProductPreviewTs
                .Where(x => x.ProductId == Id)
                .GroupBy(x => x.Preview.PreviewRating)
                .Select(g => new
                {
                    PreviewRating = g.Key,
                    Count = g.Count()
                })
                .ToList();

            if (PreviewsRating.Count() == 0)
            {
                return null;
            }

            int CountPositivePreviews = PreviewsRating.Where(x => x.PreviewRating == 1).FirstOrDefault()?.Count ?? 0;
            int CountNegativePreviews = PreviewsRating.Where(x => x.PreviewRating == 0).FirstOrDefault()?.Count ?? 0;

            double One = CountPositivePreviews * 5 + CountNegativePreviews * 1;
            double Two = CountPositivePreviews + CountNegativePreviews;
            double result = One / Two;
            result = Math.Round(result * 10.0) / 10.0;

            return new PreviewsRatingResult
            {
                Result = result,
                CountPositivePreviews = CountPositivePreviews,
                CountNegativePreviews = CountNegativePreviews
            };
        }

        [AllowAnonymous]
        [HttpGet(template: "GetPreviewsProduct")]
        public object GetPreviewsContent(int Id)
        {
            var PreviewsContent = _context.ProductPreviewTs.Select(x => new
            {
                ProductId = x.ProductId,
                NickName = x.UserLoginNavigation.NickName,
                Content = x.Preview.PreviewContent,
                RatingContent = x.Preview.PreviewRating
            }).Where(x => x.ProductId == Id);


            if (PreviewsContent.Count() == 0)
            {
                return null;
            }

            ConsoleHelper.PrintSuccessfull(PreviewsContent.Count().ToString());

            return PreviewsContent;
        }
    }
}
