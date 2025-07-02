using System;
using UnityEngine;
namespace Dktech.Services.Advertisement
{
    [Serializable]
    public class BannerAdRequest : AdRequestInfo
    {
        [SerializeField] internal BannerPosition adPosition;
        [SerializeField] internal bool isCollapsible;

        public BannerAdRequest(string adName,string adRCKey, AdNetwork adNetwork, string adID, string adID2,BannerPosition positon,bool isCollapsible,bool autoReload) : base(adName, adRCKey, adNetwork, adID, adID2,autoReload)
        {
            this.adPosition = positon;
            this.isCollapsible = isCollapsible;
        }
        public override void UpdateInfo(string[] request)
        {
            base.UpdateInfo(request);
            if (request.Length >= 4) adPosition = (BannerPosition)int.Parse(request[3]);
            if (request.Length >= 5) isCollapsible = bool.Parse(request[4]);
        }
    }
}
