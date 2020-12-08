using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using CafeBazaar.Billing;

namespace CafeBazaar.Core
{
    public class CafebazaarManager : EditorWindow
    {
        public GUISkin Skin;
        public Config Config;
        [MenuItem("CafeBazaar/Config")]
        public static void InitSimple()
        {
            Selection.activeObject = Resources.Load<Config>("CafebazaarConfig");
        }
        //[MenuItem("CafeBazaar/GUISetup")]
        public static void InitAdvacne()
        {

            CafebazaarManager window = (CafebazaarManager)EditorWindow.GetWindow(typeof(CafebazaarManager));
            window.Skin = Resources.Load<GUISkin>("Cafebazaar/Skin");
            window.Config = Resources.Load<Config>("CafebazaarConfig");
            window.titleContent.text = "Cafebazaar";
            window.Show();
            window.LoadPages();

        }
        private List<EditorBasePage> Pages;

        private EditorBasePage currentPage;
        private Stack<EditorBasePage> stackPage;
        public void LoadPages()
        {
            stackPage = new Stack<EditorBasePage>();
            Pages = new List<EditorBasePage>();
            Pages.Add(new EditorLoginPage() { Name = "Login" });

            Pages.Add(new EditorSingleAppPage() { Name = "SingleApp" });

            foreach (EditorBasePage page in Pages)
            {
                page.Manager = this;
                page.Load();
            }



            OpenPage("SingleApp");
        }
        public void OpenPage(string name)
        {
            EditorBasePage page = Pages.Where(i => i.Name == name).FirstOrDefault();
            if (page != null)
            {
                if (currentPage != null)
                    stackPage.Push(currentPage);

                currentPage = page;
                currentPage.OnOpen();
            }
        }
        public void OpenPage(EditorBasePage page)
        {
            if (page != null)
            {
                page.Manager = this;
                if (currentPage != null)
                    stackPage.Push(currentPage);

                currentPage = page;
                currentPage.OnOpen();
                GUI.enabled = false;
                GUI.enabled = true;
            }
        }
        public void BackPage()
        {
            if (stackPage.Count > 0)
            {
                var page = stackPage.Pop();
                currentPage = page;
            }
        }
        public void OnGUI()
        {
            GUI.Label(new Rect(0, 0, position.width, position.height), "", GetStyle("background"));

            if (currentPage == null)
                LoadPages();

            if (currentPage != null)
                currentPage.Render();

            if (GUI.changed)
                EditorUtility.SetDirty(Config);
        }

        public GUIStyle GetStyle(string name)
        {
            return Skin.customStyles.Where(i => i.name == name).FirstOrDefault();
        }

        public class EditorBasePage
        {
            public CafebazaarManager Manager { get; set; }
            public string Name { get; set; }
            public virtual void Load()
            {

            }
            public virtual void OnOpen()
            {

            }
            public GUIStyle GetStyle(string name)
            {
                return Manager.GetStyle(name);
            }
            public virtual void Render()
            {
                GUI.skin = Manager.Skin;
            }
            public void OpenPage(string name)
            {
                Manager.OpenPage(name);
            }
            public void OpenPage(EditorBasePage page)
            {
                Manager.OpenPage(page);
            }
            public void BackPage()
            {
                Manager.BackPage();
            }
            public void Label(string Text, bool Bold = false, params GUILayoutOption[] gUILayoutOptions)
            {
                if (Bold)
                    EditorGUILayout.LabelField(Text.ToFarsi(), GetStyle("LableBold"), gUILayoutOptions);
                else
                    EditorGUILayout.LabelField(Text.ToFarsi(), GetStyle("Lable"), gUILayoutOptions);
            }
            public string TextField(string Text, int Height = 40)
            {

                var res = EditorGUILayout.TextField(Text, GetStyle("InputTextFa_L1"), GUILayout.MinHeight(Height));
                Rect rect = GUILayoutUtility.GetLastRect();

                EditorGUI.LabelField(rect, res.ToFarsi(), GetStyle("InputTextFa_L2"));
                return res;
            }
            public string TextField(string Text, params GUILayoutOption[] gUILayoutOptions)
            {
                var res = EditorGUILayout.TextField(Text, GetStyle("InputTextFa_L1"), gUILayoutOptions);
                Rect rect = GUILayoutUtility.GetLastRect();

                EditorGUI.LabelField(rect, Text.ToFarsi(), GetStyle("InputTextFa_L2"));
                return res;
            }
            public void SelectLabel(string Text, params GUILayoutOption[] gUILayoutOptions)
            {
                var res = EditorGUILayout.TextField(Text, GetStyle("Lable"), gUILayoutOptions);
                Rect rect = GUILayoutUtility.GetLastRect();

                EditorGUI.LabelField(rect, Text.ToFarsi(), GetStyle("Lable"));
            }
            public string TextArea(string Text, int Height = 100)
            {
                return EditorGUILayout.TextField(Text, GetStyle("AreaText"), GUILayout.MinHeight(Height));
            }
            public bool Button(string Text, int Height = -1)
            {
                if (Height == -1)
                    return GUILayout.Button(Text, GetStyle("ButtonGreen"));
                else
                    return GUILayout.Button(Text, GetStyle("ButtonGreen"), GUILayout.MaxHeight(Height));
            }
            public bool DSButton(string Text, int Height = -1)
            {
                if (Height == -1)
                    return GUILayout.Button(Text, GetStyle("DSButtonGreen"));
                else
                    return GUILayout.Button(Text, GetStyle("DSButtonGreen"), GUILayout.MaxHeight(Height));
            }
        }
        public class EditorLoginPage : EditorBasePage
        {
            public string Username { get; set; }
            public string Password { get; set; }

            public override void Load()
            {
                base.Load();
                Username = "";
                Password = "";
            }
            public override void Render()
            {
                base.Render();

                Rect screenSize = new Rect();
                screenSize.size = new Vector2(Mathf.Min(Manager.position.width - 25, 250), Mathf.Min(Manager.position.height - 25, 300));
                screenSize.center = new Vector2(Manager.position.width / 2, Manager.position.height / 2);

                Rect logoRect = new Rect();
                logoRect.size = new Vector2(127, 50) * 0.7f;
                logoRect.center = new Vector2(Manager.position.width / 2, screenSize.yMin - logoRect.height);

                GUI.Box(logoRect, "", GetStyle("bazaarLogo"));

                GUILayout.BeginArea(screenSize, GetStyle("Group"));

                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();

                EditorGUILayout.LabelField("Email:", GetStyle("Lable"));
                Username = TextField(Username, 40);
                GUILayout.Space(15);
                EditorGUILayout.LabelField("Password:", GetStyle("Lable"));
                Password = EditorGUILayout.PasswordField(Password, GetStyle("InputText"), GUILayout.MinHeight(40));
                GUILayout.Space(25);
                if (GUILayout.Button("Login", GetStyle("ButtonGreen"), GUILayout.MinHeight(40)))
                {
                    OpenPage("SingleApp");
                }
                GUILayout.Space(10);

                if (GUILayout.Button("Manual setup", GetStyle("clickableLable")))
                    OpenPage("SingleApp");

                GUILayout.Space(10);

                if (GUILayout.Button("Forget password ?", GetStyle("clickableLable")))
                    Application.OpenURL("https://pishkhan.cafebazaar.ir/account/forget-password");

                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();

                GUILayout.EndArea();



            }
        }


        public class EditorSingleAppPage : EditorBasePage
        {
            public string[] Pages = new string[] { "IAP", "Login" };
            public string CurrentPage
            {
                get
                {
                    return EditorPrefs.GetString("cafebazaar_single_app_current_page", "IAP");
                }
                set
                {
                    EditorPrefs.SetString("cafebazaar_single_app_current_page", value);
                }
            }
            public string PackageName { get; set; }
            public string RSA { get; set; }
            public override void Load()
            {
                base.Load();
            }
            Vector2 p = new Vector2();
            public override void Render()
            {
                base.Render();

                GUILayout.BeginHorizontal();

                #region Left side
                GUILayout.BeginVertical(GetStyle("WhiteGroup"), GUILayout.ExpandHeight(true), GUILayout.MaxWidth(200));

                GUILayout.Box("", GetStyle("bazaarLogo"), GUILayout.Height(50 * 0.4f), GUILayout.MinWidth(127 * 0.4f));

                GUILayout.Space(30);

                foreach (var page in Pages)
                {
                    if (CurrentPage == page)
                        Button(page, 30);
                    else
                    if (DSButton(page, 30))
                    {
                        CurrentPage = page;
                    }
                }
                //Button("Login with bazaar", 30);

                GUILayout.FlexibleSpace();

                if (Button("Back"))
                {
                    OpenPage("Login");
                }

                GUILayout.EndVertical();

                #endregion

                #region Right side    
                GUILayout.BeginVertical(GetStyle("White-1Group"), GUILayout.ExpandHeight(true));

                switch (CurrentPage)
                {
                    case "IAP":
                        IAP_Page();
                        break;
                    case "Login":
                        Login_Page();
                        break;

                }

                GUILayout.EndVertical();
                #endregion


                GUILayout.EndHorizontal();

            }

            void IAP_Page()
            {
                Label("In app purchase", true);
                GUILayout.Space(30);
                Label("RSA:");
                Manager.Config.InAppPurchase.RSA = TextArea(Manager.Config.InAppPurchase.RSA, 100);

                if (Manager.Config.InAppPurchase.RSA.Length < 20)
                {
                    Label("برای دریافت این کد روی لینک زیر کلیک کن");
                    if (GUILayout.Button("دریافت کد پرداخت".ToFarsi(), GetStyle("clickableLable")))
                        Application.OpenURL("https://pishkhan.cafebazaar.ir/apps/" + Application.identifier + "/in-app-billing");
                }

                GUILayout.Space(30);
                GUILayout.BeginHorizontal();
                Label("Products :");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Add product (+)", GetStyle("ButtonGreen")))
                {
                    OpenPage(new EditorProductPage() { Product = new Product() { Title = "", ProductId = "", Kind = ProductKind._Purchasable }, NewCreate = true });
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(20);

                p = GUILayout.BeginScrollView(p);
                foreach (Product product in Manager.Config.InAppPurchase.Products)
                {
                    GUILayout.BeginHorizontal(GetStyle("WhiteGroup"), GUILayout.MaxHeight(20));
                    SelectLabel(product.ProductId, GUILayout.MaxWidth(70));
                    GUILayout.FlexibleSpace();
                    Label(product.Title, false, GUILayout.MaxWidth(90));
                    GUILayout.FlexibleSpace();
                    Label(product.Price + " IRR", false, GUILayout.MaxWidth(70));
                    GUILayout.FlexibleSpace();
                    Label(product.Kind.ToString().TrimStart('_'), false, GUILayout.MaxWidth(130));
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Edit", GetStyle("ButtonGreen"), GUILayout.MaxHeight(30), GUILayout.MaxWidth(50)))
                    {
                        OpenPage(new EditorProductPage() { Product = product });
                    }

                    if (GUILayout.Button("X", GetStyle("ButtonRed"), GUILayout.MaxHeight(30), GUILayout.MaxWidth(30)))
                    {
                        if (EditorUtility.DisplayDialog("Remove alert!", "Are you sure to remove " + product.ProductId + "?", "Yes", "No"))
                        {
                            Manager.Config.InAppPurchase.Products.Remove(product);
                            break;
                        }
                    }

                    GUILayout.EndHorizontal();
                    GUILayout.Space(3);
                }
                GUILayout.EndScrollView();
            }
            void Login_Page()
            {
                Label("Login with bazaar", true);
                GUILayout.Space(30);
                Label("ClientId:");
                // Manager.Config.LoginClientId = TextField(Manager.Config.LoginClientId, GUILayout.MaxWidth(250), GUILayout.MinHeight(40));
                GUILayout.Space(30);

                Label("برای دریافت این کد روی لینک زیر کلیک کن");
                if (GUILayout.Button("صفحه لاگین با بازار".ToFarsi(), GetStyle("clickableLable")))
                    Application.OpenURL("https://pishkhan.cafebazaar.ir/apps/" + Application.identifier + "/oauth");
            }
        }



        public class EditorProductPage : EditorBasePage
        {
            public bool NewCreate;
            public Product Product { get; set; }
            private Product DraftProduct;
            public override void Load()
            {

                base.Load();
            }
            public override void OnOpen()
            {
                DraftProduct = new Product();
                DraftProduct.Title = Product.Title;
                DraftProduct.ProductId = Product.ProductId;
                DraftProduct.Price = Product.Price;
                DraftProduct.Kind = Product.Kind;
            }
            public override void Render()
            {
                base.Render();


                Rect screenSize = new Rect();
                screenSize.size = new Vector2(Mathf.Min(Manager.position.width - 25, 250), Mathf.Min(Manager.position.height - 25, 400));
                screenSize.center = new Vector2(Manager.position.width / 2, Manager.position.height / 2);


                GUILayout.BeginArea(screenSize, GetStyle("Group"));

                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();

                EditorGUILayout.LabelField("Sku:", GetStyle("Lable"));
                DraftProduct.ProductId = TextField(DraftProduct.ProductId, 40);
                GUILayout.Space(15);

                EditorGUILayout.LabelField("Title:", GetStyle("Lable"));
                DraftProduct.Title = TextField(DraftProduct.Title, 40);
                GUILayout.Space(15);
                EditorGUILayout.LabelField("Price:(IRR)", GetStyle("Lable"));
                if (DraftProduct.Price == null)
                    DraftProduct.Price = "";
                DraftProduct.Price = TextField(DraftProduct.Price, GUILayout.MinHeight(40));
                GUILayout.Space(15);
                EditorGUILayout.LabelField("Kind:", GetStyle("Lable"));
                DraftProduct.Kind = (ProductKind)EditorGUILayout.EnumPopup(DraftProduct.Kind, GetStyle("DropDown"), GUILayout.MinHeight(30));
                GUILayout.Space(15);

                GUILayout.BeginHorizontal();

                GUI.enabled = DraftProduct.ProductId.Length > 1;
                if (NewCreate)
                {

                    if (GUILayout.Button("Add", GetStyle("ButtonGreen"), GUILayout.MinHeight(40)))
                    {
                        Manager.Config.InAppPurchase.Products.Add(DraftProduct);
                        BackPage();
                    }
                }
                else
                {
                    if (GUILayout.Button("Save", GetStyle("ButtonGreen"), GUILayout.MinHeight(40)))
                    {
                        Product.Title = DraftProduct.Title;
                        Product.ProductId = DraftProduct.ProductId;
                        Product.Price = DraftProduct.Price;
                        Product.Kind = DraftProduct.Kind;
                        BackPage();
                    }
                }
                GUI.enabled = true;
                GUILayout.Space(10);
                if (GUILayout.Button("Close", GetStyle("ButtonGreen"), GUILayout.MinHeight(40)))
                {
                    BackPage();
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();

                GUILayout.EndArea();
            }
        }
    }
}
