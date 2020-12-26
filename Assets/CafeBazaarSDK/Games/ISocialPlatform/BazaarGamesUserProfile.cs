using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;


namespace CafeBazaar.Games
{
    public class BazaarGamesUserProfile : IUserProfile
    {
        private string mDisplayName;
        private string mPlayerId;
        
        
        internal BazaarGamesUserProfile(string displayName, string playerId)
        {
            mDisplayName = displayName;
            mPlayerId = playerId;
        }
        
        
        
        #region IUserProfile implementation

        public string userName
        {
            get { return mDisplayName; }
        }

        public string id
        {
            get { return mPlayerId; }
        }

        public string gameId
        {
            get { return mPlayerId; }
        }

        public bool isFriend
        {
            get { return true; }
        }

        public UserState state
        {
            get { return UserState.Online; }
        }

        public Texture2D image
        {
            get
            {
                return null;
            }
        }

        #endregion
    }
}
