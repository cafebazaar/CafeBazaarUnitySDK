#if UNITY_EDITOR 
using CafeBazaar.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CafeBazaar.Billing;
//using UnityEditor.Android;
namespace CafeBazaar.Core.UI
{

    public class CafebazaarSandboxUI : MonoBehaviour
    {
        #region Singleton
        private static CafebazaarSandboxUI pointer;
        private EventSystem eventSystem;
        public static CafebazaarSandboxUI Instacne
        {
            get
            {
                if (pointer == null)
                    pointer = FindObjectOfType<CafebazaarSandboxUI>();

                if (pointer == null)
                {
                    GameObject cafebazaarObject = Instantiate(Resources.Load<GameObject>("Cafebazaar/SandboxUI"), new Vector3(3000, 3000, 0), Quaternion.identity);
                    cafebazaarObject.name = "_CAFE_SANDBOX_UI_";
                    //cafebazaarObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInHierarchy;
                    pointer = cafebazaarObject.GetComponent<CafebazaarSandboxUI>();

                    SetLayerRecursively(pointer.gameObject, 31);
                }

                if (pointer.eventSystem == null)
                    pointer.eventSystem = FindObjectOfType<EventSystem>();

                if (pointer.eventSystem == null)
                {
                    GameObject eventSystemObject = Instantiate(Resources.Load<GameObject>("Cafebazaar/EventSystem"));
                    eventSystemObject.name = "EventSystem";
                    eventSystemObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInHierarchy;
                    pointer.eventSystem = eventSystemObject.GetComponent<EventSystem>();
                }

                return pointer;
            }
        }
        #endregion
        private string SKU;
        private string DeveloperPayload;
        private Action<PurchaseResult> OnResult;

        private Animator Animator;
        public GameObject purchaseStartPage, purchaseResultPage;
        public GameObject checkObject;
        public Text productIdText;
        public Text purchaseResultText;
        public void Awake()
        {
            Animator = GetComponent<Animator>();
        }
        public static void StartPurchase(string sku, string DeveloperPayload, Action<PurchaseResult> OnResult)
        {
            Instacne.checkObject.SetActive(false);
            Instacne.purchaseStartPage.SetActive(true);
            Instacne.purchaseResultPage.SetActive(false);

            Instacne.productIdText.text =  sku;
            Instacne.SKU = sku;
            Instacne.DeveloperPayload = DeveloperPayload;
            Instacne.OnResult = OnResult;
            Instacne.Animator.SetTrigger("Show");
        }
        public void Buy()
        {
            StartCoroutine(IEBuy());
        }
        public void Cancel()
        {
            StartCoroutine(IECancel());
        }
        private IEnumerator IEBuy()
        {
            purchaseResultText.text = "خرید با موفقیت انجام شد".ToFarsi();
            purchaseStartPage.SetActive(false);
            purchaseResultPage.SetActive(true);
            Instacne.checkObject.SetActive(true);
            yield return new WaitForSecondsRealtime(0.6f);
            Instacne.Animator.SetTrigger("Hide");
            yield return new WaitForSecondsRealtime(0.3f);

            if (OnResult != null)
            {
                Purchase product = new Purchase();
                product.ProductId = SKU;
                product.PurchaseTime = DateTime.UtcNow;
                product.PurchaseToken = Guid.NewGuid().ToString();
                product.State = PurchaseState.Purchased;
                product.OrderId = product.PurchaseToken;
                product.Signature = "";
                product.DeveloperPayload = DeveloperPayload;

                PurchaseResult result = new PurchaseResult();
                result.Status = PurchaseStatus.Success;
                result.TestMode = true;
                result.Purchase = product;
                OnResult(result);

                OnResult = null;
            }
        }
        private IEnumerator IECancel()
        {
            purchaseResultText.text = "انصراف از خرید".ToFarsi();
            purchaseStartPage.SetActive(false);
            purchaseResultPage.SetActive(true);
            yield return new WaitForSecondsRealtime(0.6f);
            Instacne.Animator.SetTrigger("Hide");
            yield return new WaitForSecondsRealtime(0.3f);

            if (OnResult != null)
            {
                PurchaseResult result = new PurchaseResult();
                result.Status = PurchaseStatus.Canceled; 
                result.TestMode = true;
                OnResult(result);

                OnResult = null;
            }
        }

        public static void SetLayerRecursively(GameObject obj, int newLayer)
        {
            if (null == obj)
            {
                return;
            }

            obj.layer = newLayer;

            foreach (Transform child in obj.transform)
            {
                if (null == child)
                {
                    continue;
                }
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }
    }

}
#endif