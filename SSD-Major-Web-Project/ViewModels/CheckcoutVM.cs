namespace SSD_Major_Web_Project.ViewModels
{
    public class CheckoutVM
    {
        public OrderVM Order { get; set; }
        public ShoppingCartVM ShoppingCart { get; set; }
        public string currency { get; set; }
        public string DeliveryContactEmail { get; set; }

    }
}
