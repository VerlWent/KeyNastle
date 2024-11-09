namespace API1.Resources.Classes
{
    public class UserBusket
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserLogin { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string ProductImage { get; set; }
        public string NameShop { get; set; }
        public bool IsStock { get; set; }
    }
}
