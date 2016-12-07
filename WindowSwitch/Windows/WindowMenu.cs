using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


public class WindowMenu : MonoBehaviour {
    void OnGUI()
    {
        if (GUILayout.Button("最小化"))
        {
            User32.ShowWindow(WindowCalc.GetCurrProcessWnd(), WindowShowStyle.Minimize);
        }
        if (GUILayout.Button("最大化"))
        {
            User32.ShowWindow(WindowCalc.GetCurrProcessWnd(), WindowShowStyle.Maximize);
        }
        if (GUILayout.Button("普通"))
        {
            User32.ShowWindow(WindowCalc.GetCurrProcessWnd(), WindowShowStyle.ShowDefault);
        }
        //if (GUILayout.Button("隐藏"))
        //{
        //    User32.ShowWindow(WindowCalc.GetCurrProcessWnd(), WindowShowStyle.Hide);
        //}
    }
}
