using UnityEditor;
using UnityEngine;

public class ResizableEditorWindow : EditorWindow
{
    private Vector2 windowSize = new Vector2(200, 200);
    private Vector2 resizeDelta = Vector2.zero;
    private bool resizing = false;

    [MenuItem("Tools/Resizable Editor Window")]
    public static void OpenWindow()
    {
        GetWindow<ResizableEditorWindow>("Resizable Window");
    }

    private void OnGUI()
    {
        Rect resizableArea = new Rect(Vector2.zero, windowSize);
        GUILayout.BeginArea(resizableArea, GUI.skin.box);

        Rect resizeHandle = new Rect(resizableArea.width - 10, 0, 10, resizableArea.height);
        EditorGUIUtility.AddCursorRect(resizeHandle, MouseCursor.ResizeHorizontal);
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0 &&
            resizeHandle.Contains(Event.current.mousePosition))
        {
            resizing = true;
            resizeDelta = new Vector2(windowSize.x - Event.current.mousePosition.x, windowSize.y);
            Event.current.Use();
        }
        else if (Event.current.type == EventType.MouseDrag && resizing)
        {
            windowSize = new Vector2(Event.current.mousePosition.x + resizeDelta.x, resizeDelta.y);
            Repaint();
        }
        else if (Event.current.type == EventType.MouseUp && resizing)
        {
            resizing = false;
            Event.current.Use();
        }

        GUILayout.EndArea();
    }
}