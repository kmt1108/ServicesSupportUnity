namespace Dktech.Services.Advertisement
{
    public class AdInfo
    {
        internal AdNetwork adNetwork;
        internal string id;

        public AdInfo(AdNetwork adNetwork, string id)
        {
            this.adNetwork = adNetwork;
            this.id = id;
        }
    }
}