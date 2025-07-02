using System;
using System.Collections.Generic;

namespace Dktech.Services.Advertisement
{
    [Serializable]
    public class LoadAdGroup
    {
        public List<BannerAdRequest> listBannerOrder;
        public List<AdRequestInfo> listInterOrder;
        public List<AdRequestInfo> listRewardedOrder;
        public List<AppOpenAdRequest> listAOAOrder;
        public List<AdRequestInfo> listNativeOrder;
        public List<NativeOverlaysAdRequest> listNativeOverlayOrder;
        public int TotalAdsCount
        {
            get
            {
                return listBannerOrder.Count + listInterOrder.Count + listRewardedOrder.Count + listAOAOrder.Count + listNativeOrder.Count + listNativeOverlayOrder.Count;
            }
        }
    }
}
