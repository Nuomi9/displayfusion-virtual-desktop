using System;
using System.Drawing;

// DisplayFusion Scripted Function: 当前窗口占屏幕右侧 2/3（全高）
// 与 VD-Left2Third.cs 配对使用（左 2/3 + 右 2/3 可拼出「左右各取 2/3 重叠对比」布局）
// 建议热键：Alt+D（或其他不与已有热键冲突的组合）
// 注意：仅使用官方确认存在的 BFS API；最大化窗口请先手动还原再按热键

public static class DisplayFusionFunction
{
    // 右侧占比：2/3
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
            work.X + work.Width - w,   // 右侧起点 = 工作区右缘往左一个宽度
            work.Y,
            w, work.Height);
    }
}
