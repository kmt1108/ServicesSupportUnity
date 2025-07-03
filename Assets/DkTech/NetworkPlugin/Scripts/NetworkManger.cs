using Dktech.Services.Firebase;
using System;
using Unity.VisualScripting;
#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
using UnityEngine;

namespace Dktech.Services.Advertisement
{
    public static class NetworkManager
    {
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void startListening();
        [DllImport("__Internal")]
        private static extern bool getCurrentNetworkState();
#endif
        private static bool checkInternet;
        private static bool isConnected;

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            FirebaseManager.OnFetchFailed += () =>
            {
                SetCheckInternet(FirebaseManager.GetDefaultCheckInternet());
            };
            InitNativeReceiver();
        }
        private static void InitNativeReceiver()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using (AndroidJavaClass unityPlayer = new("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                using (AndroidJavaClass plugin = new("com.dktech.networkplugin.NetworkPlugin"))
                {
                    plugin.CallStatic("Init", activity);
                    /*bool isConnected = plugin.CallStatic<bool>("getCurrentNetworkState");
                    Debug.Log("Current State: " + isConnected);*/
                }
            }
            #endif
            #if UNITY_IOS && !UNITY_EDITOR
                startListening();
                /*Debug.Log("Current State: " + getCurrentNetworkState());*/
            #endif

        }
        public static void SetCheckInternet(bool isCheck)
        {
            checkInternet = isCheck;
            if (!isCheck)
            {
                ServicesManager.instance.ShowNoInternetUI(false);
            }else if (!isConnected)
            {
                ServicesManager.instance.ShowNoInternetUI(true);

            }

        }

        // Called from native code
        public static void OnNetworkConnectedChanged(bool isConnected)
        {
            NetworkManager.isConnected = isConnected;
            if (isConnected)
            {
#if firebase_enabled
                FirebaseManager.CheckReloadRemoteConfig();
#endif
            }
            if (checkInternet)
            {
                Debug.Log("Network Status: " + isConnected);
                ServicesManager.instance.ShowNoInternetUI(!isConnected);
            }
            else
            {
                ServicesManager.instance.ShowNoInternetUI(false);
            }
        }
    }
}