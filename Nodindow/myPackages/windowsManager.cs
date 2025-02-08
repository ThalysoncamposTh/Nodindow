using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using static Nodindow.myPackages.windowsManager;

namespace Nodindow.myPackages
{
    static public class windowsManager
    {
        // Lista estática para armazenar as informações das janelas
        static public List<WindowInfo> windowsInfo;

        // Dicionário para armazenar identificadores únicos para cada handle de janela
        static private Dictionary<IntPtr, Guid> windowHandlesToGuid = new Dictionary<IntPtr, Guid>();

        // Método estático para carregar ou recarregar as informações das janelas
        static public void ReloadWindowsInfo()
        {
            windowsInfo = GetAllWindowsInfo();
        }
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetOwner(IntPtr hWnd, IntPtr hWndNewOwner);
        // Importações das funções do Windows API
        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr GetWindow(IntPtr hWnd, int wCmd);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("user32.dll")]
        private static extern bool EnumThreadWindows(uint dwThreadId, EnumThreadWndProc lpfn, IntPtr lParam);

        private delegate bool EnumThreadWndProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_FRAMECHANGED = 0x0020;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        // Definição da estrutura RECT para representar as dimensões de uma janela
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        const int GW_CHILD = 5;
        const int GW_HWNDNEXT = 2;

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        const int GWL_EXSTYLE = -20;
        const int WS_EX_LAYERED = 0x00080000;
        const int WS_EX_TRANSPARENT = 0x00000020;
        const int LWA_ALPHA = 0x00000002;

        const int WS_EX_TOOLWINDOW = 0x00000080;
        const int WS_EX_APPWINDOW = 0x00040000;

        const int GWL_STYLE = -16;  // Índice para obter o estilo da janela
        const int WS_MINIMIZED = 0x20000000;  // Estilo para janela minimizada

        static string GetWindowTitle(IntPtr hWnd)
        {
            const int nMaxCount = 256;
            StringBuilder sb = new StringBuilder(nMaxCount);
            if (GetWindowText(hWnd, sb, nMaxCount) > 0)
            {
                return sb.ToString();
            }
            return string.Empty;
        }
        static public List<IntPtr> getAllWindowHandlesAssociatedThisApplication()
        {
            List<IntPtr> handles = new List<IntPtr>();

            // Obter todas as threads do processo atual
            foreach (ProcessThread thread in Process.GetCurrentProcess().Threads)
            {
                uint threadId = (uint)thread.Id;

                // Enumerar todas as janelas associadas a esta thread
                EnumThreadWindows(threadId, (hWnd, lParam) =>
                {
                    // Validar se a janela pertence a este processo
                    GetWindowThreadProcessId(hWnd, out uint processId);
                    if (processId == (uint)Process.GetCurrentProcess().Id)
                    {
                        handles.Add(hWnd);
                    }
                    return true; // Continuar a enumeração
                }, IntPtr.Zero);
            }

            return handles;
        }
        static public List<WindowInfo> GetAllWindowsInfo()
        {
            List<WindowInfo> windowsInfo = new List<WindowInfo>();

            EnumWindowsProc enumProc = (hWnd, lParam) =>
            {
                RECT rect;
                GetWindowRect(hWnd, out rect);

                Guid uniqueId = GetOrCreateWindowGuid(hWnd);
                WindowInfo windowInfo = new WindowInfo(hWnd, uniqueId);
                windowInfo.Position = new WindowPosition(rect.Left, rect.Top);
                windowInfo.Size = new WindowSize(rect.Right - rect.Left, rect.Bottom - rect.Top);
                windowInfo.IsVisible = IsWindowVisible(hWnd);
                windowInfo.Title = GetWindowTitle(hWnd);

                IntPtr childWindowHandle = GetWindow(hWnd, GW_CHILD);
                while (childWindowHandle != IntPtr.Zero)
                {
                    GetWindowRect(childWindowHandle, out rect);
                    Guid childUniqueId = GetOrCreateWindowGuid(childWindowHandle);
                    WindowInfo childWindowInfo = new WindowInfo(childWindowHandle, childUniqueId);
                    childWindowInfo.Position = new WindowPosition(rect.Left, rect.Top);
                    childWindowInfo.Size = new WindowSize(rect.Right - rect.Left, rect.Bottom - rect.Top);
                    childWindowInfo.IsVisible = IsWindowVisible(childWindowHandle);
                    childWindowInfo.Title = GetWindowTitle(childWindowHandle);
                    windowInfo.ChildWindows.Add(childWindowInfo);

                    childWindowHandle = GetWindow(childWindowHandle, GW_HWNDNEXT);
                }

                windowsInfo.Add(windowInfo);
                return true;
            };

            if (!EnumWindows(enumProc, IntPtr.Zero))
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }

            return windowsInfo;
        }

        public static bool IsProcessActive(int processId)
        {
            try
            {
                Process.GetProcessById(processId);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
        public static bool IsWindowMinimized(IntPtr hWnd)
        {
            int style = GetWindowLong(hWnd, GWL_STYLE);
            return (style & WS_MINIMIZED) != 0;
        }

        static private Guid GetOrCreateWindowGuid(IntPtr hWnd)
        {
            if (!windowHandlesToGuid.TryGetValue(hWnd, out Guid guid))
            {
                guid = Guid.NewGuid();
                windowHandlesToGuid[hWnd] = guid;
            }
            return guid;
        }
        static public void RedrawWindow(IntPtr hWnd)
        {
            // Force the window to redraw
            if (IsWindowVisible(hWnd))
            {
                bool result = SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
                if (!result)
                {
                    throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), "Failed to redraw the window.");
                }
            }
        }

        public class WindowInfo
        {
            public IntPtr Handle { get; private set; }
            public Guid UniqueId { get; private set; }
            public WindowPosition Position { get; set; }
            public WindowSize Size { get; set; }
            public bool IsVisible { get; set; }
            public string Title { get; set; }
            public uint ProcessId { get; private set; } // Campo para armazenar o PID
            public bool isWindowMinimized { get; set; }
            public List<WindowInfo> ChildWindows { get; set; }
            public initialValues InitialValues { get; set; }

            public WindowInfo(IntPtr handle, Guid uniqueId)
            {
                Handle = handle;
                InitialValues = new initialValues(handle);
                UniqueId = uniqueId;
                ChildWindows = new List<WindowInfo>();
                updateWindowInfo();
            }

            public void moveWindow(int x, int y)
            {
                if (Handle != IntPtr.Zero)
                {
                    MoveWindow(Handle, x, y, Size.Width, Size.Height, true);
                    updateWindowInfo();
                }
            }

            public void resizeWindow(int width, int height)
            {
                if (Handle != IntPtr.Zero)
                {
                    MoveWindow(Handle, Position.X, Position.Y, width, height, true);
                    updateWindowInfo();
                }
            }

            public void hideWindow()
            {
                if (Handle != IntPtr.Zero)
                {
                    ShowWindow(Handle, SW_HIDE);
                    updateWindowInfo();
                }
            }

            public void showWindow()
            {
                if (Handle != IntPtr.Zero)
                {
                    ShowWindow(Handle, SW_SHOW);
                    updateWindowInfo();
                    RedrawWindow(Handle);
                }
            }

            public void bringFrontWindow()
            {
                if (Handle != IntPtr.Zero)
                {
                    SetForegroundWindow(Handle);
                    updateWindowInfo();
                }
            }

            public void setWindowOpacity(byte opacity)
            {
                if (Handle != IntPtr.Zero)
                {
                    int styles = GetWindowLong(Handle, GWL_EXSTYLE);
                    SetWindowLong(Handle, GWL_EXSTYLE, styles | WS_EX_LAYERED);
                    SetLayeredWindowAttributes(Handle, 0, opacity, LWA_ALPHA);
                    updateWindowInfo();
                }
            }

            public void setClickThrough(bool enabled)
            {
                if (Handle != IntPtr.Zero)
                {
                    int styles = GetWindowLong(Handle, GWL_EXSTYLE);
                    if (enabled)
                    {
                        SetWindowLong(Handle, GWL_EXSTYLE, styles | WS_EX_LAYERED | WS_EX_TRANSPARENT);
                    }
                    else
                    {
                        SetWindowLong(Handle, GWL_EXSTYLE, styles & ~WS_EX_TRANSPARENT);
                    }
                    updateWindowInfo();
                }
            }

            public bool isWindow()
            {
                return Handle != IntPtr.Zero && IsWindow(Handle);
            }

            public void HideFromTaskbar()
            {
                if (Handle != IntPtr.Zero)
                {
                    // Ocultar a janela
                    ShowWindow(Handle, SW_HIDE);

                    // Remover WS_EX_APPWINDOW e adicionar WS_EX_TOOLWINDOW
                    int exStyle = GetWindowLong(Handle, GWL_EXSTYLE);
                    exStyle &= ~WS_EX_APPWINDOW; // Remove o estilo de aplicativo
                    exStyle |= WS_EX_TOOLWINDOW;  // Adiciona o estilo de ferramenta
                    SetWindowLong(Handle, GWL_EXSTYLE, exStyle);
                }
            }

            public void ShowInTaskbar()
            {
                if (Handle != IntPtr.Zero)
                {
                    int styles = GetWindowLong(Handle, GWL_EXSTYLE);
                    SetWindowLong(Handle, GWL_EXSTYLE, styles & ~WS_EX_TOOLWINDOW);
                    updateWindowInfo();
                }
            }
            public bool getVisible()
            {
                return IsWindowVisible(Handle);
            }
            /// <summary>
            /// muito utilizado para sincronizar as atualizações das janelas para uso posterior em minha aplicação
            /// </summary>
            public void updateWindowInfo()
            {
                if (Handle == IntPtr.Zero || !IsWindow(Handle))
                {
                    Console.WriteLine("Invalid handle or window does not exist.");
                    return;
                }

                RECT rect;
                GetWindowRect(Handle, out rect);
                Position = new WindowPosition(rect.Left, rect.Top);
                Size = new WindowSize(rect.Right - rect.Left, rect.Bottom - rect.Top);
                IsVisible = IsWindowVisible(Handle);
                Title = GetWindowTitle(Handle);
                isWindowMinimized = IsWindowMinimized(Handle);
                // Obter o PID do processo associado a esta janela
                GetWindowThreadProcessId(Handle, out uint processId);
                ProcessId = processId; // Armazenar o PID
            }
        }

        public class WindowPosition
        {
            public int X { get; set; }
            public int Y { get; set; }

            public WindowPosition(int x, int y)
            {
                X = x;
                Y = y;
            }

            public override string ToString()
            {
                return $"({X}, {Y})";
            }
        }

        public class WindowSize
        {
            public int Width { get; set; }
            public int Height { get; set; }

            public WindowSize(int width, int height)
            {
                Width = width;
                Height = height;
            }

            public override string ToString()
            {
                return $"{Width}x{Height}";
            }
        }
        public class initialValues
        {
            public bool isVisible { get; set; }
            public IntPtr handle { get; set; }
            public initialValues(IntPtr handle)
            {
                this.handle = handle;
                if (handle != IntPtr.Zero)
                {
                    this.isVisible = IsWindowVisible(handle);
                }
            }
            public void setInitialValues()
            {
                if (handle != IntPtr.Zero && IsWindow(handle))
                {
                    if (isVisible)
                    {
                        ShowWindow(handle, SW_SHOW);
                    }
                    else
                    {
                        ShowWindow(handle, SW_HIDE);
                    }
                    RedrawWindow(handle);
                }
            }
        }
    }
}