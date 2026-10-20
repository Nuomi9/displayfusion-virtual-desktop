using System;
using System.Drawing;

// DisplayFusion Scripted Function: 当前窗口占屏幕左侧 2/3（全高）
// API 用法参考官方论坛确认的 BFS.Window.SetSizeAndLocation
// 建议热键：Alt+W（或其他不与已有热键冲突的组合）
// 注意：仅使用官方确认存在的 BFS API；最大化窗口请先手动还原再按热键

public static class DisplayFusionFunction
{
    // 左侧占比：2/3
    private const int NUM = 2;
    private const int DEN = 3;

    public static void Run(IntPtr windowHandle)
    {
        if (windowHandle == IntPtr.Zero) return;

        // 取窗口当前所在显示器的工作区（自动排除任务栏）
        Rectangle work = BFS.Monitor.GetMonitorWorkAreaByWindow(windowHandle);

        int w = work.Width * NUM / DEN;

        BFS.Window.SetSizeAndLocation(
            windowHandle,
            work.X, work.Y,
            w, work.Height);
    }
}
