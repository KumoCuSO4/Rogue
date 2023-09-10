using System;
using System.Collections.Generic;
using System.Linq;
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
    private Dictionary<VisualElement, object> dictionary = new Dictionary<VisualElement, object>();
    public void CreateGUI()
    {
        root = rootVisualElement;
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/ItemEditor.uxml");
        visualTree.CloneTree(root);
        
        VisualElement leftElement = root.Q("LeftPart");
        leftElement.AddManipulator(new EdgeScaleManipulator());
        VisualElement rightElement = root.Q("RightPart");
        
        ItemsSO itemsSo = SOLoader.GetSO<ItemsSO>();
        List<Item> items = itemsSo.items;

        ListView listView = leftElement.Q<ListView>("ItemList");
        listView.itemsSource = items;
        listView.makeItem = () => new Label();
        listView.bindItem = (e, i) => (e as Label).text = $"{items[i].id}. {items[i].name}";
        listView.selectionType = SelectionType.Single;
        //listView.onItemsChosen += obj => DisplayItem((Item)obj.First());
        listView.onSelectionChange += objects => DisplayItem((Item)objects.First());
        listView.RefreshItems();
        
        TextField name = rightElement.Q<TextField>("Name");
        name.RegisterCallback<InputEvent>(delegate(InputEvent evt)
        {
            ((Item)dictionary[name]).name = name.value;
            listView.RefreshItems();
        });
    }

    private void DisplayItem(Item item)
    {
        VisualElement rightElement = root.Q("RightPart");
        TextField name = rightElement.Q<TextField>("Name");
        dictionary[name] = item;
        name.value = item.name;
    }
    
    private void OnGUI()
    {
        VisualElement leftElement = root.Q("LeftPart");
        Rect resizableArea = leftElement.contentRect;
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
            Vector2 mousePosition = evt.mousePosition;
            bool clickedOnEdge = Mathf.Abs(target.contentRect.width-mousePosition.x) < m_EdgeThreshold;
            if (clickedOnEdge)
            {
                m_Dragging = true;
                m_StartMousePosition = evt.localMousePosition;
                m_StartSize = new Vector2(target.resolvedStyle.width, target.resolvedStyle.height);
                target.CaptureMouse();
                evt.StopPropagation();
            }
            
        }
    }

    private void OnMouseMove(MouseMoveEvent evt)
    {
        if (m_Dragging)
        {
            Vector2 delta = evt.localMousePosition - m_StartMousePosition;
            Vector2 newSize = m_StartSize + delta;
            newSize.x = Mathf.Max(newSize.x, 100);
            newSize.x = Mathf.Min(newSize.x, target.parent.contentRect.width-100);
            
            target.style.width = newSize.x;

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