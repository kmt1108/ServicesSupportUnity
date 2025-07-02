using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;

[InitializeOnLoad]
public class CreateImageOnDragAndDrop
{
    static CreateImageOnDragAndDrop()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemGUI;
    }

    private static void OnHierarchyWindowItemGUI(int instanceID, Rect selectionRect)
    {
        Event currentEvent = Event.current;
        // Detect mouse up
        if (currentEvent.type == EventType.DragPerform)
        {
            // Check if the mouse is over the item
            if (selectionRect.Contains(currentEvent.mousePosition))
            {
                // Get the GameObject associated with the instanceID
                GameObject clickedObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

                if (clickedObject != null)
                {
                    var selectedObjects = new List<GameObject>();

                    var parentTrf = clickedObject.transform;

                    Canvas canvas = parentTrf.GetComponentInParent<Canvas>();
                    bool instantiateAsImage = parentTrf && canvas;

                    // run through each object that was dragged in.
                    foreach (var objectRef in DragAndDrop.objectReferences)
                    {
                        // if the object is the particular asset type...
                        if (objectRef is Texture2D)
                        {
                            string path = AssetDatabase.GetAssetPath(objectRef);
                            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

                            if (textureImporter.textureType == TextureImporterType.Sprite)
                            {
                                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

                                var gameObject = new GameObject(objectRef.name);

                                if (parentTrf)
                                    gameObject.transform.SetParent(parentTrf);

                                if (instantiateAsImage)
                                {
                                    var image = gameObject.AddComponent<Image>();

                                    image.GetComponent<RectTransform>().localPosition = Vector3.zero;

                                    image.transform.localScale = Vector3.one;

                                    image.sprite = sprite;

                                    image.SetNativeSize();

                                }
                                else
                                {
                                    var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                                    spriteRenderer.sprite = sprite;
                                }

                                // add to the list of selected objects.
                                selectedObjects.Add(gameObject);
                            }
                        }
                    }
                    // we didn't drag any assets of type AssetX, so do nothing.
                    if (selectedObjects.Count == 0) return;
                    // emulate selection of newly created objects.
                    Selection.objects = selectedObjects.ToArray();

                    // make sure this call is the only one that processes the event.
                    Event.current.Use();
                }
            }
        }
    }
}
