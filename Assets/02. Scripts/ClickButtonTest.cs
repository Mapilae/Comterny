using UnityEngine;
using System;
using static DebugLogUsingTest;

public class ClickButtonTest : MonoBehaviour
{
    DebugLogUsingTest debug = new DebugLogUsingTest();
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        debug.UsingDebugLog();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
