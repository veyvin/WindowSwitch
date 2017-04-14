using System;
using System.Diagnostics;
namespace FormSwitch.Internal
{

    public interface IWindowSwitchCtrl
    {
        Process ChildProcess { get; }
        IntPtr Parent { get; }
        IntPtr Child { get; }
        IntPtr Current { get; }
        bool OnOpenedByParent(ref string[] args);
        bool OpenChildWindow(string path, bool hideThis, params string[] args);
        bool CloseChildWindow();
        void OnCloseThisWindow();
    }
}

