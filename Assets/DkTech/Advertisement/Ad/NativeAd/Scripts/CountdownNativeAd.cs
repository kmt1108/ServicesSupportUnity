using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownNativeAd : MonoBehaviour
{
    [SerializeField] Text txtCountdown;
    [SerializeField] GameObject labelCountdown;
    [SerializeField] GameObject btnClose;
    private void OnEnable()
    {
        StartCoroutine(Countdown());
    }
    IEnumerator Countdown()
    {
        btnClose.SetActive(false);
        labelCountdown.SetActive(true);
        txtCountdown.text = "3";
        yield return new WaitForSecondsRealtime(1);
        txtCountdown.text = "2";
        yield return new WaitForSecondsRealtime(1);
        txtCountdown.text = "1";
        yield return new WaitForSecondsRealtime(1);
        btnClose.SetActive(true);
        labelCountdown.SetActive(false);
    }
}
