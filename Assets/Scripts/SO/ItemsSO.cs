using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ItemsSO", order = 1)]
public class ItemsSO : ScriptableObject
{
    public List<Item> items = new List<Item>();
}

[Serializable]
public class Item
{
    public int id;
}