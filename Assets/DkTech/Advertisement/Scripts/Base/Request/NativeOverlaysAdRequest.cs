using System;
using UnityEngine;
namespace Dktech.Services.Advertisement
{
    [Serializable]
    public class NativeOverlaysAdRequest:AdRequestInfo
    {
        [SerializeField]internal BannerPosition adPosition;
        [SerializeField]internal NativeStyle style;


        public NativeOverlaysAdRequest(string adName, string adRCKey, AdNetwork adNetwork, string adID, string adID2,BannerPosition positon,NativeStyle style,bool autoReload) : base(adName, adRCKey, adNetwork, adID, adID2,autoReload)
        {
            this.adPosition = positon;
            this.style = style;
        }

        public override void UpdateInfo(string[] request)
        {
            base.UpdateInfo(request);
            if (request.Length >= 4) adPosition = (BannerPosition)int.Parse(request[3]);
        }
    }
}
