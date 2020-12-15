using System;
using System.Threading;
using CafeBazaar.Core;
using CafeBazaar.Core.UI;
using CafeBazaar.Games.BasicApi;
using CafeBazaar.Games.BasicApi.SavedGame;
using UnityEngine;

namespace CafeBazaar.Games
{
    public class AndroidClient : IBazaarGamesClient
    {
        private readonly BazaarGamesClientConfiguration mConfiguration;
        private readonly ISavedGameClient mSavedGameClient;

        private volatile Player mUser = null;

        private bool isAuthenticated;
        internal AndroidClient(BazaarGamesClientConfiguration configuration)
        {
            this.mConfiguration = configuration;
            this.mSavedGameClient = new AndroidSavedGameClient();
        }


        public void Authenticate(bool silent, Action<BasicApi.SignInStatus> callback)
        {
            CafeBazaar.Core.CafeBazaarManager.Instacne.LOGIN_SignIn(silent,
                (result) =>
                {
                    if (result.Status == CoreSignInStatus.Success)
                    {
                        mUser = new Player(result.AccountId, result.AccountId);
                        //afterSuccessFull authenticate
                        if (mConfiguration.EnableSavedGames)
                        {
                            //Inital Storage
                            CafeBazaar.Core.CafeBazaarManager.Instacne.STORAGE_Init(
                                (storage_result) =>
                                {
                                    if (storage_result.Status == InitStorageStatus.Success)
                                    {
                                        CafebazaarLoginUI.Instacne.Show();
                                        isAuthenticated = true;
                                        if (callback != null)
                                            callback(SignInStatus.Success);
                                    }
                                    else
                                    {
                                        isAuthenticated = false;
                                        if (callback != null)
                                            callback(SignInStatus.Failed);
                                    }
                                });
                        }
                        else
                        {
                            CafebazaarLoginUI.Instacne.Show();
                            isAuthenticated = true;
                            if (callback != null)
                                callback(SignInStatus.Success);
                        }
                    }
                    else
                    {
                        isAuthenticated = false;
                        if (callback != null)
                            callback(SignInStatus.Failed);
                    }
                }
                );
        }

        public string GetUserDisplayName()
        {
            if (mUser == null)
            {
                return null;
            }

            return mUser.userName;
        }

        public string GetUserId()
        {
            if (mUser == null)
            {
                return null;
            }

            return mUser.id;
        }


        public bool IsAuthenticated()
        {
            return isAuthenticated;
        }

        public ISavedGameClient GetSavedGameClient()
        {
            return mSavedGameClient;
        }


    }
}
