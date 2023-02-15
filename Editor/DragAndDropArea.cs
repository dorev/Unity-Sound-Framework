using System;
using UnityEngine;
using UnityEditor;

static class DragAndDropArea <ExpectedObjectType> where ExpectedObjectType : UnityEngine.Object
{
    static public void Draw(string dropAreaText, float height = 50.0f, Action<ExpectedObjectType> action = null)
    {
        Event currentEvent = Event.current;
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, height, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, dropAreaText);

        switch (currentEvent.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(currentEvent.mousePosition))
                    return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (currentEvent.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
                    {
                        if (draggedObject is ExpectedObjectType && action != null)
                        {
                            action.Invoke(draggedObject as ExpectedObjectType);
                        }
                    }
                }
                break;
        }
    }
}
