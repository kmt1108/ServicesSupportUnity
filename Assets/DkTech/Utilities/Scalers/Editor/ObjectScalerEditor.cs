using Dktech.Services.Advertisement;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectScaler),true)]
public class ObjectScalerEditor : Editor
{
    ObjectScaler objScaler;
    const int kSliderEndpointLabelsHeight = 12;
    private class Styles
    {
        public GUIContent widthContent;
        public GUIContent heightContent;
        public GUIStyle leftAlignedLabel;
        public GUIStyle rightAlignedLabel;

        public Styles()
        {
            widthContent = new GUIContent("Width");
            heightContent = new GUIContent("Height");

            leftAlignedLabel = new GUIStyle(EditorStyles.label);
            rightAlignedLabel = new GUIStyle(EditorStyles.label);
            rightAlignedLabel.alignment = TextAnchor.MiddleRight;
        }
    }
    private static Styles s_Styles;
    private void OnEnable()
    {
        if (objScaler == null) objScaler = (ObjectScaler)target;
    }
    public override void OnInspectorGUI()
    {
        if (s_Styles == null)
            s_Styles = new Styles();
        objScaler.scaleMode= (ObjectScaleMode)EditorGUILayout.EnumPopup ("Object Scale Mode", objScaler.scaleMode);
        objScaler.baseSize = EditorGUILayout.Vector2Field("Reference Resolution", objScaler.baseSize);
        if (objScaler.scaleMode == ObjectScaleMode.MatchWithOrHeight)
        {
            objScaler.matchWithOrHeightValue = EditorGUILayout.Slider("Match", objScaler.matchWithOrHeightValue, 0, 1);
            Rect r = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight + kSliderEndpointLabelsHeight);
            DualLabeledSlider(r, s_Styles.widthContent, s_Styles.heightContent);
        }
        objScaler.orginalScale = EditorGUILayout.FloatField("Orginnal Scale", objScaler.orginalScale);
        objScaler.HandleScale();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(objScaler);
        }
    }
    private static void DualLabeledSlider(Rect position, GUIContent labelLeft, GUIContent labelRight)
    {
        position.height = EditorGUIUtility.singleLineHeight;
        position.xMin += EditorGUIUtility.labelWidth;
        position.xMax -= EditorGUIUtility.fieldWidth;
        position.y -= 5;
        GUI.Label(position, labelLeft, s_Styles.leftAlignedLabel);
        GUI.Label(position, labelRight, s_Styles.rightAlignedLabel);
    }
}
