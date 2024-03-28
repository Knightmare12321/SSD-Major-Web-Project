using SSD_Major_Web_Project.Models;

namespace SSD_Major_Web_Project.ViewModels
{
    public class ApplyDiscountCodeVM
    {
        List<ShoppingCartItem> shoppingCartItems { get; set; }
        string couponCode { get; set; }
    }
}
