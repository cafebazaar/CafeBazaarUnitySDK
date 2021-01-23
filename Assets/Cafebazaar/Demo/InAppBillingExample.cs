

using UnityEngine;
using CafeBazaar;
using UnityEngine.UI;
using CafeBazaar.Billing;
using System.Collections.Generic;

public class InAppBillingExample : MonoBehaviour
{
    public Text ConsoleText;

    public Button Btn_InitIAB, Btn_StartPurchase, Btn_ConsumePurchase, Btn_subscribe,Btn_purchaseList;

    void Start()
    {
        Log("CafeBazaar Plugin Version: " + PluginVersion.VersionString);
        RefreshButtonEnableStatus();
    }

    private void RefreshButtonEnableStatus()
    {

        Btn_InitIAB.interactable = !BazaarBilling.IsInited;
        Btn_StartPurchase.interactable = BazaarBilling.IsInited;
        Btn_ConsumePurchase.interactable = BazaarBilling.IsInited;
        Btn_subscribe.interactable = BazaarBilling.IsInited;
        Btn_purchaseList.interactable = BazaarBilling.IsInited;
    }

    public void InitIAB()
    {
        Btn_InitIAB.interactable = false;
        Log("Initializing IAB ... ");
        BazaarBilling.Init((result) =>
        {
            if (result.Successful)
            {
                Log("BazaarBilling is inited.");
            }
            else
            {
                Log("BazaarBilling Can't init !");
            }

            RefreshButtonEnableStatus();
        });
    }

    public void Gem1Purchase()
    {
        Btn_StartPurchase.interactable = false;

        string sku = "test";
        Log("Purchasing ... " + sku);

        BazaarBilling.Purchase(sku, "Test",
            (result) =>
            {
                if (result.Successful)
                {
                    Purchase purchase = result.Body;

                    Log("Purchase info :");
                    Log("   ProductId         : " + purchase.ProductId);
                    Log("   OrderId           : " + purchase.OrderId);
                    Log("   PurchaseToken     : " + purchase.PurchaseToken);
                    Log("   PurchaseTime      : " + purchase.PurchaseTime);
                    Log("   DeveloperPayload  : " + purchase.DeveloperPayload);
                    Log("   State             : " + purchase.State.ToString());
                    Log("   ProductType       : " + purchase.ProductType.ToString());
                }
                else
                {
                    Log("Purchase Failed :" + result.Message);
                }

                RefreshButtonEnableStatus();
            });
    }
    public void Gem1Consume()
    {
        Btn_ConsumePurchase.interactable = false;
        string sku = "test";
        Log("Consuming ... " + sku);
        BazaarBilling.Consume(sku,
            (result) =>
            {
                if (result.Successful)
                    Log("Consume " + sku + " Successful");
                else
                    Log("Consume Failed : " + result.Message);

                RefreshButtonEnableStatus();
            });
    }
    public void Suscribe()
    {
        Btn_subscribe.interactable = false;

        string sku = "substest";
        Log("Subscirbing ... " + sku);
        BazaarBilling.Subscribe(sku, "Test",
            (result) =>
            {

                if (result.Successful)
                {
                    Purchase purchase = result.Body;
                    Log("Suscribe purchase info :");
                    Log("   ProductId         : " + purchase.ProductId);
                    Log("   OrderId           : " + purchase.OrderId);
                    Log("   PurchaseToken     : " + purchase.PurchaseToken);
                    Log("   PurchaseTime      : " + purchase.PurchaseTime);
                    Log("   DeveloperPayload  : " + purchase.DeveloperPayload);
                    Log("   State             : " + purchase.State.ToString());
                    Log("   ProductType       : " + purchase.ProductType.ToString());
                }
                else
                {
                    Log("Suscribe Failed :" + result.Message);
                }

                RefreshButtonEnableStatus();
            });

    }

    public void GetPurchases()
    {
        BazaarBilling.GetPurchases(
        (result) =>
        {
            if (result.Successful)
            {
                int i = 0;
                List<Purchase> purchases = result.Body;
                Log("purchase List : "+purchases.Count);

                foreach (Purchase p in purchases)
                {
                    i++; 
                    Log(i+") ProductId: " + p.ProductId);
                    Log("   OrderId           : " + p.OrderId);
                    Log("   PurchaseToken     : " + p.PurchaseToken);
                    Log("   PurchaseTime      : " + p.PurchaseTime);
                    Log("   DeveloperPayload  : " + p.DeveloperPayload);
                    Log("   State             : " + p.State.ToString());
                    Log("   ProductType       : " + p.ProductType.ToString());
                }
            }
            else
            {
                Log("Failed :" + result.Message);
            }
        });
    }
    public void GetSkuDetails()
    {
        BazaarBilling.GetSkuDetails(new string[] { "Gem1", "Gem2" },
        (result) =>
        {
            if (result.Successful)
            {
                List<Product> products = result.Body;


            }
            else
            {
                Log("Failed :" + result.Message);
            }
        });
    }

    public void Log(string message)
    {
        ConsoleText.text += message + "\n";
    }
}
