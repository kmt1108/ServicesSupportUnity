using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Dktech.Services.Advertisement
{
    public enum ObjectScaleMode { MatchWithOrHeight, Expanded }
    public class ObjectScaler : MonoBehaviour
    {
        public Vector2 baseSize;
        public ObjectScaleMode scaleMode = ObjectScaleMode.MatchWithOrHeight;
        [Range(0f, 1f)]
        public float matchWithOrHeightValue = 0.5f;
        public float orginalScale = 1f;
        private void Awake()
        {
            HandleScale();
        }
        public void HandleScale()
        {
            Vector2 screen;
#if UNITY_EDITOR
            screen = new(Camera.main.pixelWidth, Camera.main.pixelHeight);
#else
        screen = new(Screen.width, Screen.height);
#endif
            float ratio = 1f;
            if (scaleMode == ObjectScaleMode.Expanded)
            {
                if ((screen.x / screen.y) < (baseSize.x / baseSize.y))
                {
                    ratio = (screen.x / screen.y) / (baseSize.x / baseSize.y);
                }
                transform.localScale = new(ratio * orginalScale, ratio * orginalScale, 1);
            }
            else
            {
                ratio = (screen.x / screen.y) / (baseSize.x / baseSize.y);
                transform.localScale = new((ratio + (1 - ratio) * matchWithOrHeightValue) * orginalScale, (ratio + (1 - ratio) * matchWithOrHeightValue) * orginalScale, 1);
            }
        }
    }
}
