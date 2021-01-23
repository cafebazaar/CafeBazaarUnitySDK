#region USINGS
using CafeBazaar.Core.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CafeBazaar.Core.SimpleJSON;
using CafeBazaar.Billing;
#endregion
namespace CafeBazaar.Core
{
    public class CafeBaseResult
    {
        public string Message { get; set; }
    }

    public class CafeBazaarManager : MonoBehaviour
    {
        #region Singleton
        private static CafeBazaarManager pointer;
        public static CafeBazaarManager Instacne
        {
            get
            {
                if (pointer == null)
                    pointer = FindObjectOfType<CafeBazaarManager>();

                if (pointer == null)
                {
                    GameObject cafebazaarObject = new GameObject("_CAFEBAZAAR_", typeof(CafeBazaarManager));
                    cafebazaarObject.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
                    DontDestroyOnLoad(cafebazaarObject);
                    pointer = cafebazaarObject.GetComponent<CafeBazaarManager>();
                    pointer.Config = Resources.Load<Config>("CafebazaarConfig");

                    if (pointer.Config == null)
                    {
                        Debug.LogError("Cafebazaar config file not found please import CafeSDK package again.");
                    }
                }

                return pointer;
            }
        }
        #endregion

        #region Private Variables
        private Config Config;
        private AndroidJavaObject mPlugin;
        private AndroidJavaObject bazaarBridgePlugin;

        private readonly Dictionary<string, List<Action<CafeBaseResult>>> callBackResult = new Dictionary<string, List<Action<CafeBaseResult>>>();
        #endregion

        #region Unity Functions
        public void Awake()
        {
            BaseInit();
        }
        public void Start()
        {
#if !UNITY_EDITOR
            if (!init_storageLoop)
                StartCoroutine(IEProcess_SotrageLoop());
#endif
        }
        public void Update()
        {
#if UNITY_EDITOR
            if (!init_storageLoop)
                StartCoroutine(IEProcess_SotrageLoop());
#endif
        }
        #endregion

        #region Core functions
        private void BaseInit()
        {
            if (!Application.isEditor)
            {
#if UNITY_ANDROID
                using (var pluginClass = new AndroidJavaClass("com.farsitel.bazaar.BazaarIABPlugin"))
                    mPlugin = pluginClass.CallStatic<AndroidJavaObject>("instance");

                using (AndroidJavaClass pluginClass = new AndroidJavaClass("com.farsitel.bazaar.BazaarBridge"))
                    bazaarBridgePlugin = pluginClass.CallStatic<AndroidJavaObject>("instance");


                EnableLogging(true);
#endif
            }
        }

        private void ExecuteCallBack(string MethodName, CafeBaseResult cafeBaseResult)
        {
            if (callBackResult.TryGetValue(MethodName, out List<Action<CafeBaseResult>> list))
            {
                callBackResult.Remove(MethodName);

                foreach (Action<CafeBaseResult> action in list)
                {
                    try
                    {
                        if (action != null)
                            action(cafeBaseResult);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            }
        }
        private void RegisterCallBack(string MethodName, Action<CafeBaseResult> callBackAction)
        {
            if (callBackAction != null)
            {
                if (!callBackResult.TryGetValue(MethodName, out List<Action<CafeBaseResult>> list))
                {
                    list = new List<Action<CafeBaseResult>>();
                    callBackResult.Add(MethodName, list);
                }

                list.Add(callBackAction);
            }
        }
        private void RemoveCallback(string MethodName)
        {
            callBackResult.Remove(MethodName);
        }
        #endregion
        #region MAIN System
        public string Version { get { return "1.0"; } }
        public bool IAB_IsInited { get; private set; }
        public void IAB_Init(Action<IABInitResult> OnResult)
        {
            IAB_Init(Instacne.Config.InAppPurchase.RSA, OnResult);
        }
        public void IAB_Init(string RSA, Action<IABInitResult> OnResult)
        {
            if (!Application.isEditor)
            {
#if UNITY_ANDROID
                RegisterCallBack("billingSupported", (x) => { OnResult((IABInitResult)x); });
                RegisterCallBack("billingNotSupported", (x) => { OnResult((IABInitResult)x); });

                mPlugin.Call("init", RSA, "com.farsitel.bazaar", "ir.cafebazaar.pardakht");
#else
                if (OnResult != null)
                    OnResult(new IABInitResult() { Status = CoreInitStatus.Failed, Message = "CafeSDK work only on android !"  });
#endif

            }
            else
            {
                IAB_IsInited = true;
                if (OnResult != null)
                    OnResult(new IABInitResult() { Status = CoreInitStatus.Success });
            }
        }

#if UNITY_ANDROID
        public void billingSupported()
        {
            IAB_IsInited = true;
            IABInitResult initResult = new IABInitResult();
            initResult.Status = CoreInitStatus.Success;
            initResult.Message = "Success";
            RemoveCallback("billingNotSupported");
            ExecuteCallBack("billingSupported", initResult);
        }
        public void billingNotSupported(string Message)
        {
            IAB_IsInited = true;
            IABInitResult initResult = new IABInitResult();
            initResult.Message = Message;
            initResult.Status = CoreInitStatus.Failed;
            RemoveCallback("billingSupported");
            ExecuteCallBack("billingNotSupported", initResult);
        }
#endif
        #endregion
        #region IAB System
        public void IAB_UnBind()
        {
            if (!Application.isEditor)
            {
#if UNITY_ANDROID
                mPlugin.Call("unbindService");
#endif
            }
        }
        public bool IAB_SubscriptionsSupported()
        {
            if (!Application.isEditor)
            {
#if UNITY_ANDROID
                return mPlugin.Call<bool>("areSubscriptionsSupported");
#else
                return false;
#endif
            }
            else
            {
                return true;
            }
        }
        public void IAB_QueryInventory(string[] SKUs, Action<GetInventoryResult> OnResult)
        {
            if (!Application.isEditor)
            {
#if UNITY_ANDROID
                mPlugin.Call("queryInventory", new object[] { SKUs });

                RegisterCallBack("queryInventorySucceeded", (x) => { OnResult((GetInventoryResult)x); });
                RegisterCallBack("queryInventoryFailed", (x) => { OnResult((GetInventoryResult)x); });
#endif
            }
            else
            {
                if (OnResult != null)
                    OnResult(new GetInventoryResult() { Status = GetInventoryStatus.Failed, Message = CafeStorageInitError });
            }
        }
        public void IAB_QuerySkuDetails(string[] SKUs, Action<SkuDetailsResult> OnResult)
        {
            if (!Application.isEditor)
            {
#if UNITY_ANDROID
                mPlugin.Call("querySkuDetails", new object[] { SKUs });

                RegisterCallBack("querySkuDetailsSucceeded", (x) => { OnResult((SkuDetailsResult)x); });
                RegisterCallBack("querySkuDetailsFailed", (x) => { OnResult((SkuDetailsResult)x); });
#endif
            }
            else
            {
                if (OnResult != null)
                    OnResult(new SkuDetailsResult() { Status = GetSkuDetailsStatus.Failed, Message = CafeStorageInitError });
            }
        }
        public void IAB_QueryPurchases(Action<GetPurchasesResult> OnResult)
        {
            if (!Application.isEditor)
            {
#if UNITY_ANDROID
                mPlugin.Call("queryPurchases");

                RegisterCallBack("queryPurchasesSucceeded", (x) => { OnResult((GetPurchasesResult)x); });
                RegisterCallBack("queryPurchasesFailed", (x) => { OnResult((GetPurchasesResult)x); });
#endif
            }
        }

        public void IAB_PurchaseProduct(string Sku, string DeveloperPayload, string purchaseType, Action<PurchaseResult> OnResult)
        {
            if (!Application.isEditor)
            {
                if (Application.platform == RuntimePlatform.Android)
                {
#if UNITY_ANDROID
                    RegisterCallBack("purchaseSucceeded", (x) => { OnResult((PurchaseResult)x); });
                    RegisterCallBack("purchaseFailed", (x) => { OnResult((PurchaseResult)x); });

                    mPlugin.Call("purchaseProduct", Sku, DeveloperPayload, purchaseType);
#endif
                }
                else
                {
                    if (OnResult != null)
                        OnResult(new PurchaseResult() { Status = PurchaseStatus.Failed, Message = "CafeSDK work only on android !" });
                }
            }
            else
            {
#if UNITY_EDITOR
                CafebazaarSandboxUI.StartPurchase(Sku, DeveloperPayload, OnResult);
#endif
            }
        }
        public void EnableLogging(bool enable)
        {
#if UNITY_ANDROID
            mPlugin.Call("enableLogging", enable);
#endif
        }
        public void IAB_ConsumeProduct(string Sku, Action<ConsumeResult> OnResult)
        {
            if (!Application.isEditor)
            {
                if (Application.platform == RuntimePlatform.Android)
                {
#if UNITY_ANDROID
                    RegisterCallBack("consumePurchaseSucceeded", (x) => { OnResult((ConsumeResult)x); });
                    RegisterCallBack("consumePurchaseFailed", (x) => { OnResult((ConsumeResult)x); });

                    mPlugin.Call("consumeProduct", Sku);
#endif
                }
                else
                {
                    if (OnResult != null)
                        OnResult(new ConsumeResult() { Status = ConsumeStatus.Failed, Message = "CafeSDK work only on android !" });
                }
            }
            else
            {
                if (OnResult != null)
                    OnResult(new ConsumeResult() { Status = ConsumeStatus.Success, TestMode = true });
            }
        }


#if UNITY_ANDROID
        //QueryInventory
        public void queryInventoryFailed(string Message)
        {
            GetInventoryResult result = new GetInventoryResult();
            result.Status = GetInventoryStatus.Failed;
            result.Message = Message;
            RemoveCallback("queryInventorySucceeded");
            ExecuteCallBack("queryInventoryFailed", result);

        }
        public void queryInventorySucceeded(string JsonData)
        {
            GetInventoryResult result = new GetInventoryResult();
            result.Status = GetInventoryStatus.Success;

            JSONNode jsonNode = JSONNode.Parse(JsonData);

            result.Purchases = new List<Purchase>();
            foreach (JSONNode jn in jsonNode["purchases"].AsArray)
                result.Purchases.Add(Purchase.Parse(jn));

            result.Products = new List<Product>();
            foreach (JSONNode jn in jsonNode["skus"].AsArray)
                result.Products.Add(Product.Parse(jn));

            RemoveCallback("queryInventoryFailed");
            ExecuteCallBack("queryInventorySucceeded", result);
        }
        //QuerySkuDetails
        public void querySkuDetailsSucceeded(string JsonData)
        {
            SkuDetailsResult skuDetailsResult = new SkuDetailsResult();
            skuDetailsResult.Status = GetSkuDetailsStatus.Success;

            skuDetailsResult.Products = new List<Product>();
            foreach (JSONNode jn in JSONNode.Parse(JsonData).AsArray)
                skuDetailsResult.Products.Add(Product.Parse(jn));

            RemoveCallback("querySkuDetailsFailed");
            ExecuteCallBack("querySkuDetailsSucceeded", skuDetailsResult);
        }
        public void querySkuDetailsFailed(string Message)
        {
            SkuDetailsResult result = new SkuDetailsResult();
            result.Status = GetSkuDetailsStatus.Failed;
            result.Message = Message;

            RemoveCallback("querySkuDetailsSucceeded");
            ExecuteCallBack("querySkuDetailsFailed", result);
        }
        //GetPurchasesResult
        public void queryPurchasesSucceeded(string JsonData)
        {
            GetPurchasesResult result = new GetPurchasesResult();
            result.Status = GetPurchasesStatus.Success;

            result.Purchases = new List<Purchase>();
            foreach (JSONNode jn in JSONNode.Parse(JsonData).AsArray)
                result.Purchases.Add(Purchase.Parse(jn));

            RemoveCallback("queryPurchasesFailed");
            ExecuteCallBack("queryPurchasesSucceeded", result);
        }
        public void queryPurchasesFailed(string Message)
        {
            GetPurchasesResult result = new GetPurchasesResult();
            result.Status = GetPurchasesStatus.Failed;
            result.Message = Message;

            RemoveCallback("queryPurchasesSucceeded");
            ExecuteCallBack("queryPurchasesFailed", result);
        }
        //Purchase
        public void purchaseSucceeded(string JsonData)
        {
            PurchaseResult result = new PurchaseResult();
            result.Status = PurchaseStatus.Success;
            result.Purchase = Purchase.Parse(JsonData);

            RemoveCallback("purchaseFailed");
            ExecuteCallBack("purchaseSucceeded", result);
        }
        public void purchaseFailed(string Message)
        {
            PurchaseResult result = new PurchaseResult();
            result.Status = PurchaseStatus.Failed;
            result.Message = Message;

            RemoveCallback("purchaseSucceeded");
            ExecuteCallBack("purchaseFailed", result);
        }
        //Consume
        public void consumePurchaseSucceeded(string JsonData)
        {
            ConsumeResult result = new ConsumeResult();
            result.Status = ConsumeStatus.Success;
            result.Purchase = Purchase.Parse(JsonData);

            RemoveCallback("consumePurchaseFailed");
            ExecuteCallBack("consumePurchaseSucceeded", result);
        }
        public void consumePurchaseFailed(string Message)
        {
            ConsumeResult result = new ConsumeResult();
            result.Status = ConsumeStatus.Failed;
            result.Message = Message;

            RemoveCallback("consumePurchaseSucceeded");
            ExecuteCallBack("consumePurchaseFailed", result);
        }
#endif

        #endregion
        #region LOGIN System
        public bool IsSignIn { get; private set; }
        private string AccountId;
        public void LOGIN_SignIn(bool SilentMode, Action<SignInResult> OnResult)
        {
            if (!Application.isEditor)
            {
                if (Application.platform == RuntimePlatform.Android)
                {
#if UNITY_ANDROID
                    RegisterCallBack("OnLoginSucceed", (x) => { OnResult((SignInResult)x); });
                    RegisterCallBack("OnLoginFailed", (x) => { OnResult((SignInResult)x); });

                    if (SilentMode)
                        bazaarBridgePlugin.Call("SilentSignIn");
                    else
                        bazaarBridgePlugin.Call("SignIn");
#endif
                }
            }
            else
            {
                if (!IsSignIn)
                {
                    AccountId = "TEST_MODE";

                    IsSignIn = true;

                }
                else
                {

                    if (OnResult != null)
                        OnResult(new SignInResult() { Status = CoreSignInStatus.Success, AccountId = AccountId });
                }
            }

        }

        public void OnLoginSucceed(string AccountId)
        {
            SignInResult result = new SignInResult();
            result.Status = CoreSignInStatus.Success;
            result.AccountId = AccountId;
            IsSignIn = true;
            RemoveCallback("OnLoginFailed");
            ExecuteCallBack("OnLoginSucceed", result);
        }
        public void OnLoginFailed()
        {
            SignInResult result = new SignInResult();
            result.Status = CoreSignInStatus.Failed;
            RemoveCallback("OnLoginSucceed");
            ExecuteCallBack("OnLoginFailed", result);
        }
        #endregion
        #region STORAGE System
        public bool StorageIsInit { get; private set; }
        public bool Storage_Is_Synced { get; private set; }
        public bool Storage_Is_Syncing { get; private set; }
        private void STORAGE_SetData(string Data, Action<SetStorageResult> OnResult)
        {
            if (!Application.isEditor)
            {
                if (Application.platform == RuntimePlatform.Android)
                {
#if UNITY_ANDROID
                    //Debug.Log("STORAGE_SetData : " + Data);
                    RegisterCallBack("OnSaveDataSucceed", (x) => { OnResult((SetStorageResult)x); });
                    RegisterCallBack("OnSaveDataFailed", (x) => { OnResult((SetStorageResult)x); });

                    bazaarBridgePlugin.Call("SaveData", Data);
#endif
                }
                else
                {
                    if (OnResult != null)
                    {
                        OnResult(new SetStorageResult() { Status = SetStorageStatus.Failed, Message = "CafeSDK work only on android !" });
                    }
                }
            }
            else
            {
                PlayerPrefs.SetString("cafesdk_storage_data", Data);
                PlayerPrefs.Save();

                if (OnResult != null)
                {
                    OnResult(new SetStorageResult() { Status = SetStorageStatus.Success });
                }
            }
        }
        public void STORAGE_Init(Action<InitStorageResult> OnResult)
        {
            if (!StorageIsInit)
            {
                if (!Application.isEditor)
                {
                    if (Application.platform == RuntimePlatform.Android)
                    {
#if UNITY_ANDROID
                        RegisterCallBack("OnGetDataSucceed", (x) => { OnResult((InitStorageResult)x); });
                        RegisterCallBack("OnGetDataFailed", (x) => { OnResult((InitStorageResult)x); });

                        bazaarBridgePlugin.Call("GetSavedData");
#endif
                    }
                    else
                    {
                        InitStorageResult result = new InitStorageResult();
                        result.Status = InitStorageStatus.Failed;
                        result.Message = "CafeSDK work only on android !";
                        if (OnResult != null)
                            OnResult(result);
                    }
                }
                else
                {
                    InitStorageResult result = new InitStorageResult();
                    result.Status = InitStorageStatus.Success;
                    Storage_LoadData(PlayerPrefs.GetString("cafesdk_storage_data", ""));
                    Storage_Is_Synced = true;
                    Storage_Is_Syncing = false;
                    StorageIsInit = true;
                    if (OnResult != null)
                        OnResult(result);
                }
            }
            else
            {
                InitStorageResult result = new InitStorageResult();
                result.Status = InitStorageStatus.Success;
                if (OnResult != null)
                    OnResult(result);
            }
        }
        public void OnGetDataSucceed(string data)
        {
            InitStorageResult initStorageResult = new InitStorageResult();
            initStorageResult.Status = InitStorageStatus.Success;
            Storage_LoadData(data);
            Storage_Is_Synced = true;
            Storage_Is_Syncing = false;
            StorageIsInit = true;
            lastChangeStorage = Time.unscaledTime;

            RemoveCallback("OnGetDataFailed");
            ExecuteCallBack("OnGetDataSucceed", initStorageResult);
        }
        public void OnGetDataFailed(string error)
        {
            InitStorageResult initStorageResult = new InitStorageResult();
            initStorageResult.Status = InitStorageStatus.Failed;
            initStorageResult.Message = error;
            RemoveCallback("OnGetDataSucceed");
            ExecuteCallBack("OnGetDataFailed", initStorageResult);
        }

        public void OnSaveDataSucceed(string data)
        {
            SetStorageResult setStorageResult = new SetStorageResult();
            setStorageResult.Status = SetStorageStatus.Success;
            //Debug.Log("OnSaveDataSucceed " + data);
            RemoveCallback("OnSaveDataFailed");
            ExecuteCallBack("OnSaveDataSucceed", setStorageResult);
        }
        public void OnSaveDataFailed(string error)
        {
            SetStorageResult setStorageResult = new SetStorageResult();
            setStorageResult.Status = SetStorageStatus.Failed;
            setStorageResult.Message = error;
            //Debug.Log("OnSaveDataFailed " + error);
            RemoveCallback("OnSaveDataSucceed");
            ExecuteCallBack("OnSaveDataFailed", setStorageResult);
        }

        private IEnumerator IEOnLoginToCafebazaarSuccessfull(Action<SignInResult> OnResult, SignInResult loginResult)
        {
            yield return true;
            yield return true;
            CafebazaarLoginUI.Instacne.Show();
            yield return new WaitForSecondsRealtime(1.4f);

            if (OnResult != null)
                OnResult(loginResult);
        }

        private readonly Dictionary<string, string> storageKeyValue = new Dictionary<string, string>();
        private static bool init_storageLoop;
        private static float lastChangeStorage;

        private string Storage_CalculateSaveData()
        {
            JSONClass saveObject = new JSONClass();

            JSONArray jSONArray = new JSONArray();

            foreach (var i in storageKeyValue)
            {
                JSONClass jSONClass = new JSONClass();
                jSONClass["k"] = i.Key;
                jSONClass["v"] = i.Value;
                jSONArray.Add(jSONClass);
            }

            saveObject["keys"] = jSONArray;
            saveObject["utc"] = DateTime.UtcNow.ToString();
            return saveObject.ToString();
        }
        private void Storage_LoadData(string Data)
        {
            if (Data != "")
            {
                JSONClass json;
                try { json = JSONNode.Parse(Data).AsObject; } catch { json = new JSONClass(); }

                storageKeyValue.Clear();

                if (json["keys"] != null)
                    foreach (JSONClass i in json["keys"].AsArray)
                    {
                        storageKeyValue.Add(i["k"].Value, i["v"].Value);
                    }
            }
            else
            {
                storageKeyValue.Clear();
            }
        }
        private IEnumerator IEProcess_SotrageLoop()
        {
            init_storageLoop = true;
            while (true)
            {
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();

                if (!Storage_Is_Synced && StorageIsInit)
                {
                    float _lastChangeStorage = lastChangeStorage;
                    string data_to_save = Storage_CalculateSaveData();

                    int falg = -1;
                    STORAGE_SetData(data_to_save,
                        (result) =>
                        {
                            if (result.Status == SetStorageStatus.Success)
                            {
                                falg = 1;
                            }
                            else
                            {
                                if (result.Message == "CafeSDK work only on android !")
                                {
                                    falg = 10;
                                }
                                else
                                    falg = 0;
                            }

                        });

                    while (falg == -1)
                        yield return true;

                    if (falg == 10)
                    {
                        yield break;
                    }
                    else
                    if (falg == 1)
                    {
                        if (_lastChangeStorage == lastChangeStorage)
                        {
                            Storage_Is_Synced = true;
                            Storage_Is_Syncing = false;
                        }
                        else
                        {
                            Storage_Is_Synced = false;
                            Storage_Is_Syncing = true;
                        }
                    }
                    else
                    {
                        yield return new WaitForSecondsRealtime(3);
                    }

                }
            }
        }

        private const string CafeStorageInitError = "Init Storage before Set Data key!  Call Storage.Init();";
        public void Storage_SetKey(string Key, string Value)
        {
            if (StorageIsInit)
            {
                if (storageKeyValue.ContainsKey(Key))
                {
                    if (storageKeyValue[Key] != Value)
                    {
                        Storage_Is_Syncing = true;
                        Storage_Is_Synced = false;
                        storageKeyValue[Key] = Value;
                        lastChangeStorage = Time.unscaledTime;
                    }
                }
                else
                {
                    storageKeyValue.Add(Key, Value);
                    Storage_Is_Syncing = true;
                    Storage_Is_Synced = false;
                    lastChangeStorage = Time.unscaledTime;
                }
            }
            else
            {
                Debug.LogError(CafeStorageInitError);
            }
        }
        public string Storage_GetKey(string Key, string defaultValue)
        {
            if (storageKeyValue.ContainsKey(Key))
                return storageKeyValue[Key];
            else
                return defaultValue;
        }
        public bool Storage_HasKey(string Key)
        {
            return storageKeyValue.ContainsKey(Key);
        }
        public void Storage_DeleteKey(string Key)
        {
            if (StorageIsInit)
            {
                if (storageKeyValue.ContainsKey(Key))
                {
                    storageKeyValue.Remove(Key);
                    Storage_Is_Syncing = true;
                    Storage_Is_Synced = false;
                    lastChangeStorage = Time.unscaledTime;
                }
            }
            else
            {
                Debug.LogError(CafeStorageInitError);
            }
        }
        public void Storage_Clear()
        {
            if (StorageIsInit)
            {
                storageKeyValue.Clear();
                Storage_Is_Syncing = true;
                Storage_Is_Synced = false;
                lastChangeStorage = Time.unscaledTime;
            }
            else
            {
                Debug.LogError(CafeStorageInitError);
            }
        }
        #endregion
    }

    public enum GetStorageStatus
    {
        Success,
        Failed,
    }
    public enum SetStorageStatus
    {
        Success,
        Failed,
    }
    public class SetStorageResult : CafeBaseResult
    {
        public SetStorageStatus Status { get; set; }

    }
}