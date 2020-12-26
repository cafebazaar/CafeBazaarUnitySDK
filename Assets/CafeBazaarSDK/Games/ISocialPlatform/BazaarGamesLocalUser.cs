using System;
using UnityEngine.SocialPlatforms;


namespace CafeBazaar.Games
{
    public class BazaarGamesLocalUser : BazaarGamesUserProfile, ILocalUser
    {
        internal BazaarGamesPlatform mPlatform;



        internal BazaarGamesLocalUser(BazaarGamesPlatform plaf)
            : base("localUser", string.Empty)
        {
            mPlatform = plaf;
        }





        public void Authenticate(Action<bool> callback)
        {
            mPlatform.Authenticate(callback);
        }

        public void Authenticate(Action<bool, string> callback)
        {
            mPlatform.Authenticate(callback);
        }

        public void Authenticate(bool silent, Action<bool> callback)
        {
            mPlatform.Authenticate(silent, callback);
        }

        public void Authenticate(bool silent, Action<bool, string> callback)
        {
            mPlatform.Authenticate(silent, callback);
        }


        public void LoadFriends(Action<bool> callback)
        {
            throw new NotImplementedException();
        }

        public IUserProfile[] friends { get; }

        public bool authenticated
        {
            get { return mPlatform.IsAuthenticated(); }
        }


        public new string userName
        {
            get
            {
                string retval = string.Empty;
                if (authenticated)
                {
                    retval = mPlatform.GetUserDisplayName();
                }

                return retval;
            }
        }

        public new string id
        {
            get
            {
                string retval = string.Empty;
                if (authenticated)
                {
                    retval = mPlatform.GetUserId();
                }

                return retval;
            }
        }

        public bool underage { get; }
    }
}

