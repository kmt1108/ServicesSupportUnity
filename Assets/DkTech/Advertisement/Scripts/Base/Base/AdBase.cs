using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Dktech.Services.Advertisement
{
    using static LoadAdController;
    [Serializable]
    public enum AdNetwork
    {
        None = 0
#if admob_enabled
        , Admob = 1
#endif
#if applovin_enabled
        , Applovin = 2
#endif
#if ironsource_enabled
    , IronSource = 3
#endif
    }
    [Serializable]
    public abstract class AdBase
    {
        public static Action<AdBase> OnAdLoaded { get; set; }
        [SerializeField] protected string name;
        [SerializeField] protected string id;
        [SerializeField] protected string id2;
        [SerializeField] protected AdNetwork network;
        [SerializeField] protected bool autoReload;
        protected string currentID;
        public string Name => name;
        public string AdID => currentID;
        public AdNetwork AdNetwork => network;
        public bool AutoReload => autoReload;
        protected bool HaveInternet { get { return Application.internetReachability != NetworkReachability.NotReachable; } }
        protected bool waitingDelayReload;
        protected virtual void SetAdInfo(AdRequestInfo adRequest)
        {
            this.name = adRequest.adName;
            this.id = adRequest.adID;
            this.id2 = adRequest.adID2;
            this.network = adRequest.adNetwork;
            this.autoReload = adRequest.autoReload;
            currentID = id;
        }
        public abstract void LoadAd();
        public abstract void ShowAd();
        public abstract void DestroyAd();
        public abstract bool IsAvailable();
        protected void CheckSwitchID()
        {
            if (currentID.Equals(id) && !string.IsNullOrWhiteSpace(id2))
            {
                currentID = id2;
            }
            else if (currentID.Equals(id2) && !string.IsNullOrWhiteSpace(id))
            {
                currentID = id;
            }
        }
        protected async void DelayLoadAd(int milisecond)
        {
            if (waitingDelayReload)
            {
                return;
            }
            waitingDelayReload = true;
            await Task.Delay(milisecond);
            this.RequestLoadAd();
            waitingDelayReload = false;
        }
    }
}