using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]

public class OpenFileName
{
    public int structSize = 0;
    public IntPtr dlgOwner = IntPtr.Zero;
    public IntPtr instance = IntPtr.Zero;
    public String filter = null;
    public String customFilter = null;
    public int maxCustFilter = 0;
    public int filterIndex = 0;
    public IntPtr file = IntPtr.Zero;
    public int maxFile = 0;
    public String fileTitle = null;
    public int maxFileTitle = 0;
    public String initialDir = null;
    public String title = null;
    public int flags = 0;
    public short fileOffset = 0;
    public short fileExtension = 0;
    public String defExt = null;
    public IntPtr custData = IntPtr.Zero;
    public IntPtr hook = IntPtr.Zero;
    public String templateName = null;
    public IntPtr reservedPtr = IntPtr.Zero;
    public int reservedInt = 0;
    public int flagsEx = 0;
}

public enum OpenFileNameFlags
{
    OFN_EXPLORER = 0x00080000,
    OFN_FILEMUSTEXIST = 0x00001000,
    OFN_PATHMUSTEXIST = 0x00000800,
    OFN_ALLOWMULTISELECT = 0x00000200,
    OFN_NOCHANGEDIR = 0x00000008,
    OFN_FORCESHOWHIDDEN = 0x10000000,
}