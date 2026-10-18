using System;
using System.Diagnostics;
using System.Threading;

// DisplayFusion Scripted Function: VD Switch 2
// 切换到虚拟桌面 2（不存在时自动新建）
// 依赖: F:\agent_file\VirtualDesktop\VirtualDesktop11-24H2.exe
// 直接调用 VirtualDesktop CLI，不使用任何第三方 DLL（兼容 Win11 24H2）

public static class DisplayFusionFunction
{
    private const string CLI = @"F:\agent_file\VirtualDesktop\VirtualDesktop11-24H2.exe";
    private const int TARGET = 1;            // 0-based 目标桌面
    private const int MAX_DESKTOPS = 8;          // 安全上限，防止失控新建

    // 隐藏窗口运行 CLI，返回退出码
    private static int RunCli(string args)
    {
        try
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = CLI;
            psi.Arguments = args;
            psi.UseShellExecute = false;      // 不经过 shell，避免闪黑框
            psi.CreateNoWindow = true;        // 不创建控制台窗口
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.WorkingDirectory = @"F:\agent_file\VirtualDesktop";
            using (Process p = Process.Start(psi))
            {
                if (p == null) return -1;
                p.WaitForExit(5000);
                return p.ExitCode;
            }
        }
        catch
        {
            return -1;
        }
    }

    // /Count 的退出码就是虚拟桌面数量
    private static int GetCount()
    {
        int n = RunCli("/q /Count");
        return (n >= 1) ? n : 1;
    }

    // 确保目标桌面存在，不够则新建
    private static bool EnsureDesktop(int idx)
    {
        if (idx < 0 || idx >= MAX_DESKTOPS) return false;
        int guard = 0;
        while (GetCount() <= idx && guard < MAX_DESKTOPS + 2)
        {
            RunCli("/q /anim:off /New");
            Thread.Sleep(150);
            guard++;
        }
        return GetCount() > idx;
    }

    public static void Run(IntPtr windowHandle)
    {
        if (!EnsureDesktop(TARGET)) return;
        RunCli("/q /anim:off /Switch:" + TARGET);
    }
}
