using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
public class WindowSwitchCtrl : IWindowSwitchCtrl
{
    private bool hideParent;
    private IntPtr parentWindow;
    private Process childProcess;
    public IntPtr Parent
    {
        get
        {
            return parentWindow;
        }
    }

    /// <summary>
    /// 关闭指定窗口
    /// </summary>
    /// <param name="ptr"></param>
    public bool CloseChildWindow()
    {
        if (childProcess != null && !childProcess.HasExited)
        {
            childProcess.Kill();
        }

        return false;
    }
    /// <summary>
    /// 关闭所有打开的窗口
    /// </summary>
    public void OnCloseThisWindow()
    {
        CloseChildWindow();

        if (hideParent)
        {
            User32.ShowWindow(parentWindow, WindowShowStyle.Show);
        }
    }

    /// <summary>
    /// 仅当第二个参数为1时，隐藏父级
    /// </summary>
    /// <param name="pwindow"></param>
    /// <param name="hideParent"></param>
    public bool OnOpenedByParent(out string[] argus)
    {
        hideParent = false;
        parentWindow = IntPtr.Zero;
        argus = null;

        string[] CommandLineArgs = Environment.GetCommandLineArgs();

        if (CommandLineArgs.Length == 1)
        {
            //无参数
            return false;
        }

        if (CommandLineArgs.Length > 1)
        {
            parentWindow = new IntPtr(int.Parse(CommandLineArgs[1]));
        }

        if (CommandLineArgs.Length > 2)
        {
            hideParent = new IntPtr(int.Parse(CommandLineArgs[2])) == (IntPtr)1;
        }

        if (CommandLineArgs.Length > 3)
        {
            argus = new string[CommandLineArgs.Length - 3];
            for (int i = 0; i < argus.Length; i++)
            {
                argus[i] = CommandLineArgs[(i + 3)];
            }
        }

        if (hideParent)
        {
            User32.ShowWindow(parentWindow, WindowShowStyle.Hide);
        }

        return true;
    }
    /// <summary>
    /// 打开一个子窗口,并记录
    /// </summary>
    /// <param name="path"></param>
    /// <param name="hideThis"></param>
    public bool OpenChildWindow(string path, bool hideThis,params string[] args)
    {
        if (childProcess == null|| childProcess.HasExited)
        {
            if (System.IO.File.Exists(path))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(path);
                startInfo.Arguments = (WindowCalc.GetCurrProcessWnd() + " " + (hideThis ? 1 : 0).ToString()).ToString();
                for (int i = 0; i < args.Length; i++)
                {
                    startInfo.Arguments += " " + args[i];
                }
                startInfo.UseShellExecute = true;
                startInfo.WindowStyle = ProcessWindowStyle.Normal;
                childProcess = new Process();

                childProcess.StartInfo = startInfo;
                childProcess.Start();
                return true;
            }
        }
        return false;
    }
}
