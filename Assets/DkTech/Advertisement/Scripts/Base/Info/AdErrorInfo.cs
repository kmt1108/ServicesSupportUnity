namespace Dktech.Services.Advertisement
{
    public class AdErrorInfo : AdInfo
    {
        internal string errorMessage;
        internal int errorCode;

        public AdErrorInfo(AdNetwork adNetwork, string id, string errorMessage, int errorCode) : base(adNetwork, id)
        {
            this.errorMessage = errorMessage;
            this.errorCode = errorCode;
        }
    }
}