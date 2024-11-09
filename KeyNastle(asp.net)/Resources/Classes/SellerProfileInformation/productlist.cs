namespace KeyNastle.Resources.Classes.SellerProfileInformation
{
    public class productlist
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public decimal price { get; set; }
        public string nameShop { get; set; }
        public string image { get; set; }
        public List<string> genre { get; set; }
    }
}
