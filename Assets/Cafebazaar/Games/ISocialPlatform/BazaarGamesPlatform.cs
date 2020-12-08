using System;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using CafeBazaar.Games.BasicApi;
using CafeBazaar.Games.Utilites;
using CafeBazaar.Games.BasicApi.SavedGame;

namespace CafeBazaar.Games
{
    public class BazaarGamesPlatform : ISocialPlatform
    {
        private static volatile BazaarGamesPlatform sInstance = null;
        private BazaarGamesLocalUser mLocalUser = null;
        private readonly BazaarGamesClientConfiguration mConfiguration;
        private IBazaarGamesClient mClient = null;

        internal BazaarGamesPlatform(IBazaarGamesClient client)
        {
            this.mClient = client;
            this.mLocalUser = new BazaarGamesLocalUser(this);
            this.mConfiguration = BazaarGamesClientConfiguration.DefaultConfiguration;
        }

        private BazaarGamesPlatform(BazaarGamesClientConfiguration configuration)
        {
            Debug.Log("Creating new BazaarGamesPlatform");
            this.mLocalUser = new BazaarGamesLocalUser(this);
            this.mConfiguration = configuration;
            BazaarGamesHelperObject.CreateObject();
        }

        public void Authenticate(Action<bool> callback)
        {
            Authenticate(true, callback);
        }

        public void Authenticate(Action<bool, string> callback)
        {
            Authenticate(true, callback);
        }

        public void Authenticate(bool silent, Action<bool> callback)
        {
            Authenticate(silent, (bool success, string msg) => callback(success));
        }

        public void Authenticate(bool silent, Action<bool, string> callback)
        {
            Authenticate(silent ? SignInInteractivity.NoPrompt : SignInInteractivity.CanPromptAlways, status =>
            {
                if (status == SignInStatus.Success)
                {
                    callback(true, "Authentication succeeded");
                }
                else if (status == SignInStatus.Canceled)
                {
                    callback(false, "Authentication canceled");
                    Debug.Log("Authentication canceled");
                }
                else if (status == SignInStatus.DeveloperError)
                {
                    callback(false, "Authentication failed - developer error");
                    Debug.Log("Authentication failed - developer error");
                }
                else
                {
                    callback(false, "Authentication failed");
                    Debug.Log("Authentication failed");
                }
            });
        }

        public void Authenticate(SignInInteractivity signInInteractivity, Action<SignInStatus> callback)
        {
            if (mClient == null)
            {
                Debug.Log("Creating Bazaar Games client.");
                mClient = BazaarGamesClientFactory.GetPlatformBazaarGamesClient(mConfiguration);
            }

            if (callback == null)
            {
                callback = status => { };
            }

            switch (signInInteractivity)
            {
                case SignInInteractivity.NoPrompt:
                    mClient.Authenticate(true, code =>
                 {
                        // SignInStatus.UiSignInRequired is returned when silent sign in fails or when there is no
                        // internet connection.
                        if (code == SignInStatus.UiSignInRequired &&
                         Application.internetReachability == NetworkReachability.NotReachable)
                     {
                         callback(SignInStatus.NetworkError);
                     }
                     else
                     {
                         callback(code);
                     }
                 });
                    break;

                case SignInInteractivity.CanPromptAlways:
                    mClient.Authenticate(false, code =>
                  {
                        // SignInStatus.Canceled is returned when interactive sign in fails or when there is no internet connection.
                        if (code == SignInStatus.Canceled &&
                          Application.internetReachability == NetworkReachability.NotReachable)
                      {
                          callback(SignInStatus.NetworkError);
                      }
                      else
                      {
                          callback(code);
                      }
                  });
                    break;

                case SignInInteractivity.CanPromptOnce:

                    // 1. Silent sign in first
                    mClient.Authenticate(true, silentSignInResultCode =>
                  {
                      if (silentSignInResultCode == SignInStatus.Success)
                      {
                          Debug.Log("Successful, triggering callback");
                          callback(silentSignInResultCode);
                          return;
                      }

                        // 2. Check the shared pref and bail out if it's true.
                        if (!SignInHelper.ShouldPromptUiSignIn())
                      {
                          Debug.Log(
                              "User cancelled sign in attempt in the previous attempt. Triggering callback with silentSignInResultCode");
                          callback(silentSignInResultCode);
                          return;
                      }

                        // 3. Check internet connection
                        if (Application.internetReachability == NetworkReachability.NotReachable)
                      {
                          Debug.Log("No internet connection");
                          callback(SignInStatus.NetworkError);
                          return;
                      }

                        // 4. Interactive sign in
                        mClient.Authenticate(false, interactiveSignInResultCode =>
                      {
                            // 5. Save that the user has cancelled the interactive sign in.
                            if (interactiveSignInResultCode == SignInStatus.Canceled)
                          {
                              Debug.Log("Cancelled, saving this to a shared pref");
                              SignInHelper.SetPromptUiSignIn(false);
                          }

                          callback(interactiveSignInResultCode);
                      });
                  });
                    break;

                default:
                    BazaarGamesHelperObject.RunOnGameThread(() => callback(SignInStatus.Failed));
                    break;
            }
        }


        public string GetUserDisplayName()
        {
            if (!IsAuthenticated())
            {
                Debug.Log("GetUserDisplayName can only be called after authentication.");
                return string.Empty;
            }

            return mClient.GetUserDisplayName();
        }


        public string GetUserId()
        {
            if (!IsAuthenticated())
            {
                Debug.Log("GetUserId() can only be called after authentication.");
                return "0";
            }

            return mClient.GetUserId();
        }

        public void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback)
        {
            if (!IsAuthenticated())
            {
                Debug.Log("GetUserId() can only be called after authentication.");
                callback(new IUserProfile[0]);

                return;
            }

            //mClient.LoadUsers(userIds, callback);
        }

        public void ReportProgress(string achievementID, double progress, Action<bool> callback)
        {
            throw new NotImplementedException();
        }

        public void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback)
        {
            throw new NotImplementedException();
        }

        public void LoadAchievements(Action<IAchievement[]> callback)
        {
            throw new NotImplementedException();
        }

        public IAchievement CreateAchievement()
        {
            throw new NotImplementedException();
        }

        public void ReportScore(long score, string board, Action<bool> callback)
        {
            throw new NotImplementedException();
        }

        public void LoadScores(string leaderboardID, Action<IScore[]> callback)
        {
            throw new NotImplementedException();
        }

        public ILeaderboard CreateLeaderboard()
        {
            throw new NotImplementedException();
        }

        public void ShowAchievementsUI()
        {
            throw new NotImplementedException();
        }

        public void ShowLeaderboardUI()
        {
            throw new NotImplementedException();
        }

        public void Authenticate(ILocalUser user, Action<bool> callback)
        {
            Authenticate(true, callback);
        }

        public void Authenticate(ILocalUser user, Action<bool, string> callback)
        {
            Authenticate(true, callback);
        }


        public void LoadFriends(ILocalUser user, Action<bool> callback)
        {
            throw new NotImplementedException();
        }

        public void LoadScores(ILeaderboard board, Action<bool> callback)
        {
            throw new NotImplementedException();
        }

        public bool GetLoading(ILeaderboard board)
        {
            throw new NotImplementedException();
        }

        public ILocalUser localUser
        {
            get { return mLocalUser; }
        }

        public bool IsAuthenticated()
        {
            return mClient != null && mClient.IsAuthenticated();
        }

        public ISavedGameClient SavedGame
        {
            get { return mClient.GetSavedGameClient(); }
        }


        public static BazaarGamesPlatform Instance
        {
            get
            {
                if (sInstance == null)
                {
                    InitializeInstance(BazaarGamesClientConfiguration.DefaultConfiguration);
                }

                return sInstance;
            }
        }

        public static void InitializeInstance(BazaarGamesClientConfiguration configuration)
        {
            if (sInstance == null || sInstance.mConfiguration != configuration)
            {
                sInstance = new BazaarGamesPlatform(configuration);
                return;
            }

        }


        public static BazaarGamesPlatform Activate()
        {
            Debug.Log("Activating BazaarGamesPlatform.");
            Social.Active = BazaarGamesPlatform.Instance;
            Debug.Log("BazaarGamesPlatform activated: " + Social.Active);
            return BazaarGamesPlatform.Instance;
        }
    }

}

