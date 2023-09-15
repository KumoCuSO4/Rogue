using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[EditorWindowTitle(title = "Custom Project", icon = "Project")]
public class CustomProjectWindow : EditorWindow
{
    [MenuItem("Window/Custom Project Item GUI")]
    static void Init()
    {
        CustomProjectWindow window = GetWindow<CustomProjectWindow>();
        //window.minSize = new Vector2(100, 100);
        window.Show();
    }

    public void CreateGUI()
    {
        string[] pins = ProjectUtils.pin.ToArray();
        ListView listView = new ListView();
        listView.itemsSource = pins;
        listView.makeItem = () => new Label();
        listView.bindItem = (e, i) => (e as Label).text = $"{pins[i]}";
        listView.selectionType = SelectionType.Single;
        listView.RefreshItems();
        rootVisualElement.Add(listView);
        
        string[] guids = Selection.assetGUIDs;
        List<string> select = new List<string>();
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            Debug.Log(path);
        }
    }
}

public static class ProjectUtils
{
    //private static CustomProjectWindow window;
    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
        //window = EditorWindow.GetWindow<CustomProjectWindow>();
    }

    public static FileTree fileTree = new FileTree();
    public static HashSet<string> pin = new HashSet<string>();
    private static void OnProjectWindowItemGUI(string guid, Rect rect)
    {
        // 在Project窗口中的每个文件/文件夹上绘制一个自定义图标
        // 你可以根据文件类型或其他条件来选择不同的图标来显示
        string path = AssetDatabase.GUIDToAssetPath(guid);
        if (!string.IsNullOrEmpty(path))
        {
            if (pin.Contains(path))
            {
                Texture2D customIcon = EditorGUIUtility.IconContent("Project").image as Texture2D;
                if (customIcon != null)
                {
                    rect.x -= 18; // 偏移量，根据需要进行调整
                    GUI.DrawTexture(rect, customIcon);
                }
            }
        }
    }
    
    [MenuItem("Assets/Pin",false,10000)]
    private static void Pin()
    {
        string[] guids = Selection.assetGUIDs;
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            pin.Add(path);
            fileTree.AddFile(path);
        }

        //window.rootVisualElement.Q<ListView>().RefreshItems();
    }
    
    [MenuItem("Assets/UnPin",false,10000)]
    private static void UnPin()
    {
        string[] guids = Selection.assetGUIDs;
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            pin.Remove(path);
        }
    }
    
    [MenuItem("Assets/Send",false,10000)]
    private static void Send()
    {
        string[] guids = Selection.assetGUIDs;
        List<string> paths = new List<string>();
        foreach (var t in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(t);
            paths.Add(path);
        }
        string[] dependencies = AssetDatabase.GetDependencies(paths.ToArray());
        FileTree fileTree = new FileTree();
        foreach (var path in dependencies)
        {
            fileTree.AddFile(path);
        }
        SendFile.Send(fileTree,"1234567");
    }
}