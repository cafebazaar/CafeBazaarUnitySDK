using UnityEngine;

namespace CafeBazaar.Games.BasicApi
{
    internal class BazaarGamesClientFactory
    {
        internal static IBazaarGamesClient GetPlatformBazaarGamesClient(BazaarGamesClientConfiguration config)
        {
            if (Application.isEditor)
            {
                CafeBazaar.Games.OurUtils.Logger.d("Creating IBazaarGamesClient in editor, using DummyClient.");
                return new DummyClient();
            }

#if UNITY_ANDROID
            CafeBazaar.Games.OurUtils.Logger.d("Creating Android IBazaarGamesClient Client");
            return new AndroidClient(config);
#else
            CafeBazaar.Games.OurUtils.Logger.d("Cannot create IBazaarGamesClient for unknown platform, returning DummyClient");
            return new DummyClient();
#endif
        }
    }
}
