using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;


public static class FileDialog  {
    public static string OpenFolderDialog(string initPath,string dialogTitle = "选择文件夹")
    {
        return BrowseForFolder.SelectFolder(dialogTitle, initPath, IntPtr.Zero);
    }
    public static string[] OpenMutiFileDialog(AssetFileType fileType,string dialogTitle = "选择多个文件", string initPath = null)
    {
        const int MAX_FILE_LENGTH = 2048;

        OpenFileName ofn = InitDialogCommon(dialogTitle, initPath);
        GetFliter(fileType, out ofn.filter, out ofn.defExt);
        ofn.flags = (int)(
           OpenFileNameFlags.OFN_EXPLORER
           | OpenFileNameFlags.OFN_FILEMUSTEXIST
           | OpenFileNameFlags.OFN_PATHMUSTEXIST
           | OpenFileNameFlags.OFN_NOCHANGEDIR
           | OpenFileNameFlags.OFN_ALLOWMULTISELECT
           );
    
        // Initialize buffer with NULL bytes
        for (int i = 0; i < MAX_FILE_LENGTH * Marshal.SystemDefaultCharSize; i++)
        {
            Marshal.WriteByte(ofn.file, i, 0);
        }

        if (WindowDll.GetOpenFileName(ofn))
        {
            List<string> selectedFilesList = new List<string>();

            IntPtr filePointer = ofn.file;
            long pointer = (long)filePointer;
            string file = Marshal.PtrToStringAuto(filePointer);

            // Retrieve file names
            while (file.Length > 0)
            {
                selectedFilesList.Add(file);

                pointer += file.Length * Marshal.SystemDefaultCharSize + Marshal.SystemDefaultCharSize;
                filePointer = (IntPtr)pointer;
                file = Marshal.PtrToStringAuto(filePointer);
            }

            if (selectedFilesList.Count == 1)
            {
                // Only one file selected with full path
                Marshal.FreeHGlobal(ofn.file);
                return selectedFilesList.ToArray();
            }
            else
            {
                // Multiple files selected, add directory
                string[] selectedFiles = new string[selectedFilesList.Count - 1];

                for (int i = 0; i < selectedFiles.Length; i++)
                {
                    selectedFiles[i] = selectedFilesList[0];

                    if (!selectedFiles[i].EndsWith("\\"))
                    {
                        selectedFiles[i] += "\\";
                    }

                    selectedFiles[i] += selectedFilesList[i + 1];
                }

                // Return selected files
                Marshal.FreeHGlobal(ofn.file);
                return selectedFiles;
            }
        }
        else
        {
            // "Cancel" pressed
            Marshal.FreeHGlobal(ofn.file);
            return null;
        }
    }
    public static string OpenFileDialog(AssetFileType fileType, string windowTitle = "打开文件", string initPath = null)
    {
        OpenFileName ofn = InitDialogCommon(windowTitle, initPath);
        GetFliter(fileType, out ofn.filter, out ofn.defExt);
        ofn.flags = (int)(
          OpenFileNameFlags.OFN_EXPLORER
          | OpenFileNameFlags.OFN_FILEMUSTEXIST
          | OpenFileNameFlags.OFN_PATHMUSTEXIST
          | OpenFileNameFlags.OFN_NOCHANGEDIR
          );

        if (WindowDll.GetOpenFileName(ofn))
        {
            IntPtr filePointer = ofn.file;
            string file = Marshal.PtrToStringAuto(filePointer);
            return file;
        }
        return null;
    }
    public static string SaveFileDialog(AssetFileType fileType, string windowTitle = "保存文件", string initPath = null)
    {
        OpenFileName ofn = InitDialogCommon(windowTitle, initPath);
        GetFliter(fileType, out ofn.filter, out ofn.defExt);
        ofn.flags = (int)(
          OpenFileNameFlags.OFN_EXPLORER
          | OpenFileNameFlags.OFN_FILEMUSTEXIST
          | OpenFileNameFlags.OFN_PATHMUSTEXIST
          | OpenFileNameFlags.OFN_NOCHANGEDIR
          );
        if (WindowDll.GetSaveFileName(ofn))
        {
            IntPtr filePointer = ofn.file;
            string file = Marshal.PtrToStringAuto(filePointer);
            return file;
        }
        return null;
    }

    private static OpenFileName InitDialogCommon(string windowTitle, string initPath)
    {
        const int MAX_FILE_LENGTH = 2048;

        OpenFileName ofn = new global::OpenFileName();
        ofn.title = windowTitle;
        ofn.structSize = Marshal.SizeOf(ofn);
        // Create buffer for file names
        ofn.file = Marshal.AllocHGlobal(MAX_FILE_LENGTH * Marshal.SystemDefaultCharSize);
        ofn.maxFile = MAX_FILE_LENGTH;
        ofn.fileTitle = new string(new char[MAX_FILE_LENGTH]);
        ofn.maxFileTitle = ofn.fileTitle.Length;
        //string path = initPath/* == null ? Application.streamingAssetsPath : initPath*/;
        ofn.initialDir = "C:\\";//path.Replace('/', '\\');
        return ofn;
    }
    static void GetFliter(AssetFileType type,out string fliter,out string defExt)
    {
        fliter = "All Files (*.*)|*.*";
        defExt = "";
        switch (type)
        {
            case AssetFileType.Csv:
                fliter = "配制(*.csv)|*.csv";
                defExt = "csv";//显示文件的类型  
                break;
            case AssetFileType.Movie:
                fliter = "配制(*.avi*.mp4)|*.avi;*.mp4";
                defExt = "mp4";//显示文件的类型  
                break;
            case AssetFileType.Picture:
                fliter = "表格(*.png*.jpg)|*.png;*.jpg";
                defExt = "png";//显示文件的类型  
                break;
            case AssetFileType.Html:
                fliter = "网页(*.html*.htm)|*.html;*.htm";
                defExt = "html";//显示文件的类型  
                break;
            case AssetFileType.Pdf:
                fliter = "信息(*.pdf)|*.pdf";
                defExt = "pdf";//显示文件的类型  
                break;
            case AssetFileType.Excel:
                fliter = "表格(*.xls*.xlsx*.csv)|*.xls;*.xlsx;*.csv;";
                defExt = "xls";//显示文件的类型  
                break;
            case AssetFileType.Exe:
                fliter = "程序(*.exe)|*.exe;";
                defExt = "exe";//显示文件的类型  
                break;
            default:
                break;
        }
        fliter = fliter.Replace("|", "\0") + "\0";
    }
}
