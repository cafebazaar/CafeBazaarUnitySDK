using CafeBazaar.Billing;
using CafeBazaar.Core;
using CafeBazaar.Core.SimpleJSON;
using System;

namespace CafeBazaar.Billing
{
    public class Purchase
    {
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public DateTime PurchaseTime { get; set; }
        public PurchaseState State { get; set; }
        public ProductType ProductType { get; set; }
        public string DeveloperPayload { get; set; }
        public string PurchaseToken { get; set; }
        public string Signature { get; set; }
        public bool AutoRenewing { get; set; }

        public static Purchase Parse(string json)
        {
            return Parse(JSONNode.Parse(json));
        }
        public static Purchase Parse(JSONNode json)
        {
            Purchase purchase = new Purchase();

            purchase.OrderId = json["orderId"].Value;
            purchase.ProductId = json["productId"].Value;
            purchase.DeveloperPayload = json["developerPayload"].Value;
            purchase.PurchaseTime = CafeBazaarUtil.ConvertFromUnixTimestamp(long.Parse(json["purchaseTime"].Value));
            purchase.State = (PurchaseState)int.Parse(json["purchaseState"].Value);
            purchase.PurchaseToken = json["purchaseToken"].Value;
            purchase.Signature = json["signature"].Value;
            UnityEngine.Debug.Log(json.ToString());
            switch (json["itemType"].Value)
            {
                case "inapp":
                    purchase.ProductType = ProductType.InApp;
                    break;
                case "subs":
                    purchase.ProductType = ProductType.Subs;
                    break;
            }

            return purchase;
        }
    }
}