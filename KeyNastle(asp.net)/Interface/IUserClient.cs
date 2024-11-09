using KeyNastle.Resources.Classes;
using KeyNastle.Resources.Classes.DataUserInfoFolder;
using KeyNastle.Resources.Classes.ProductInfoFolder;
using KeyNastle.Resources.Classes.SellerProfileInformation;
using Refit;

namespace KeyNastle.Interface
{
    public interface IUserClient
    {
        [Get("/Product/GetProduct")]
        Task<ApiResponse<List<Product>>> GetProduct();

        [Get("/Product/GetProductSearth")]
        Task<ApiResponse<List<Product>>> GetProductSearth(string NameProduct);

        [Get("/Product/GetProductCollection")]
        Task<ApiResponse<List<Product>>> GetProductCollection(string collection);

        [Get("/Product/GetGenreProducts")]
        Task<ApiResponse<List<Product>>> GetGenreProducts(string genre);

        [Get("/Product/GetPreviewsProduct")]
        Task<ApiResponse<List<PreviewsInfo>>> GetPreviewsProduct(int Id);

        [Get("/Product/GetProductInfo")]
        Task<ApiResponse<ProductAndPreviewData>> GetProductInfo(int Id);

        [Get("/HyperLink/GetSellerInfo")]
        Task<ApiResponse<CombinedSellerInfo>> GetSellerInfo(string Login);


        [Get("/Identification/Authorization")]
        Task<ApiResponse<UserTokenResponse>> GetUser(string Login, string Password);

        [Post("/Identification/Registrations")]
        Task<ApiResponse<HttpResponseMessage>> RegisterUser(UserInfo model);

        [Post("/User/AddToCart")]
        Task<ApiResponse<HttpResponseMessage>> AddToCart([Header("Authorization")] string token, int ProductId);

        [Get("/User/GetCart")]
        Task<ApiResponse<List<UserBusket>>> GetCart([Header("Authorization")] string token);

        [Post("/User/BuyFullBusket")]
        Task<ApiResponse<HttpResponseMessage>> BuyFullBusket([Header("Authorization")] string token);

        [Get("/User/GetHistoryPurchase")]
        Task<ApiResponse<List<UserPurchase>>> GetUserPurchases([Header("Authorization")] string token);

        [Get("/User/GetDataUser")]
        Task<ApiResponse<UserAndPreviewData>> GetDataUser([Header("Authorization")] string token);

        [Put("/User/ChangeData")]
        Task<ApiResponse<HttpResponseMessage>> ChangeData([Header("Authorization")] string token, string NickName, string OldPassword, string NewPassword);

        

        [Post("/User/AddPreview")]
        Task<ApiResponse<HttpResponseMessage>> AddPreview([Header("Authorization")] string token, int Rating, string TextPreview, int ProductId);

        [Delete("/User/DeleteFromBusket")]
        Task<ApiResponse<HttpResponseMessage>> DeleteFromBusket([Header("Authorization")] string token, int ProductId);

        //[Get("/Product/GetPreviewsProduct")]
        //Task<ApiResponse<List<PreviewsInfo>>> GetPreviewsProduct(int Id);

    }
}
