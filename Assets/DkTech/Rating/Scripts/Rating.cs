#if UNITY_ANDROID && google_review_enabled
using Google.Play.Review;
#elif UNITY_IOS
using UnityEngine.iOS;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Dktech.Services
{
    public class Rating : MonoBehaviour
    {
        public static bool OutFromRate { get; set; }
        private string contactEmail;
        public void SetContactEmail(string email)
        {
            contactEmail = email;
        }
        [SerializeField] List<Image> star;
        [SerializeField] List<Button> btnStar;
        [SerializeField] Sprite sprStar, sprStarNo;

        int numStar = 0;

        // Start is called before the first frame update
        void Start()
        {
            foreach (Button bt in btnStar)
            {
                bt.onClick.AddListener(() =>
                {
                    numStar = btnStar.IndexOf(bt) + 1;
                    for (int i = 0; i < star.Count; i++)
                    {
                        if (i <= btnStar.IndexOf(bt)) star[i].sprite = sprStar;
                        else star[i].sprite = sprStarNo;
                    }
                });
            }
        }
        private void OnEnable()
        {
            numStar = 0;
            for (int i = 0; i < star.Count; i++)
            {
                star[i].sprite = sprStarNo;
            }
        }

        public void OnClickRateNow()
        {
            if (numStar != 0)
            {
                OpenUrlRate();
                gameObject.SetActive(false);
            }
        }
        void OpenUrlRate()
        {
            switch (numStar)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    SendEmail();
                    break;
                case 5:
                    if (!PlayerPrefs.HasKey("FirstRate"))
                    {
                        PlayerPrefs.SetString("FirstRate", "done");
                        RequestInAppReviews();
                    }
                    else
                    {
                        OpenStore();
                    }
                    break;
            }
        }
#if UNITY_IOS
        private static string linkStoreIOS;
#endif
        public void RequestInAppReviews()
        {
            OutFromRate = true;
            StartCoroutine(RequestReviews());
        }
        // Start is called before the first frame update

        public IEnumerator RequestReviews()
        {
            // Create instance of ReviewManager
#if UNITY_ANDROID && google_review_enabled
            ReviewManager _reviewManager;
            // ...
            _reviewManager = new ReviewManager();
            var requestFlowOperation = _reviewManager.RequestReviewFlow();
            yield return requestFlowOperation;
            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                // Log error. For example, using requestFlowOperation.Error.ToString().
                yield break;
            }
            PlayReviewInfo _playReviewInfo = requestFlowOperation.GetResult();
            var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
            yield return launchFlowOperation;
            _playReviewInfo = null; // Reset the object
            if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            {
                // Log error. For example, using requestFlowOperation.Error.ToString().
                yield break;
            }
            // The flow has finished. The API does not indicate whether the user
            // reviewed or not, or even whether the review dialog was shown. Thus, no
            // matter the result, we continue our app flow.
#elif UNITY_IOS
        Device.RequestStoreReview();
#endif
            yield return null;
        }

        public void OpenStore()
        {
            OutFromRate = true;
#if UNITY_ANDROID
            Application.OpenURL("https://play.google.com/store/apps/details?id=" + Application.identifier);
#elif UNITY_IOS
            Application.OpenURL(linkStoreIOS);
#endif
        }
        public void SendEmail()
        {
            OutFromRate = true;
            Application.OpenURL("mailto:" + contactEmail + "?subject=" + MyEscapeURL("Feedback about " + Application.productName) + "&body=" + MyEscapeURL("Tell us the problems that you are facing?"));
        }
        string MyEscapeURL(string url)
        {
            return UnityWebRequest.EscapeURL(url).Replace("+", "%20");
        }
    }
}