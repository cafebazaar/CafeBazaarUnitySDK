using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CafeBazaar.Games.BasicApi
{
    public class SignInHelper
    {
        private static int True = 0;
        private static int False = 1;
        private const string PromptSignInKey = "prompt_sign_in";

        public static SignInStatus ToSignInStatus(int code)
        {
            Dictionary<int, SignInStatus> dictionary = new Dictionary<int, SignInStatus>()
            {
                {
                     -12, SignInStatus.AlreadyInProgress
                },
                {
                     0, SignInStatus.Success
                },
                {
                     4, SignInStatus.UiSignInRequired
                },
                {
                     7, SignInStatus.NetworkError
                },
                {
                     8, SignInStatus.InternalError
                },
                {
                     10, SignInStatus.DeveloperError
                },
                {
                     16, SignInStatus.Canceled
                },
                {
                     17, SignInStatus.Failed
                },
                {
                     12500, SignInStatus.Failed
                },
                {
                     12501, SignInStatus.Canceled
                },
                {
                     12502, SignInStatus.AlreadyInProgress
                },
            };

            return dictionary.ContainsKey(code) ? dictionary[code] : SignInStatus.Failed;
        }


        public static void SetPromptUiSignIn(bool value)
        {
            PlayerPrefs.SetInt(PromptSignInKey, value ? True : False);
        }


        public static bool ShouldPromptUiSignIn()
        {
            return PlayerPrefs.GetInt(PromptSignInKey, True) != False;
        }
    }
}

