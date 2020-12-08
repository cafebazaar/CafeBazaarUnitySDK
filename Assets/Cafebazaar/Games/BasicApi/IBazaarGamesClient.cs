using System;
using UnityEngine.SocialPlatforms;

namespace CafeBazaar.Games.BasicApi
{
    public interface IBazaarGamesClient 
    {
        void Authenticate(bool silent, Action<SignInStatus> callback);
        
        bool IsAuthenticated();
        string GetUserDisplayName();
        string GetUserId();

        SavedGame.ISavedGameClient GetSavedGameClient();
    }
}
