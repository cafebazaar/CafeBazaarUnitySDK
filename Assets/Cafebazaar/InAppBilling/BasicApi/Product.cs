using CafeBazaar.Core;
using CafeBazaar.Core.SimpleJSON;
using System;

namespace CafeBazaar.Billing
{
    [Serializable()]
    public class Product
    {
        public string ProductId;
        public string Price;
        public string Title;
        public string Description;
        public ProductType Type;
        public ProductKind Kind;
        public static Product Parse(string json)
        {
            return Parse(JSONNode.Parse(json));
        }
        public static Product Parse(JSONNode json)
        {
            Product product = new Product();

            product.Title = json["title"].Value;
            product.Price = json["price"].Value;

            switch (json["type"].Value)
            {
                case "inapp":
                    product.Type = ProductType.InApp;
                    break;
                case "subs":
                    product.Type = ProductType.Subs;
                    break;
            }

            product.Description = json["description"].Value;
            product.ProductId = json["productId"].Value;

            return product;
        }
    }


}