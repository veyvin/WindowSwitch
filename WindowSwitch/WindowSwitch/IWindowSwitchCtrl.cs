using System;
namespace FormSwitch.Internal
{

    public interface IWindowSwitchCtrl
    {
        IntPtr Parent { get; }
        IntPtr Child { get; }
        IntPtr Current { get; }
        bool OnOpenedByParent(ref string[] args);
        bool OpenChildWindow(string path, bool hideThis, params string[] args);
        bool CloseChildWindow();
        void OnCloseThisWindow();
    }
}

