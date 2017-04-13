using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public delegate IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
//public class CHOOSECOLOR
//{
//    public Int32 lStructSize;
//    public Int32 hwndOwner;
//    public Int32 hInstance;
//    public Int32 rgbResult;
//    public IntPtr lpCustColors;
//    public Int32 Flags;
//    public Int32 lCustData;
//    public Int32 lpfnHook;
//    public Int32 lpTemplateName;
//}
public class CHOOSECOLOR
{
    public Int32 lStructSize = Marshal.SizeOf(typeof(CHOOSECOLOR));
    public IntPtr hwndOwner;
    public IntPtr hInstance;
    public Int32 rgbResult;
    public IntPtr lpCustColors;
    public Int32 Flags;
    public IntPtr lCustData = IntPtr.Zero;
    public WndProc lpfnHook;
    public string lpTemplateName;
}

