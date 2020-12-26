using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CafeBazaar.UI
{
    [CustomEditor(typeof(Core.Config))]
    public class CafebazaarConfigEditor : Editor
    {
        public Texture _headerBanner;
        public override void OnInspectorGUI()
        {
            var targetObject = serializedObject;
            targetObject.Update();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Box(_headerBanner);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(targetObject.FindProperty("ActiveSDK"));

            GUI.enabled = targetObject.FindProperty("ActiveSDK").boolValue;


            EditorGUILayout.PropertyField(targetObject.FindProperty("InAppPurchase").FindPropertyRelative("RSA"));

            if (GUI.changed)
            {
                if (targetObject.FindProperty("ActiveSDK").boolValue)
                {
                    ActiveSDK();
                }
                else
                {
                    DeActiveSDK();
                }

                targetObject.ApplyModifiedProperties();
            }
        }

        public void ActiveSDK()
        {
            //Application.
           // string address = Application.persistentDataPath + "/Assets/Cafebazaar/Script/CafeSDK.aar";
            //  System.IO.File.Copy(Application.persistentDataPath)
        }
        public void DeActiveSDK()
        {

        }
    }
}
