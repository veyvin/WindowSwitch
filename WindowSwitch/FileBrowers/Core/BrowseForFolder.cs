﻿using System.Runtime.InteropServices;
using System;
using System.Text;

public class BrowseCallBack
{
    // Constants for sending and receiving messages in BrowseCallBackProc
    public const int WM_USER = 0x400;
    public const int BFFM_INITIALIZED = 1;
    public const int BFFM_SELCHANGED = 2;
    public const int BFFM_VALIDATEFAILEDA = 3;
    public const int BFFM_VALIDATEFAILEDW = 4;
    public const int BFFM_IUNKNOWN = 5; // provides IUnknown to client. lParam: IUnknown*
    public const int BFFM_SETSTATUSTEXTA = WM_USER + 100;
    public const int BFFM_ENABLEOK = WM_USER + 101;
    public const int BFFM_SETSELECTIONA = WM_USER + 102;
    public const int BFFM_SETSELECTIONW = WM_USER + 103;
    public const int BFFM_SETSTATUSTEXTW = WM_USER + 104;
    public const int BFFM_SETOKTEXT = WM_USER + 105; // Unicode only
    public const int BFFM_SETEXPANDED = WM_USER + 106; // Unicode only
}

public class BrowsingforDir
{

    // Browsing for directory.
    public uint BIF_RETURNONLYFSDIRS = 0x0001;  // For finding a folder to start document searching
    public uint BIF_DONTGOBELOWDOMAIN = 0x0002;  // For starting the Find Computer
    public uint BIF_STATUSTEXT = 0x0004;
    // Top of the dialog has 2 lines of text for BROWSEINFO.lpszTitle and one line if
    // this flag is set.  Passing the message BFFM_SETSTATUSTEXTA to the hwnd can set the
    // rest of the text.  This is not used with BIF_USENEWUI and BROWSEINFO.lpszTitle gets
    // all three lines of text.
    public uint BIF_RETURNFSANCESTORS = 0x0008;
    public uint BIF_EDITBOX = 0x0010;   // Add an editbox to the dialog
    public uint BIF_VALIDATE = 0x0020;   // insist on valid result (or CANCEL)

    public const uint BIF_NEWDIALOGSTYLE = 0x0040;   // Use the new dialog layout with the ability to resize
                                                      // Caller needs to call OleInitialize() before using this API
    public uint BIF_USENEWUI = 0x0040 + 0x0010; //(BIF_NEWDIALOGSTYLE | BIF_EDITBOX);

    public uint BIF_BROWSEINCLUDEURLS = 0x0080;   // Allow URLs to be displayed or entered. (Requires BIF_USENEWUI)
    public uint BIF_UAHINT = 0x0100;   // Add a UA hint to the dialog, in place of the edit box. May not be combined with BIF_EDITBOX
    public uint BIF_NONEWFOLDERBUTTON = 0x0200;   // Do not add the "New Folder" button to the dialog.  Only applicable with BIF_NEWDIALOGSTYLE.
    public uint BIF_NOTRANSLATETARGETS = 0x0400;  // don't traverse target as shortcut

    public uint BIF_BROWSEFORCOMPUTER = 0x1000;  // Browsing for Computers.
    public uint BIF_BROWSEFORPRINTER = 0x2000;// Browsing for Printers
    public uint BIF_BROWSEINCLUDEFILES = 0x4000; // Browsing for Everything
    public const uint BIF_SHAREABLE = 0x8000;  // sharable resources displayed (remote shares, requires BIF_USENEWUI)

}

public class BrowseForFolder
{
   

    [DllImport("shell32.dll")]
    static extern IntPtr SHBrowseForFolder(ref BROWSEINFO lpbi);

    // Note that the BROWSEINFO object's pszDisplayName only gives you the name of the folder.
    // To get the actual path, you need to parse the returned PIDL
    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    // static extern uint SHGetPathFromIDList(IntPtr pidl, [MarshalAs(UnmanagedType.LPWStr)]
    //StringBuilder pszPath);
    static extern bool SHGetPathFromIDList(IntPtr pidl, IntPtr pszPath);

    [DllImport("user32.dll", PreserveSig = true)]
    public static extern IntPtr SendMessage(HandleRef hWnd, uint Msg, int wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, string lParam);

    private static string _initialPath;

    public delegate int BrowseCallBackProc(IntPtr hwnd, int msg, IntPtr lp, IntPtr wp);
    struct BROWSEINFO
    {
        public IntPtr hwndOwner;
        public IntPtr pidlRoot;
        public string pszDisplayName;
        public string lpszTitle;
        public uint ulFlags;
        public BrowseCallBackProc lpfn;
        public IntPtr lParam;
        public int iImage;
    }
    private static int OnBrowseEvent(IntPtr hWnd, int msg, IntPtr lp, IntPtr lpData)
    {
        switch (msg)
        {
            case BrowseCallBack.BFFM_INITIALIZED: // Required to set initialPath
                {
                    //Win32.SendMessage(new HandleRef(null, hWnd), BFFM_SETSELECTIONA, 1, lpData);
                    // Use BFFM_SETSELECTIONW if passing a Unicode string, i.e. native CLR Strings.
                    SendMessage(new HandleRef(null, hWnd), BrowseCallBack.BFFM_SETSELECTIONW, 1, _initialPath);
                    break;
                }
            case BrowseCallBack.BFFM_SELCHANGED:
                {
                    IntPtr pathPtr = Marshal.AllocHGlobal((int)(260 * Marshal.SystemDefaultCharSize));
                    if (SHGetPathFromIDList(lp, pathPtr))
                        SendMessage(new HandleRef(null, hWnd), BrowseCallBack.BFFM_SETSTATUSTEXTW, 0, pathPtr);
                    Marshal.FreeHGlobal(pathPtr);
                    break;
                }
        }

        return 0;
    }

    public static string SelectFolder(string caption, string initialPath, IntPtr parentHandle)
    {
        _initialPath = initialPath;
        StringBuilder sb = new StringBuilder(256);
        IntPtr bufferAddress = Marshal.AllocHGlobal(256); ;
        IntPtr pidl = IntPtr.Zero;
        BROWSEINFO bi = new BROWSEINFO();
        bi.hwndOwner = parentHandle;
        bi.pidlRoot = IntPtr.Zero;
        bi.lpszTitle = caption;
        bi.ulFlags = BrowsingforDir.BIF_NEWDIALOGSTYLE | BrowsingforDir.BIF_SHAREABLE;
        bi.lpfn = new BrowseCallBackProc(OnBrowseEvent);
        bi.lParam = IntPtr.Zero;
        bi.iImage = 0;

        try
        {
            pidl = SHBrowseForFolder(ref bi);
            if (true != SHGetPathFromIDList(pidl, bufferAddress))
            {
                return null;
            }
            sb.Append(Marshal.PtrToStringAuto(bufferAddress));
        }
        finally
        {
            // Caller is responsible for freeing this memory.
            Marshal.FreeCoTaskMem(pidl);
        }

        return sb.ToString();
    }
}