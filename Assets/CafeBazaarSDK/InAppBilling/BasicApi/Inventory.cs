using System.Collections.Generic;

namespace CafeBazaar.Billing
{
    public class Inventory
    {
        public List<Purchase> Purchases { get; set; }
        public List<Product> Products { get; set; }

    }
}