using UnityEngine;
using CafeBazaar;
using UnityEngine.UI;
using CafeBazaar.Games.BasicApi;
using CafeBazaar.Games;
using CafeBazaar.Games.BasicApi.SavedGame;

public class LoginStorageExample : MonoBehaviour
{
    public Text ConsoleText;

    public Button Btn_SetKey, Btn_GetKey;
    public Button Btn_SigiIn, Btn_SilentSignIn;
    public Text LoginStorageStatus;

    void Start()
    {
        Log("CafeBazaar Plugin Version: " + PluginVersion.VersionString);
        RefreshButtonEnableStatus();

        var config = new BazaarGamesClientConfiguration.Builder().EnableSavedGames().Build();
        BazaarGamesPlatform.InitializeInstance(config);
        BazaarGamesPlatform.Activate();


    }
    private void Update()
    {
        if (BazaarGamesPlatform.Instance.IsAuthenticated())
        {
            if (BazaarGamesPlatform.Instance.SavedGame.IsSynced)
            {
                LoginStorageStatus.text = "Synced";
            }
            else
            {
                LoginStorageStatus.text = "Syncing ...";
            }
        }
    }
    private void RefreshButtonEnableStatus()
    {
        Btn_SetKey.interactable = BazaarGamesPlatform.Instance.IsAuthenticated();
        Btn_GetKey.interactable = BazaarGamesPlatform.Instance.IsAuthenticated();

        Btn_SigiIn.interactable = !BazaarGamesPlatform.Instance.IsAuthenticated();
        Btn_SilentSignIn.interactable = !BazaarGamesPlatform.Instance.IsAuthenticated();
    }

    public void SignInToBazaar()
    {
        Btn_SigiIn.interactable = false;
        Btn_SilentSignIn.interactable = false;

        Log("Signing ... ");

        BazaarGamesPlatform.Instance.Authenticate(false, response =>
        {
            if (response)
                Log("SignedIn to bazaar AccountId : " + BazaarGamesPlatform.Instance.GetUserId());
            else
                Log("SignedIn error ");

            RefreshButtonEnableStatus();
        });
    }

    public void SilentSignInToBazaar()
    {
        Btn_SigiIn.interactable = false;
        Btn_SilentSignIn.interactable = false;

        Log("Silent signing ... ");
        BazaarGamesPlatform.Instance.Authenticate(true, response =>
        {
            if (response)
                Log("SignedIn to bazaar AccountId : " + BazaarGamesPlatform.Instance.GetUserId());
            else
                Log("SignedIn error");

            RefreshButtonEnableStatus();
        });

    }

    public void SetKey()
    {
        string data = Random.Range(0, 1000).ToString();
        BazaarGamesPlatform.Instance.SavedGame.SetString("Data1", data);

        Log("Bazaar Storage : Set Data1 -> " + data);
    }

    public void GetKey()
    {
        var savedGameClient = BazaarGamesPlatform.Instance.SavedGame;
        string value1 = savedGameClient.GetString("Data1");
        Log("Bazaar Storage > Data1 = " + BazaarGamesPlatform.Instance.SavedGame.GetString("Data1"));
    }

    public void Log(string message)
    {
        ConsoleText.text += message + "\n";
    }
}
