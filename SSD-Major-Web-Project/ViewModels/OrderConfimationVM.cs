using System.ComponentModel.DataAnnotations;

namespace SSD_Major_Web_Project.ViewModels
{
    public class OrderConfirmationVM
    {
        public string TransactionId { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal? Amount { get; set; }
        public string PayerName { get; set; }

    }
}
