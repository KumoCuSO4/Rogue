using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class SOLoader
{
    [MenuItem("Tools/Load Data")]
    public static void LoadData()
    {
        ItemsSO itemsSO = ScriptableObject.CreateInstance<ItemsSO>();
        for (int i = 0; i < 20; i++)
        {
            Item item = new Item();
            item.id = i;
            item.name = "item" + i;
            itemsSO.items.Add(item);
        }
        string path = "Assets/Resources/SO/ItemsSO.asset";
        
        AssetDatabase.CreateAsset(itemsSO, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("ScriptableObject saved to: " + path);
    }

    [MenuItem("Tools/Read Data")]
    public static void ReadData()
    {
        // 使用AssetDatabase.LoadAssetAtPath加载ScriptableObject
        string path = "Assets/Resources/SO/ItemsSO.asset"; // 替换为实际的文件路径
        ItemsSO itemsSO = AssetDatabase.LoadAssetAtPath<ItemsSO>(path);

        if (itemsSO != null)
        {
            foreach (var item in itemsSO.items)
            {
                Debug.Log(item.id);
            }
        }
        else
        {
            Debug.LogError("Failed to load ScriptableObject.");
        }
    }
    
    public static T GetSO<T>() where T : ScriptableObject
    {
        // 使用AssetDatabase.LoadAssetAtPath加载ScriptableObject
        string path = $"Assets/Resources/SO/{typeof(T)}.asset"; // 替换为实际的文件路径
        T SO = AssetDatabase.LoadAssetAtPath<T>(path);
        return SO;
    }
}
