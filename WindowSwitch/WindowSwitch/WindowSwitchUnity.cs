using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using FormSwitch.Internal;
namespace FormSwitch
{

    public class WindowSwitchUnity : IWindowSwitchCtrl
    {
        private bool hideParent;
        private bool loadAsChild;
        private IntPtr parentWindow;
        private Process childProcess;
        public IntPtr Current
        {
            get
            {
                if (current == IntPtr.Zero)
                {
                    if (loadAsChild)
                    {
                        EnumChildWindows(parentWindow, (hwnd, lparam) => { current = hwnd; return 0; }, IntPtr.Zero);
                    }
                    else
                    {
                        Process process = Process.GetCurrentProcess();
                        current = GetProcessWnd(process);
                    }
                }
                return current;
            }
        }
        public IntPtr Parent
        {
            get
            {
                return parentWindow;
            }
        }
        public IntPtr Child
        {
            get
            {
                if (childProcess == null)
                {
                    child = IntPtr.Zero;
                }
                else if (child == IntPtr.Zero)
                {
                    child = GetProcessWnd(childProcess);
                }
                return child;
            }

        }
        private IntPtr child;
        private IntPtr current;
        /// <summary>
        /// 关闭指定窗口
        /// </summary>
        /// <param name="ptr"></param>
        public bool CloseChildWindow()
        {
            if (childProcess != null && !childProcess.HasExited)
            {
                childProcess.Kill();
                childProcess = null;
                child = IntPtr.Zero;
                return true;
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
                ShowWindow(parentWindow, WindowShowStyle.Show);
            }
        }

        /// <summary>
        ///一、 第二个参数传入-parentHWND时，第三个参数传入父级节点，其他参数
        ///二、 第二个参数传入是否隐藏父级，第三个参数传入父级节点，其他参数
        /// </summary>
        /// <param name="pwindow"></param>
        /// <param name="hideParent"></param>
        public bool OnOpenedByParent(ref string[] argus)
        {
            hideParent = false;
            parentWindow = IntPtr.Zero;
            argus = null;
            string[] CommandLineArgs = Environment.GetCommandLineArgs();
            //默认参数
            if (CommandLineArgs.Length == 1)
            {
                //无参数
                return false;
            }
            ///父级handle
            if (CommandLineArgs.Length > 1)
            {
                if (CommandLineArgs[1] == "-parentHWND")
                {
                    loadAsChild = true;
                }
                else
                {
                    hideParent = new IntPtr(int.Parse(CommandLineArgs[1])) == (IntPtr)0;
                }
            }
            ///是否隐藏了父级
            if (CommandLineArgs.Length > 2)
            {
                parentWindow = new IntPtr(int.Parse(CommandLineArgs[2]));
            }
            ///用户自定义参数
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
                ShowWindow(parentWindow, WindowShowStyle.Hide);
            }

            return true;
        }
        /// <summary>
        /// 打开一个子窗口,并记录
        /// </summary>
        /// <param name="path"></param>
        /// <param name="hideThis"></param>
        public bool OpenChildWindow(string path, bool hideThis, params string[] args)
        {
            if (childProcess == null || childProcess.HasExited)
            {
                if (System.IO.File.Exists(path))
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo(path);
                    startInfo.Arguments = ((hideThis ? 0 : 1).ToString() + " " + Current).ToString();
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

        public enum WindowShowStyle : uint
        {
            /// <summary>Hides the window and activates another window.</summary>
            /// <remarks>See SW_HIDE</remarks>
            Hide = 0,
            /// <summary>Activates and displays a window. If the window is minimized
            /// or maximized, the system restores it to its original size and
            /// position. An application should specify this flag when displaying
            /// the window for the first time.</summary>
            /// <remarks>See SW_SHOWNORMAL</remarks>
            ShowNormal = 1,
            /// <summary>Activates the window and displays it as a minimized window.</summary>
            /// <remarks>See SW_SHOWMINIMIZED</remarks>
            ShowMinimized = 2,
            /// <summary>Activates the window and displays it as a maximized window.</summary>
            /// <remarks>See SW_SHOWMAXIMIZED</remarks>
            ShowMaximized = 3,
            /// <summary>Maximizes the specified window.</summary>
            /// <remarks>See SW_MAXIMIZE</remarks>
            Maximize = 3,
            /// <summary>Displays a window in its most recent size and position.
            /// This value is similar to "ShowNormal", except the window is not
            /// actived.</summary>
            /// <remarks>See SW_SHOWNOACTIVATE</remarks>
            ShowNormalNoActivate = 4,
            /// <summary>Activates the window and displays it in its current size
            /// and position.</summary>
            /// <remarks>See SW_SHOW</remarks>
            Show = 5,
            /// <summary>Minimizes the specified window and activates the next
            /// top-level window in the Z order.</summary>
            /// <remarks>See SW_MINIMIZE</remarks>
            Minimize = 6,
            /// <summary>Displays the window as a minimized window. This value is
            /// similar to "ShowMinimized", except the window is not activated.</summary>
            /// <remarks>See SW_SHOWMINNOACTIVE</remarks>
            ShowMinNoActivate = 7,
            /// <summary>Displays the window in its current size and position. This
            /// value is similar to "Show", except the window is not activated.</summary>
            /// <remarks>See SW_SHOWNA</remarks>
            ShowNoActivate = 8,
            /// <summary>Activates and displays the window. If the window is
            /// minimized or maximized, the system restores it to its original size
            /// and position. An application should specify this flag when restoring
            /// a minimized window.</summary>
            /// <remarks>See SW_RESTORE</remarks>
            Restore = 9,
            /// <summary>Sets the show state based on the SW_ value specified in the
            /// STARTUPINFO structure passed to the CreateProcess function by the
            /// program that started the application.</summary>
            /// <remarks>See SW_SHOWDEFAULT</remarks>
            ShowDefault = 10,
            /// <summary>Windows 2000/XP: Minimizes a window, even if the thread
            /// that owns the window is hung. This flag should only be used when
            /// minimizing windows from a different thread.</summary>
            /// <remarks>See SW_FORCEMINIMIZE</remarks>
            ForceMinimized = 11
        }

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);


        public static IntPtr GetProcessWnd(Process process)
        {
            IntPtr ptrWnd = IntPtr.Zero;
            uint pid = (uint)process.Id;  // 当前进程 ID 

            bool bResult = EnumWindows(new WNDENUMPROC(delegate (IntPtr hwnd, uint lParam)
            {
                uint id = 0;

                if (GetParent(hwnd) == IntPtr.Zero)
                {
                    GetWindowThreadProcessId(hwnd, ref id);
                    if (id == lParam)    // 找到进程对应的主窗口句柄 
                    {
                        ptrWnd = hwnd;   // 把句柄缓存起来 
                        SetLastError(0);    // 设置无错误 
                        return false;   // 返回 false 以终止枚举窗口 
                    }
                }

                return true;

            }), pid);

            return (!bResult && Marshal.GetLastWin32Error() == 0) ? ptrWnd : IntPtr.Zero;
        }
        public delegate bool WNDENUMPROC(IntPtr hwnd, uint lParam);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, uint lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetParent(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref uint lpdwProcessId);

        [DllImport("kernel32.dll")]
        public static extern void SetLastError(uint dwErrCode);

        public delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);
        [DllImport("user32.dll")]
        public static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);

    }

}