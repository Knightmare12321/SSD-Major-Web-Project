using System.ComponentModel.DataAnnotations;

namespace SSD_Major_Web_Project.ViewModels
{
    public class OrderConfirmationVM
    {
        // from paypal
        public string?TransactionId { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal? Amount { get; set; }
        public string PayerName { get; set; }

        // from checkout View Model
        public CheckoutVM CheckoutVM { get; set; }

    }
}
