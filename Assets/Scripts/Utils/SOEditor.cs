using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Utils
{
    //[CustomEditor(typeof(ScriptableObject))]
    public class SOEditor : Editor
    {
        private int foldCount = 0;
        private List<bool> fold = new List<bool>();
        bool f = false;
        
        public override void OnInspectorGUI()
        {
            foldCount = 0;
            ShowItem(target, target.GetType().ToString());
            /*
            for (int i = 0; i < items.items.Count; i++)
            {
                items.items[i].id = EditorGUILayout.IntField("Item " + i, items.items[i].id);
                Debug.Log(items.items[i].id);
            }
    
            if (GUILayout.Button("Save"))
            {
                // 如果需要，在此处执行保存操作
            }
            */
        }

        private void ShowItem(object item, string name)
        {
            Type type = item.GetType();
            if (type == typeof(int))
            {
                item = EditorGUILayout.IntField(name, (int)item);
            }
            else if (type == typeof(float))
            {
                item = EditorGUILayout.FloatField(name, (float)item);
            }
            else if (type == typeof(double))
            {
                item = EditorGUILayout.DoubleField(name, (double)item);
            }
            else if (type == typeof(string))
            {
                item = EditorGUILayout.TextField(name, (string)item);
            }
            else if (type == typeof(bool))
            {
                item = EditorGUILayout.Toggle(name, (bool)item);
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                // Type elementType = type.GetGenericArguments()[0];
                // EditorGUILayout.LabelField(elementType.ToString());
                if(fold.Count<=foldCount) fold.Add(false);
                IList ilist = (IList)item;
                foldCount++;
                //EditorGUILayout.LabelField(foldCount.ToString());
                fold[foldCount-1] = EditorGUILayout.Foldout(fold[foldCount-1], name);
                if (fold[foldCount-1])
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    int i = 0;
                    foreach (var listItem in ilist)
                    {
                        ShowItem(listItem, i.ToString());
                        i++;
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            else
            {
                if(fold.Count<=foldCount) fold.Add(false);
                foldCount++;
                //EditorGUILayout.LabelField(foldCount.ToString());
                fold[foldCount-1] = EditorGUILayout.Foldout(fold[foldCount-1], name);
                if (fold[foldCount-1])
                {
                    //Debug.Log(foldCount);
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    // 获取类中的所有属性
                    FieldInfo[] fieldInfos = type.GetFields();
                    foreach (FieldInfo fieldInfo in fieldInfos)
                    {
                        object value = fieldInfo.GetValue(item);
                        ShowItem(value, fieldInfo.Name);
                    }
                    EditorGUILayout.EndVertical();
                }
                
            }
        }
    }
}