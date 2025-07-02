using UnityEngine;
using UnityEngine.UI;

public class ScalerUI : MonoBehaviour
{
    [SerializeField] private float baseWidth = 1080f;
    [SerializeField] private float baseHeight = 1920f;
    public float ratio;
    [SerializeField] private CanvasScaler scaler;

    float curWid, curHei;

    private void Awake()
    {
        CheckScale();
    }
    private void CheckScale()
    {
        curWid = Screen.width;
        curHei = Screen.height;
        var w = curWid / baseWidth;
        var h = curHei / baseHeight;
        ratio = w / h;
        if ((w == 1 && h > 1) || (w < 1 && h >= 1))
        {
            ratio = 0;
            //Debug.LogError(1);
        }
        else if ((h == 1 && w > 1) || (h < 1 && w >= 1))
        {
            ratio = 1;
            //Debug.LogError(2);
        }
        else if (w < 1 && h < 1)
        {
            ratio = (w <= h) ? 0 : 1;
            //Debug.LogError(3);
        }
        else
        {
            ratio = (w <= h) ? 0 : 1;
            //Debug.LogError(4);
        }
        scaler.matchWidthOrHeight = ratio;
    }

}
