using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGScaler : MonoBehaviour
{
    float curWid, curHei;
    [SerializeField] Transform tr;
    [SerializeField] SpriteRenderer sp;
    private void Start()
    {
        CheckScale();
    }
    void CheckScale()
    {
        curWid = Screen.width;
        curHei = Screen.height;
        Vector3 tempscale = tr.localScale;
        float height = sp.bounds.size.y;
        float width = sp.bounds.size.x;

        float worldHeight = Camera.main.orthographicSize * 2f;
        float worldWidth = worldHeight * curWid / curHei;

        tempscale.y = worldHeight / height;
        tempscale.x = worldWidth / width;
        transform.localScale = tempscale;
    }
}
