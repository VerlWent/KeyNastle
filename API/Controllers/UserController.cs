using Microsoft.AspNetCore.Mvc;
using API1.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.Net.Mime;
using System.Data;
using API1.Resources.Classes;
using Microsoft.VisualBasic;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace API1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

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

        [Authorize]
        [HttpPost(template:"AddPreview")]
        public IActionResult AddPreview(int Rating, string TextPreview, int ProductId) 
        {
            UserT userT = new UserT();
            userT = GetCurrectUser();

            ConsoleHelper.PrintSuccessfull("Рейтинг: " + Rating.ToString());
            ConsoleHelper.PrintSuccessfull("Содержание отзыва: " + TextPreview);
            ConsoleHelper.PrintSuccessfull("Код продукта: " + ProductId.ToString());

            int MaxIdPreview = _context.PreviewTs.Max(x => x.Id) + 1;

            PreviewT NewPreview = new PreviewT()
            {
                Id = MaxIdPreview,
                PreviewContent = TextPreview,
                PreviewRating = Rating
            };

            ProductPreviewT NewProductPreview = new ProductPreviewT()
            {
                ProductId = ProductId,
                PreviewId = MaxIdPreview,
                UserLogin = userT.Login
            };

            _context.PreviewTs.Add(NewPreview);
            _context.ProductPreviewTs.Add(NewProductPreview);
            _context.SaveChanges();

            return Ok();
        }

        [Authorize]
        [HttpGet(template: "GetDataUser")]
        public IActionResult GetDataUser()
        {
            UserT userT = new UserT();
            userT = GetCurrectUser();

            var DataUser = _context.UserTs.Select(x => new
            {
                Login = x.Login,
                NickName = x.NickName,
                Email = x.Email,
                Password = x.Password,
                RegistrationDate = x.RegistrationDate,
                Salt = x.Salt,
                RoleId = x.RoleId,
                CountBuyProduct = x.UserPurchaseTs.Count,
                CountPreview = x.ProductPreviewTs.Count
            }).Where(x => x.Login == userT.Login).FirstOrDefault();

            var PreviewUser = _context.ProductPreviewTs.Select(x => new
            {
                Login = x.UserLogin,
                ProductName = x.Product.Name,
                Content = x.Preview.PreviewContent,
                Rating = x.Preview.PreviewRating
            }).Where(x => x.Login == userT.Login);

            var finalresult = new 
            {
                UserData = DataUser,
                PreviewUser = PreviewUser
            };

            if (DataUser != null && PreviewUser != null)
            {
                ConsoleHelper.PrintSuccessfull("Получен пользователь. Пользователь: " + DataUser.Login + " - Успешно.");
                return Ok(finalresult);
            }
            else
            {
                ConsoleHelper.PrintFail("Ошибка получение пользователя - Неудача");
                return NotFound();
            }
        }

        [Authorize]
        [HttpGet(template: "GetHistoryPurchase")]
        public IActionResult GetHistoryPurchase()
        {
            UserT userT = new UserT();
            userT = GetCurrectUser();

            var GetPurchase = _context.UserPurchaseTs.Select(x => new
            {
                UserLogin = x.UserLogin,
                ProductImage = x.ProductKey.Product.Image,
                ProductName = x.ProductKey.Product.Name,
                ProductPrice = x.ProductKey.Product.Price,
                DatePurchase = x.DatePurchase
            }).Where(x => x.UserLogin == userT.Login);

            if (GetPurchase != null)
            {
                ConsoleHelper.PrintSuccessfull("История пользователя получена. Пользователь: " + userT.Login + " - Успешно");
                return Ok(GetPurchase);
            }
            else
            {
                ConsoleHelper.PrintFail("История пользователя не найдена. Пользователь: " + userT.Login + " - Неудача");
                return NotFound();
            }
        }

        [Authorize]
        [HttpPut(template: "ChangeData")]
        public IActionResult ChangeData(string? NickName, string? OldPassword, string? NewPassword)
        {
            UserT userT = new UserT();
            userT = GetCurrectUser();

            UserT getuser = _context.UserTs.FirstOrDefault(x => x.Login == userT.Login);

            if (NickName != null || NickName != "" || getuser.NickName != NickName)
            {
                getuser.NickName = NickName;
            }
            if (OldPassword != null && OldPassword != "" && NewPassword != null && NewPassword != "")
            {
                string OldHashedPassword = BCrypt.Net.BCrypt.HashPassword(OldPassword, getuser.Salt);

                if (getuser.Password != OldHashedPassword)
                {
                    ConsoleHelper.PrintFail("Пароль не совпадает " + " - Неудача");
                    return Conflict();
                }

                string salt = BCrypt.Net.BCrypt.GenerateSalt();

                string NewhashedPassword = BCrypt.Net.BCrypt.HashPassword(NewPassword, salt);

                getuser.Password = NewhashedPassword;
                getuser.Salt = salt;

                ConsoleHelper.PrintSuccessfull("Пароль изменён " + " - Успешно");
            }

            ConsoleHelper.PrintSuccessfull("Имя пользователя: " + getuser.NickName);
            ConsoleHelper.PrintSuccessfull("Старый пароль: " + OldPassword);
            ConsoleHelper.PrintSuccessfull("Новый пароль: " + NewPassword);

            _context.Update(getuser);
            _context.SaveChanges();

            return Ok();
        }

        [Authorize]
        [HttpPost(template: "AddToCart")]
        public ActionResult AddToCart(int ProductId)
        {
            UserT userT = new UserT();
            userT = GetCurrectUser();

            if (_context.UserBusketTs.FirstOrDefault(x => x.ProductId == ProductId && userT.Login == x.UserLogin) != null)
            {
                ConsoleHelper.PrintFail("Данный товар уже имеется в корзине - Неудача");
                return Conflict();
            }

            UserBusketT newBusket = new UserBusketT();

            if (_context.UserBusketTs.Count() == 0)
            {
                newBusket.Id = 1;
            }
            else
            {
                newBusket.Id = _context.UserBusketTs.Max(x => x.Id) + 1;
            }

            newBusket.ProductId = ProductId;
            newBusket.UserLogin = userT.Login;

            try
            {
                _context.Add(newBusket);
                _context.SaveChanges();

                ConsoleHelper.PrintSuccessfull("Добавление в корзину - успешно");
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintFail("Ошибка добавление в корзину: " + ex.Message.ToString() + " - Неудача");
            }
        
            return Ok();
        }

        [Authorize]
        [HttpGet(template: "GetCart")]
        public ActionResult GetCart()
        {
            UserT userT = new UserT();
            userT = GetCurrectUser();

            List<UserBusket> userbusket = new List<UserBusket>();

            var ModelCart = _context.UserBusketTs.Select(x => new
            {
                x.Id,
                ProductId = x.Product.Id,
                x.UserLogin,
                ProductName = x.Product.Name,
                Price = x.Product.Price,
                ProductImage = x.Product.Image,
                NameShop = x.Product.UserProductTs.FirstOrDefault().UserLogin
            }).Where(x => x.UserLogin == userT.Login).OrderBy(x => x.Id);

            foreach (var model in ModelCart)
            {
                var CheckStock = ClassContext._context.ProductKeyTs.FirstOrDefault(x => x.ProductId == model.ProductId && x.Key.StatusId == 2);

                if (CheckStock != null)
                {
                    userbusket.Add(new UserBusket()
                    {
                        Id = model.Id,
                        ProductId = model.ProductId,
                        UserLogin = model.UserLogin,
                        ProductName = model.ProductName,
                        Price = model.Price,
                        ProductImage = model.ProductImage,
                        NameShop = model.NameShop,
                        IsStock = true
                    });
                }
                else
                {
                    userbusket.Add(new UserBusket()
                    {
                        Id = model.Id,
                        ProductId = model.ProductId,
                        UserLogin = model.UserLogin,
                        ProductName = model.ProductName,
                        Price = model.Price,
                        ProductImage = model.ProductImage,
                        NameShop = model.NameShop,
                        IsStock = false
                    });
                }
            }

            if (ModelCart == null)
            {
                ConsoleHelper.PrintFail("Корзина не найдена " + " - Неудача");
                return NotFound();
            }
            else
            {
                ConsoleHelper.PrintSuccessfull("Корзина успешно загружена. Пользователь: " + userT.Login.ToString() + " - Успешно");
                return Ok(userbusket);
            }
        }

        [Authorize]
        [HttpDelete(template:"DeleteFromBusket")]
        public ActionResult DeleteFromBusket(int ProductId)
        {
            UserT userT = new UserT();
            userT = GetCurrectUser();

            var GetBusketProduct = _context.UserBusketTs.FirstOrDefault(x => x.ProductId == ProductId && x.UserLogin == userT.Login);
            _context.UserBusketTs.Remove(GetBusketProduct);
            _context.SaveChanges();

            return Ok();
        }

        [Authorize]
        [HttpPost(template: "BuyFullBusket")]
        public ActionResult BuyFullBusket()
        {
            UserT userT = new UserT();
            userT = GetCurrectUser();

            int[] IdProductlist;
            int MaxIdUserPurchase;

            IdProductlist = _context.UserBusketTs.Where(x => x.UserLogin == userT.Login).Select(x => x.ProductId).ToArray();

            ///

            

            ///

            List<KeyStorageT> keystorage = new List<KeyStorageT>();

            foreach (var item in IdProductlist)
            {
                var GetKeyStorage = _context.KeyStorageTs
                    .FirstOrDefault(x => x.ProductKeyTs.Any(x => x.ProductId == item) && x.StatusId == 2);

                if (GetKeyStorage == null)
                {
                    ConsoleHelper.PrintFail("Товар закончился - Неудача");
                    return NotFound();
                }

                List<UserPurchaseT> userpurchase = IdProductlist
                .Select(item => new UserPurchaseT
                {
                    UserLogin = userT.Login,
                    ProductKeyId = GetKeyStorage.Id,
                    DatePurchase = DateOnly.FromDateTime(DateTime.Now)
                })
                .ToList();

                _context.UserPurchaseTs.AddRange(userpurchase);

                keystorage.Add(GetKeyStorage);
            }

            ConsoleHelper.PrintSuccessfull(keystorage.Count.ToString() + " ::: " + IdProductlist.Count().ToString());

            if (keystorage.Count != IdProductlist.Count())
            {
                ConsoleHelper.PrintFail("Товар закончился - Неудача");
                return NotFound();
            }

            keystorage.ForEach(x => x.StatusId = 1);

            _context.KeyStorageTs.UpdateRange(keystorage);

            ///

            EmailOrderInfomation information = new EmailOrderInfomation();
            List<EmailProductInformation> productInformation = new List<EmailProductInformation>();
            decimal FullPrice = 0;

            foreach (var item in IdProductlist)
            {
                var GetProductInfo = _context.ProductKeyTs.Select(x => new
                {
                    x.ProductId,
                    KeyStatus = x.Key.StatusId,
                    ProductName = x.Product.Name,
                    ProductPrice = x.Product.Price,
                    ProductKey = x.Key.Content
                }).Where(x => x.ProductId == item && x.KeyStatus == 2).FirstOrDefault();

                
                productInformation.Add(new EmailProductInformation()
                {
                    ProductName = GetProductInfo.ProductName,
                    ProductKey = GetProductInfo.ProductKey,
                    ProductPrice = GetProductInfo.ProductPrice
                });

                FullPrice += GetProductInfo.ProductPrice;
            }

            information.UserName = userT.NickName;
            information.DateNow = DateOnly.FromDateTime(DateTime.Now).ToString();
            information.PaymentMethod = "Электронный платёж";
            information.Price = FullPrice.ToString();
            information.DeliveryMethod = "На почту";
            information.productInformation = productInformation;

            SendEmail(information);

            _context.SaveChanges();

            return Ok();
        }

        [Authorize]
        [HttpGet(template: "SendEmail")]
        public ActionResult SendEmail(EmailOrderInfomation Information)
        {
            string fromEmail = "wervix.salavat@mail.ru";
            string password = "UC5tQTS6qJeh8LKiReFV";

            string toEmail = "wervix.salavat@mail.ru";

            SmtpClient smtpClient = new SmtpClient("smtp.mail.ru")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true
            };

            try
            {
                smtpClient.Send(getMailWithImg(toEmail, Information));
                _logger.LogInformation("Отправленно");
                return Ok();
            }

            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest();
            }
        }

        private MailMessage getMailWithImg(string toEmail, EmailOrderInfomation Information)
        {
            MailMessage mail = new MailMessage();
            mail.IsBodyHtml = true;
            mail.AlternateViews.Add(getEmbeddedImage("C:\\Users\\VerlWent\\source\\repos\\API\\Resources\\Images\\Group 2.png", Information));
            mail.From = new MailAddress("wervix.salavat@mail.ru");
            mail.To.Add(toEmail);
            mail.Subject = "KeyNastle";
            return mail;
        }
        private AlternateView getEmbeddedImage(String filePath, EmailOrderInfomation Information)
        {
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string htmlBody = @"<html>
                                    <body>
                                        <img src='cid:" + res.ContentId + @"'/><br/>
                                        <p>Здравствуйте " + Information.UserName + @",</p>
                                        <p>Спасибо за вашу покупку на KeyNastle! Ваша покупка успешно завершена.</p>
                                        <p>Ниже приведены детали вашего заказа:</p>
                                        
                                            <p>Дата и время заказа: " + Information.DateNow + @"</p>
                                            <p>Способ оплаты: " + Information.PaymentMethod + @"</p>
                                            
                                            <p>Товар и ключ:</p> 
                                            <ul>";
                                                
                                                
                                                foreach (var item in Information.productInformation)
                                                {
                                                    htmlBody += $"<li>Товар: {item.ProductName}, Цена: {item.ProductPrice} ₽, Ключ: {item.ProductKey}</li>";
                                                }

                                                htmlBody += $@"</ul>
                                            <p>Сумма: " + Information.Price + @" ₽</p>
                                            <p>Способ доставки: " + Information.DeliveryMethod + @"</p>
                                        
                                        <p>Если у вас есть какие-либо вопросы или проблемы, пожалуйста, не стесняйтесь связаться с нашей службой поддержки. Мы рады помочь вам!</p>
                                        <p>С уважением, команда KeyNastle.</p>
                                    </body>
                                </html>";

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }
    }
}
