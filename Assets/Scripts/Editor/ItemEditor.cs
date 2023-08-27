using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Cursor = UnityEngine.Cursor;


public class ItemEditor : EditorWindow
{
    [MenuItem("Tools/Item Editor")]
    public static void ShowExample()
    {
        ItemEditor wnd = GetWindow<ItemEditor>();
        wnd.titleContent = new GUIContent("ItemEditor");
    }

    private VisualElement root;
    public void CreateGUI()
    {
        root = rootVisualElement;
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/ItemEditor.uxml");
        visualTree.CloneTree(root);
        
        VisualElement resizableElement = root.Q("LeftPart");
        resizableElement.AddManipulator(new EdgeScaleManipulator());
    }

    private void OnGUI()
    {
        VisualElement resizableElement = root.Q("LeftPart");
        Rect resizableArea = resizableElement.contentRect;
        Rect resizeHandle = new Rect(resizableArea.width - 10, 0, 10, resizableArea.height);
        EditorGUIUtility.AddCursorRect(resizeHandle, MouseCursor.ResizeHorizontal);
    }

}

public class EdgeScaleManipulator : MouseManipulator
{
    private bool m_Dragging;
    private Vector2 m_StartMousePosition;
    private Vector2 m_StartSize;
    private float m_EdgeThreshold = 10;

    public EdgeScaleManipulator()
    {
        m_Dragging = false;
        activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
        target.RegisterCallback<MouseUpEvent>(OnMouseUp);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
        target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
    }

    private void OnMouseDown(MouseDownEvent evt)
    {
        if (CanStartManipulation(evt))
        {
            m_Dragging = true;
            m_StartMousePosition = evt.localMousePosition;
            m_StartSize = new Vector2(target.resolvedStyle.width, target.resolvedStyle.height);
            target.CaptureMouse();
            evt.StopPropagation();
        }
    }

    private void OnMouseMove(MouseMoveEvent evt)
    {
        if (m_Dragging)
        {
            Vector2 delta = evt.localMousePosition - m_StartMousePosition;
            Vector2 newSize = m_StartSize + delta;
            newSize.x = Mathf.Max(newSize.x, 100);
            newSize.y = Mathf.Max(newSize.y, 100);
            
            bool clickedOnEdge = Mathf.Abs(delta.x) > m_EdgeThreshold || Mathf.Abs(delta.y) > m_EdgeThreshold;

            if (clickedOnEdge)
            {
                target.style.width = newSize.x;
                // target.style.height = newSize.y;
            }

            evt.StopPropagation();
        }
    }

    private void OnMouseUp(MouseUpEvent evt)
    {
        if (m_Dragging && CanStopManipulation(evt))
        {
            m_Dragging = false;
            target.ReleaseMouse();
            evt.StopPropagation();
        }
    }
}