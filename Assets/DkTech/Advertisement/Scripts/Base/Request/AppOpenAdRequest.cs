using System;
using UnityEngine;
namespace Dktech.Services.Advertisement
{
    [Serializable]
    public class AppOpenAdRequest:AdRequestInfo
    {
        [SerializeField] internal AppOpenPosition position;
        public AppOpenAdRequest(string adName, string adRCKey, AdNetwork adNetwork, string adID, string adID2,AppOpenPosition positon,bool autoReload) : base(adName, adRCKey, adNetwork, adID, adID2,autoReload)
        {
            this.position = positon;
        }
    }
}
