namespace SSD_Major_Web_Project.Models
{
    public class Cart
    {
        public List<Product> Items { get; set; }

        public Cart()
        {
            Items = new List<Product>();
        }

        public void AddItem(Product product)
        {
            Items.Add(product);
        }

        public void RemoveItem(Product product)
        {
            Items.Remove(product);
        }

        public decimal GetTotal()
        {
            return (decimal)Items.Sum(item => item.Price);
        }

    }
}
