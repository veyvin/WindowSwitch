using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System;
using FormSwitch;
using FormSwitch.Internal;

public class Demo : MonoBehaviour {
    public Text parentT;
    public Text childT;
    public Text thisT;
    public Button openChild;
    public Button closeChild;

    IWindowSwitchCtrl windowswitch;
    public string[] parmars;
    IntPtr parentHandle { get { return windowswitch.Parent; } }
    IntPtr childHandle { get { return windowswitch.Child; } }
    private void Awake()
    {
        windowswitch = new WindowSwitchUnity();
        if(windowswitch.OnOpenedByParent(ref parmars)) {
            //以外部程序的方式打开
            parentT.text = parentHandle.ToString();
        }
        openChild.onClick.AddListener(TryOpenChild);
        closeChild.onClick.AddListener(TryCloseChild);
    }

    void Start()
    {
        thisT.text = windowswitch.Current.ToString();
    }
   
    void TryOpenChild()
    {
        string path = FileDialog.OpenFileDialog(AssetFileType.Exe, "SelectExe", Application.dataPath);
        if (!string.IsNullOrEmpty(path)){
            if(windowswitch.OpenChildWindow(path, false))
            {
                //打开子应用程序
                StartCoroutine(DelyRegisterSender());
            }
        }
    }
    void TryCloseChild()
    {
        childT.text = null;
        windowswitch.CloseChildWindow();
    }
    IEnumerator DelyRegisterSender()
    {
        while(childHandle == IntPtr.Zero){
            yield return null;
        }
        childT.text = childHandle.ToString();
    }

    private void OnDestroy()
    {
        windowswitch.OnCloseThisWindow();
    }
}
