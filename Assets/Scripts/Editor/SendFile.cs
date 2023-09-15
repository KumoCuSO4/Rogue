using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class SendFile : EditorWindow
{
    public FileTree data;
    public string target;

    public static void Send(FileTree fileTree, string target)
    {
        SendFile window = GetWindow<SendFile>();
        window.data = fileTree;
        window.target = target;
        window.RefreshLeft();
        //window.Show();
    }
    
    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/SendFile.uxml");
        visualTree.CloneTree(root);
        RefreshLeft();
        Button refresh = root.Q<Button>("Refresh");
        refresh.clicked += RefreshLeft;
    }
    
    public void RefreshLeft()
    {
        if (data == null) return;
        VisualElement leftElement = rootVisualElement.Q("Left");
        FileTreeNode node = data.root;
        leftElement.Clear();
        leftElement.Add(new FileTreeViewItem(node));

        Label targetLabel = rootVisualElement.Q<Label>("Target");
        targetLabel.text = target;
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

public class FileTreeViewItem : VisualElement
{
    private Label nameLabel;
    private Button expandCollapseButton;
    private Button choiceButton;
    private VisualElement InfoContainer;
    private VisualElement childrenContainer;
    private FileTreeNode node;
    public bool isExpanded;
    public enum ChoiceState
    {
        None,
        Selected,
        PartlySelected,
    }
    public ChoiceState choiceState = ChoiceState.None;
    
    public FileTreeViewItem(FileTreeNode node)
    {
        this.node = node;
        node.fileTreeViewItem = this;
        
        // 创建布局
        var itemLayout = new VisualElement();
        itemLayout.AddToClassList("tree-view-item-layout"); // 添加样式类

        InfoContainer = new VisualElement();
        InfoContainer.AddToClassList("tree-view-info-container"); // 添加样式类
        itemLayout.Add(InfoContainer);
        
        choiceButton = new Button();
        choiceButton.AddToClassList("tree-view-choice-button"); // 添加样式类
        choiceButton.text = " "; // 可根据需要更改图标
        choiceButton.clicked += OnChoiceButtonClick; // 添加点击事件处理程序
        InfoContainer.Add(choiceButton);
        
        // 创建文本元素
        nameLabel = new Label();
        nameLabel.AddToClassList("tree-view-item-label"); // 添加样式类
        InfoContainer.Add(nameLabel);
        
        /*
        // 创建展开/折叠按钮
        expandCollapseButton = new Button();
        expandCollapseButton.AddToClassList("tree-view-item-button"); // 添加样式类
        expandCollapseButton.text = "▶"; // 可根据需要更改图标
        expandCollapseButton.clicked += OnExpandCollapseButtonClick; // 添加点击事件处理程序
        itemLayout.Add(expandCollapseButton);
        */

        // 创建子节点容器
        childrenContainer = new VisualElement();
        childrenContainer.AddToClassList("tree-view-children-container"); // 添加样式类
        itemLayout.Add(childrenContainer);

        // 添加布局到 TreeViewItem
        this.Add(itemLayout);
        
        nameLabel.text = node.name;
        foreach (var child in node.children)
        {
            childrenContainer.Add(new FileTreeViewItem(child));
        }
    }

    // 展开/折叠按钮的点击事件处理程序
    private void OnExpandCollapseButtonClick()
    {
        // 处理展开/折叠逻辑，如果需要的话
    }

    private void OnChoiceButtonClick()
    {
        if (choiceState == ChoiceState.None)
        {
            SelectAll();
        }
        else
        {
            UnSelectAll();
        }
        node.parent?.fileTreeViewItem.UpdateChoiceState();
    }

    private void SelectAll()
    {
        choiceState = ChoiceState.Selected;
        foreach (var child in node.children)
        {
            FileTreeViewItem item = child.fileTreeViewItem;
            item.SelectAll();
        }
        RefreshChoiceButton();
    }

    private void UnSelectAll()
    {
        choiceState = ChoiceState.None;
        foreach (var child in node.children)
        {
            FileTreeViewItem item = child.fileTreeViewItem;
            item.UnSelectAll();
        }
        RefreshChoiceButton();
    }

    public void UpdateChoiceState()
    {
        bool hasSelected = false;
        bool hasNone = false;
        bool hasPartlySelected = false;
        foreach (var child in node.children)
        {
            FileTreeViewItem item = child.fileTreeViewItem;
            if (item.choiceState == ChoiceState.None)
            {
                hasNone = true;
            }
            else if (item.choiceState == ChoiceState.Selected)
            {
                hasSelected = true;
            }
            else if (item.choiceState == ChoiceState.PartlySelected)
            {
                hasPartlySelected = true;
            }
        }

        if (hasNone && !hasSelected && !hasPartlySelected)
        {
            choiceState = ChoiceState.None;
        }
        else if (hasSelected && !hasNone && !hasPartlySelected)
        {
            choiceState = ChoiceState.Selected;
        }
        else
        {
            choiceState = ChoiceState.PartlySelected;
        }
        RefreshChoiceButton();
        node.parent?.fileTreeViewItem.UpdateChoiceState();
    }
    
    public void RefreshChoiceButton()
    {
        if (choiceState == ChoiceState.None)
        {
            choiceButton.text = " ";
        }
        else if (choiceState == ChoiceState.Selected)
        {
            choiceButton.text = "✔";
        }
        else if (choiceState == ChoiceState.PartlySelected)
        {
            choiceButton.text = "◼";
        }
    }
}

public class FileTree
{
    public FileTreeNode root;

    public FileTree()
    {
        root = new FileTreeNode();
    }

    public void AddFile(string path)
    {
        if (path.StartsWith("Assets/"))
        {
            path = path.Remove(0, 7);
        }
        root.AddPathChild(path);
    }

    public string Print()
    {
        return root.Print();
    }
}

public class FileTreeNode
{
    public string name;
    public FileTreeNode parent;
    public List<FileTreeNode> children = new List<FileTreeNode>();
    public string path;
    public bool isDir;
    public FileTreeViewItem fileTreeViewItem;
    
    public FileTreeNode(string name = null, FileTreeNode parent = null)
    {
        this.name = name;
        this.parent = parent;
        if (parent != null)
        {
            path = parent.path + "/" + name;
        }
        else
        {
            this.name = "Assets";
            path = Application.dataPath;
        }
    }
    
    public FileTreeNode GetChild(string name)
    {
        foreach (var c in children)
        {
            if (c.name.Equals(name))
            {
                return c;
            }
        }
        return null;
    }

    public FileTreeNode AddChild(string name)
    {
        FileTreeNode c = GetChild(name);
        if (c!=null) return c;
        FileTreeNode child = new FileTreeNode(name, this);
        children.Add(child);
        return child;
    }

    public void AddPathChild(string path)
    {
        string[] split = path.Split('/', 2);
        FileTreeNode f = AddChild(split[0]);
        if (split.Length > 1)
        {
            f.AddPathChild(split[1]);
        }
    }

    public void RemovePathChild(string path)
    {
        
    }

    public string Print(int depth = 0)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(new string(' ', depth));
        stringBuilder.Append(name);
        stringBuilder.Append('\n');
        foreach (var child in children)
        {
            stringBuilder.Append(child.Print(depth+1));
        }
        return stringBuilder.ToString();
    }
}