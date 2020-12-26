using System.Collections.Generic;
using System;
using CafeBazaar.Core;

using CafeBazaar.Billing;
using CafeBazaar.Core.SimpleJSON;
namespace CafeBazaar.Core
{
    public enum PurchaseStatus
    {
        Success,
        Canceled,
        Failed,
    }
    public class PurchaseResult : CafeBaseResult
    {
        public PurchaseStatus Status { get; set; }
        public bool TestMode { get; set; }
        public Purchase Purchase { get; set; }

    }
    public enum ConsumeStatus
    {
        Success,
        Failed,
    }
    public class ConsumeResult : CafeBaseResult
    {
        public ConsumeStatus Status { get; set; }
        public bool TestMode { get; set; }
        public Purchase Purchase { get; set; }

    }
    public class IABInitResult : CafeBaseResult
    {
        public CoreInitStatus Status { get; set; }
    }

    //public enum PurchaseState
    //{
    //    Success,
    //    UserCanceled,
    //    ItemUnavailable,
    //    DeveloperError,
    //    ItemAlreadyOwned,
    //    ItemNotOwned,
    //}
    public enum CoreSignInStatus
    {
        Success,
        Failed
    }
    public enum CoreStorageStatus
    {
        Success,
        Failed
    }
    public enum CoreInitStatus
    {
        Success,
        Failed,
    }

    public enum PurchaseState
    {
        Purchased = 0,
        Canceled = 1,
        Refunded = 2
    }





    public class SignInResult : CafeBaseResult
    {
        public string AccountId { get; set; }
        public CoreSignInStatus Status { get; set; }
    }
    public class InitStorageResult : CafeBaseResult
    {
        public InitStorageStatus Status { get; set; }
    }
    public enum InitStorageStatus
    {
        Success,
        Failed,
    }

    public class GetInventoryResult : CafeBaseResult
    {
        public GetInventoryStatus Status { get; set; }

        public List<Purchase> Purchases { get; set; }
        public List<Product> Products { get; set; }

    }

    public class SkuDetailsResult : CafeBaseResult
    {
        public GetSkuDetailsStatus Status { get; set; }
        public List<Product> Products { get; set; }
    }


    public class GetPurchasesResult : CafeBaseResult
    {
        public GetPurchasesStatus Status { get; set; }
        public List<Purchase> Purchases { get; set; }
    }
    public enum GetPurchasesStatus
    {
        Success,
        Failed,
    }
    public enum GetInventoryStatus
    {
        Success,
        Failed,
    }
    public enum GetSkuDetailsStatus
    {
        Success,
        Failed,
    }
}
