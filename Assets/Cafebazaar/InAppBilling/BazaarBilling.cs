using CafeBazaar.Core;
using System;
using System.Collections.Generic;
namespace CafeBazaar.Billing
{
    public class BazaarBilling
    {
        public static bool IsInited { get { return CafeBazaarManager.Instacne.IAB_IsInited; } }

        public static bool SubscriptionsSupported
        {
            get { return CafeBazaarManager.Instacne.IAB_SubscriptionsSupported(); }
        }
        /// <summary>
        /// Init with Config
        /// </summary>
        /// <param name="OnResult"></param>
        public static void Init(Action<BazaarResponse> OnResult)
        {
            CafeBazaarManager.Instacne.IAB_Init((result) =>
            {
                if (result.Status == CoreInitStatus.Success)
                {
                    CafeBazaarManager.Instacne.IAB_QueryPurchases(
                        (res) =>
                        {
                            if (res.Status == GetPurchasesStatus.Success)
                            {
                                if(OnResult != null)
                                    OnResult(BazaarResponse<string>.Success("Success"));
                            }
                            else
                            {
                                if (OnResult != null)
                                    OnResult(BazaarResponse<string>.Error(res.Message));
                            }
                        }
                        );        
                }
                else
                {
                    if (OnResult != null)
                        OnResult(BazaarResponse<string>.Error(result.Message));
                }
            });
        }
        /// <summary>
        /// Custom init with RSA
        /// </summary>
        /// <param name="RSA"></param>
        /// <param name="OnResult"></param>
        public static void Init(string RSA, Action<BazaarResponse> OnResult)
        {
            CafeBazaarManager.Instacne.IAB_Init(RSA, (result) =>
            {
                if (result.Status == CoreInitStatus.Success)
                {
                    CafeBazaarManager.Instacne.IAB_QueryPurchases(
                        (res) =>
                        {
                            if (res.Status == GetPurchasesStatus.Success)
                            {
                                if (OnResult != null)
                                    OnResult(BazaarResponse<string>.Success("Success"));
                            }
                            else
                            {
                                if (OnResult != null)
                                    OnResult(BazaarResponse<string>.Error(res.Message));
                            }
                        }
                        );
                }
                else
                {
                    if (OnResult != null)
                        OnResult(BazaarResponse<string>.Error(result.Message));
                }
            });
        }
        public static void Subscribe(string sku, Action<BazaarResponse<Purchase>> OnResult)
        {
            Subscribe(sku, "", OnResult);
        }
        public static void Subscribe(string sku, string DeveloperPayload, Action<BazaarResponse<Purchase>> OnResult)
        {
            CafeBazaarManager.Instacne.IAB_PurchaseProduct(sku, DeveloperPayload, "subs", 
                (result)=> 
                {
                    if (OnResult != null)
                    {
                        if (result.Status == PurchaseStatus.Success)
                            OnResult(BazaarResponse<Purchase>.Success(result.Purchase));
                        else
                            OnResult(BazaarResponse<Purchase>.Error(result.Message));
                    }
                });
        }
        public static void Purchase(string sku, Action<BazaarResponse<Purchase>> OnResult)
        {
            Purchase(sku, "", OnResult);
        }
        public static void Purchase(string sku, string DeveloperPayload, Action<BazaarResponse<Purchase>> OnResult)
        {
            CafeBazaarManager.Instacne.IAB_PurchaseProduct(sku, DeveloperPayload, "inapp", (result) =>
            {
                if (OnResult != null)
                {
                    if (result.Status == PurchaseStatus.Success)
                    {
                        OnResult(BazaarResponse<Purchase>.Success(result.Purchase));
                    }
                    else
                    {
                        OnResult(BazaarResponse<Purchase>.Error(result.Message));
                    }
                }
            });
        }
        public static void Consume(string sku, Action<BazaarResponse<Purchase>> OnResult)
        {
#if UNITY_EDITOR
            if (OnResult != null)
                OnResult(BazaarResponse<Purchase>.Success(new Purchase() { ProductId = sku, State = PurchaseState.Purchased }));
#else
                CafeBazaarManager.Instacne.IAB_ConsumeProduct(sku, (result) =>
                {
                    if (OnResult != null)
                    {
                        if (result.Status == ConsumeStatus.Success)
                        {
                            OnResult(BazaarResponse<Purchase>.Success(result.Purchase));
                        }
                        else
                        {
                            OnResult(BazaarResponse<Purchase>.Error(result.Message));
                        }
                    }
                });
#endif
        }
        public static void GetInventory(string[] skus, Action<BazaarResponse<Inventory>> OnResult)
        {
            CafeBazaarManager.Instacne.IAB_QueryInventory(skus, (result) =>
            {
                if (OnResult != null)
                {
                    if (result.Status == GetInventoryStatus.Success)
                    {
                        OnResult(BazaarResponse<Inventory>.Success(new Inventory() { Products = result.Products, Purchases = result.Purchases }));
                    }
                    else
                    {
                        OnResult(BazaarResponse<Inventory>.Error(result.Message));
                    }
                }
            });
        }
        public static void GetSkuDetails(string[] skus, Action<BazaarResponse<List<Product>>> OnResult)
        {
            CafeBazaarManager.Instacne.IAB_QuerySkuDetails(skus, (result) =>
            {
                if (OnResult != null)
                {
                    if (result.Status == GetSkuDetailsStatus.Success)
                    {
                        OnResult(BazaarResponse<List<Product>>.Success(result.Products));
                    }
                    else
                    {
                        OnResult(BazaarResponse<List<Product>>.Error(result.Message));
                    }
                }
            });
        }
        public static void GetPurchases(Action<BazaarResponse<List<Purchase>>> OnResult)
        {
            CafeBazaarManager.Instacne.IAB_QueryPurchases((result) =>
            {
                if (OnResult != null)
                {
                    if (result.Status == GetPurchasesStatus.Success)
                    {
                        OnResult(BazaarResponse<List<Purchase>>.Success(result.Purchases));
                    }
                    else
                    {
                        OnResult(BazaarResponse<List<Purchase>>.Error(result.Message));
                    }
                }
            });
        }
    }
}