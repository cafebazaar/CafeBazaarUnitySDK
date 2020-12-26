
namespace CafeBazaar.Games.BasicApi
{
    using System;
    using CafeBazaar.Games.BasicApi.SavedGame;
    using CafeBazaar.Games.OurUtils;

    /// <summary>
    /// Dummy client used in Editor.
    /// </summary>
    /// <remarks>CafeBazaar Game Services are not supported in the Editor
    /// environment, so this client is used as a placeholder.
    /// </remarks>
    public class DummyClient : IBazaarGamesClient
    {
        private readonly ISavedGameClient mSavedGameClient;

        internal DummyClient()
        {
            mSavedGameClient = new AndroidSavedGameClient();
        }
        /// <summary>
        /// Starts the authentication process.
        /// </summary>
        /// <remarks> If silent == true, no UIs will be shown
        /// (if UIs are needed, it will fail rather than show them). If silent == false,
        /// this may show UIs, consent dialogs, etc.
        /// At the end of the process, callback will be invoked to notify of the result.
        /// Once the callback returns true, the user is considered to be authenticated
        /// forever after.
        /// </remarks>
        /// <param name="callback">Callback when completed.</param>
        /// <param name="silent">If set to <c>true</c> silent.</param>
        public void Authenticate(bool silent, Action<SignInStatus> callback)
        {
            LogUsage();
            if (callback != null)
            {
                callback(SignInStatus.Failed);
            }
        }

        /// <summary>
        /// Returns whether or not user is authenticated.
        /// </summary>
        /// <returns>true if authenticated</returns>
        /// <c>false</c>
        public bool IsAuthenticated()
        {
            LogUsage();
            return false;
        }

        /// <summary>
        /// Signs the user out.
        /// </summary>
        public void SignOut()
        {
            LogUsage();
        }


        /// <summary>
        /// Retrieves an id token, which can be verified server side, if they are logged in.
        /// </summary>
        /// <returns>The identifier token.</returns>
        public string GetIdToken()
        {
            LogUsage();
            return null;
        }

        /// <summary>
        /// Returns the authenticated user's ID. Note that this value may change if a user signs
        /// on and signs in with a different account.
        /// </summary>
        /// <returns>The user identifier.</returns>
        public string GetUserId()
        {
            LogUsage();
            return "DummyID";
        }


        public string GetServerAuthCode()
        {
            LogUsage();
            return null;
        }

        public void GetAnotherServerAuthCode(bool reAuthenticateIfNeeded,
            Action<string> callback)
        {
            LogUsage();
            callback(null);
        }

        /// <summary>
        /// Returns a human readable name for the user, if they are logged in.
        /// </summary>
        /// <returns>The user display name.</returns>
        public string GetUserDisplayName()
        {
            LogUsage();
            return "Player";
        }


        /// <summary>
        /// Gets the saved game client.
        /// </summary>
        /// <returns>The saved game client.</returns>
        public SavedGame.ISavedGameClient GetSavedGameClient()
        {
            LogUsage();
            return mSavedGameClient;
        }

        /// <summary>
        /// Logs the usage.
        /// </summary>
        private static void LogUsage()
        {
            Logger.d("Received method call on DummyClient - using stub implementation.");
        }
    }
}