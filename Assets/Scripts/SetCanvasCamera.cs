using System;
using UnityEngine;

public class SetCanvasCamera : MonoBehaviour
{
    private void Awake()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }

    private void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}