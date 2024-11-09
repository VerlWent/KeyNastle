namespace KeyNastle.Resources.Classes
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string Image { get; set; } = null!;

        public decimal Price { get; set; }
        public string NameShop { get; set; } = null!;
    }
}
