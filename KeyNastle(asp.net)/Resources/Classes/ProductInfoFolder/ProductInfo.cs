namespace KeyNastle.Resources.Classes.ProductInfoFolder
{
    public class ProductInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string NameShop { get; set; }
        public string Image { get; set; }
        public List<string> Genre { get; set; }
        public int CountPositiveReviews { get; set; }
        public int CountNegativeReviews { get; set; }
        public double RatingProduct { get; set; }
        public int CountSales { get; set; }
        public int CountInStock { get; set; }
        public bool ReviewAvailable { get; set; }
    }
}
