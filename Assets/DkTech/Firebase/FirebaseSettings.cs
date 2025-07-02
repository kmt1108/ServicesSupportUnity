using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Dktech.Services.Firebase
{
    public class FirebaseSettings : ScriptableObject
    {
        [SerializeField] bool getTokenFirebase;
        [SerializeField] RemoteConfigRequest configTimeWaitingInter = new() {name="TIME_FOR_INTER",key="TIME_FOR_INTER",value="30" };
        [SerializeField] RemoteConfigRequest configCheckInternet = new() {name="CHECK_INTERNET",key="CHECK_INTERNET",value="true" };
        [SerializeField] RemoteConfigRequest configCheckUpdate = new() {name="CHECK_UPDATE",key="CHECK_UPDATE",value="true" };
        [SerializeField] public List<RemoteConfigRequest> listRequest;
        private const string FirebaseSettingsResDir = "Assets/DkTech/Firebase/Resources";
        private const string FirebaseSettingsFile = "FirebaseSettings";
        private const string FirebaseSettingsFileExtension = ".asset";
#if UNITY_EDITOR
        public static FirebaseSettings LoadInstance()
        {
            //Read from resources.
            var instance = Resources.Load<FirebaseSettings>(FirebaseSettingsFile);
            //Create instance if null.
            if (instance == null)
            {
                System.IO.Directory.CreateDirectory(FirebaseSettingsResDir);
                instance = CreateInstance<FirebaseSettings>();
                string assetPath = System.IO.Path.Combine(FirebaseSettingsResDir,FirebaseSettingsFile + FirebaseSettingsFileExtension);
                AssetDatabase.CreateAsset(instance, assetPath);
                AssetDatabase.SaveAssets();
            }

            return instance;
        }
#endif
        public bool GetTokenFirebase
        {
            get => getTokenFirebase;
            set => getTokenFirebase = value;
        }
        public RemoteConfigRequest ConfigTimeWaitingInter
        {
            get => configTimeWaitingInter;
            set => configTimeWaitingInter = value;
        }
        public RemoteConfigRequest ConfigCheckInternet
        {
            get => configCheckInternet;
            set => configCheckInternet = value;
        }
        public RemoteConfigRequest ConfigCheckUpdate
        {
            get => configCheckUpdate;
            set => configCheckUpdate = value;
        }
        public List<RemoteConfigRequest> ListRequest
        {
            get => listRequest;
            set => listRequest = value;
        }
    }
}
