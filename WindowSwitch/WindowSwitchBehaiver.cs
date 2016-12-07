using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System;
public class WindowSwitchBehaiver : MonoBehaviour
{
    public Button openNext;
    public Button closeThis;
    public Text lastShow;
    public Text infoShow;
    public string[] argus;
    private IWindowSwitchCtrl windowSwitch;
    void Start()
    {
        openNext.onClick.AddListener(OnOpenNextButtonClicked);
        closeThis.onClick.AddListener(OnCloseThisButtonClicked);

        windowSwitch = new WindowSwitchCtrl();
        if (windowSwitch.OnOpenedByParent(out argus))
        {
            lastShow.text = windowSwitch.Parent.ToString();
            for (int i = 0; i < argus.Length; i++)
            {
                infoShow.text += argus[i] + "->>\n";
            }
        }
    }


    void OnOpenNextButtonClicked()
    {
        string path = FileDialog.OpenFileDialog(AssetFileType.Exe, "选择程序", Application.dataPath);
        if (!string.IsNullOrEmpty(path))
        {
            windowSwitch.OpenChildWindow(path, true,"你好下一个程序");
        }
    }

    void OnCloseThisButtonClicked()
    {
        windowSwitch.OnCloseThisThisWindow();
        Application.Quit();
    }

}
