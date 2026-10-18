# DisplayFusion Virtual Desktop Hotkeys

用 **DisplayFusion Scripted Function** + **MScholtes/VirtualDesktop CLI** 实现 Windows 11 虚拟桌面的数字键快速切换与窗口搬运：

| 热键 | 功能 |
|---|---|
| `Alt+1` ~ `Alt+4` | 切换到虚拟桌面 1~4（不存在时自动新建） |
| `Alt+Shift+1` ~ `Alt+Shift+4` | 把当前窗口传送到虚拟桌面 1~4（人留在原地） |

## 为什么这么搭

- **不用 AutoHotkey**：DisplayFusion 本来就常驻，热键统一交给它管，机器上少一个全局钩子进程。
- **不依赖第三方 DLL**：直接以子进程方式调用 VirtualDesktop CLI，绕开了 Win11 24H2 改 COM GUID 导致的 DLL 失效问题。
- **不闪黑框**：C# 里用 `UseShellExecute=false + CreateNoWindow=true` 隐藏启动 CLI。
- **防失控**：桌面数量硬上限 8 个，避免脚本出错时无限新建。

## 依赖

1. [DisplayFusion](https://www.displayfusion.com/)（Scripted Function 需要 **Pro** 版），已在运行
2. [MScholtes/VirtualDesktop](https://github.com/MScholtes/VirtualDesktop) — 到 Releases 下载预编译版

⚠️ Windows 11 24H2 必须用 `VirtualDesktop11-24H2.exe`，用错版本会崩 explorer。

## 安装

1. 把 VirtualDesktop CLI 解压到任意目录，记下 exe 完整路径
2. 打开仓库里 `VD-Switch-1.cs`，把顶部的 `CLI`、`WorkingDirectory` 常量改成你的实际路径
3. 右键 DisplayFusion 托盘图标 → **Settings → Functions → Scripted Function → Add Scripted Function**
4. Name 填 `VD Switch 1`，Language 选 **C#.net**，把改好路径的脚本整段粘进代码框
5. 点 **Edit** 设热键 → 按 `Alt+1`，OK 保存
6. 对其余 7 个脚本重复（热键对应关系见上表）

## 自定义

- 每个脚本顶部的 `TARGET` 常量（0-based）决定目标桌面，想改映射直接改数字
- `MAX_DESKTOPS = 8` 是防失控上限，可按需调整

## 核心逻辑

```csharp
// 确保目标桌面存在，不够则新建（带 guard 防失控）
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

// 切换：Alt+N
if (!EnsureDesktop(TARGET)) return;
RunCli("/q /anim:off /Switch:" + TARGET);
```

## 已验证环境

Windows 11 24H2 + DisplayFusion Pro。切换与新建逻辑用 csc 编译实跑验证过（含「已存在不重复新建」）。

## License

MIT
