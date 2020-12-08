using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CafeBazaar.Core.UI
{
    public class CafebazaarLoginUI : MonoBehaviour
    {
        #region Singleton
        private static CafebazaarLoginUI pointer;
        public static CafebazaarLoginUI Instacne
        {
            get
            {
                if (pointer == null)
                    pointer = FindObjectOfType<CafebazaarLoginUI>();

                if (pointer == null)
                {
                    GameObject cafebazaarObject = Instantiate(Resources.Load<GameObject>("CafebazaarUI/LoginWithBazaarUI"));
                    cafebazaarObject.name = "_CAFE_LOGIN_UI_";
                    cafebazaarObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInHierarchy;
                    pointer = cafebazaarObject.GetComponent<CafebazaarLoginUI>();
                }


                return pointer;
            }
        }


        public void Show()
        {

        }
        #endregion
    }
}
