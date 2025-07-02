using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjScaler : MonoBehaviour
{
    public static ObjScaler scaler;
    [SerializeField] private float baseWidth = 1080f;
    [SerializeField] private float baseHeight = 1920f;
    public float ratio, ratioH, ratioW;
    public Vector3 currentScale;
    float curWid, curHei;
   
    private void Awake()
    {
        scaler = this;
        CheckScale();
    }
    private void CheckScale()
    {
        curWid = Screen.width;
        curHei = Screen.height;
        ratioW = baseWidth / curWid;
        ratioH = baseHeight / curHei;
        
        if (baseWidth > baseHeight)
        {
            ratio = ratioW / ratioH;
        }
        else
        {
            ratio = ratioH / ratioW;
        }
        //ratio = ratio >= 1 ? 1f : ratio;
        transform.localScale = currentScale * ratio;
    }
}
