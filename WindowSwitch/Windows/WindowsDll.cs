using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static class WindowDll
{
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);

    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetSaveFileName([In, Out] OpenFileName ofn);

    [DllImport("comdlg32.dll", CharSet = CharSet.Auto)]
    public static extern bool ChooseColorA([In, Out] CHOOSECOLOR pChoosecolor);//对应的win32API

    [DllImport("user32.dll", SetLastError = true, CharSet= CharSet.Auto)]
    public static extern int MessageBoxW(int hWnd, String text, String caption, uint type);
}
