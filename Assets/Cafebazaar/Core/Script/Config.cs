using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CafeBazaar.Billing;
namespace CafeBazaar.Core
{
    [HelpURL("https://developers.cafebazaar.ir/fa/docs/")]
    public class Config : ScriptableObject
    {

        public bool ActiveSDK = true;
        public C_InAppPurchase InAppPurchase;

        //public string LoginClientId;

        [Serializable()]
        public class C_InAppPurchase
        {
            [TextArea(6, 10)]
            public string RSA;
            public List<Product> Products;
        }
    }
}
