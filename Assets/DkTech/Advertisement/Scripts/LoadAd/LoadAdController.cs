using System.Collections.Generic;

namespace Dktech.Services.Advertisement
{
    public static class LoadAdController
    {
        public static int MaxLoadingAd { private get; set; } = 2;
        static List<AdBase> waitingList = new();
        static List<AdBase> loadingList = new();
        public static void RequestLoadAd(this AdBase ad)
        {
            if (loadingList.Count < MaxLoadingAd)
            {
                loadingList.Add(ad);
                ad.LoadAd();
            }
            else
            {
                waitingList.Add(ad);
            }
        }
        public static void LoadAdComplete(this AdBase ad)
        {
            loadingList.Remove(ad);
            if (waitingList.Count > 0)
            {
                loadingList.Add(waitingList[0]);
                waitingList.RemoveAt(0);
                loadingList[^1].LoadAd();
            }
        }
    }
}
