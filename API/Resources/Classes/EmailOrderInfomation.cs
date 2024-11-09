namespace API1.Resources.Classes
{
    public class EmailOrderInfomation
    {
        public string UserName { get; set; }
        public string DateNow { get; set; }
        public string PaymentMethod { get; set; }
        public string Price { get; set; }
        public string DeliveryMethod { get; set; }

        public List<EmailProductInformation> productInformation { get; set; }
    }
}
