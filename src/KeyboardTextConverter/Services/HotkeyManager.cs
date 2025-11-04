using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KeyboardTextConverter;

/// <summary>
/// Manages global hotkey registration and events.
/// Allows the application to listen for Ctrl+Shift+Space regardless of active application.
/// </summary>
public class HotkeyManager : IDisposable
{
    private const int WM_HOTKEY = 0x0312;
    private const int HOTKEY_ID = 9000;

    private IntPtr _windowHandle;
    private bool _isRegistered = false;

    public event EventHandler? HotkeyPressed;

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr CreateWindowEx(
        uint dwExStyle,
        string lpClassName,
        string lpWindowName,
        uint dwStyle,
        int x,
        int y,
        int nWidth,
        int nHeight,
        IntPtr hWndParent,
        IntPtr hMenu,
        IntPtr hInstance,
        IntPtr lpParam);

    [DllImport("user32.dll")]
    private static extern bool DestroyWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetModuleHandle(string? lpModuleName);

    private const uint MOD_CONTROL = 0x0002;
    private const uint MOD_SHIFT = 0x0004;
    private const uint VK_SPACE = 0x20;

    public HotkeyManager()
    {
        CreateHiddenWindow();
    }

    /// <summary>
    /// Creates a hidden window to receive hotkey messages.
    /// </summary>
    private void CreateHiddenWindow()
    {
        try
        {
            // Create a hidden window that will receive hotkey messages
            var hInstance = GetModuleHandle(null);

            // Register window class
            var wndClass = new WNDCLASS
            {
                lpszClassName = "HotkeyListenerClass",
                lpfnWndProc = WndProc
            };

            if (RegisterWindowClass(ref wndClass) == 0)
            {
                throw new Exception("Failed to register window class");
            }

            // Create hidden window
            _windowHandle = CreateWindowEx(
                0,
                "HotkeyListenerClass",
                "HotkeyListener",
                0,
                0, 0, 0, 0,
                IntPtr.Zero,
                IntPtr.Zero,
                hInstance,
                IntPtr.Zero
            );

            if (_windowHandle == IntPtr.Zero)
            {
                throw new Exception("Failed to create hidden window");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to initialize hotkey listener window", ex);
        }
    }

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern ushort RegisterClass(ref WNDCLASS lpWndClass);

    private ushort RegisterWindowClass(ref WNDCLASS wndClass)
    {
        return RegisterClass(ref wndClass);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct WNDCLASS
    {
        public uint style;
        public IntPtr lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        public string? lpszMenuName;
        public string lpszClassName;
    }

    public delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    /// <summary>
    /// Window procedure to handle hotkey messages.
    /// </summary>
    private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        if (msg == WM_HOTKEY && wParam.ToInt32() == HOTKEY_ID)
        {
            HotkeyPressed?.Invoke(this, EventArgs.Empty);
            return IntPtr.Zero;
        }

        return DefWindowProc(hWnd, msg, wParam, lParam);
    }

    /// <summary>
    /// Register the global Ctrl+Shift+Space hotkey.
    /// </summary>
    public void Register()
    {
        if (_isRegistered) return;

        if (!RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_SHIFT, VK_SPACE))
        {
            throw new InvalidOperationException(
                "Failed to register hotkey. It may already be in use by another application."
            );
        }

        _isRegistered = true;
    }

    /// <summary>
    /// Unregister the global hotkey.
    /// </summary>
    public void Unregister()
    {
        if (!_isRegistered) return;

        UnregisterHotKey(_windowHandle, HOTKEY_ID);
        _isRegistered = false;
    }

    /// <summary>
    /// Start listening for the hotkey.
    /// Must be called from the main message loop.
    /// </summary>
    public void StartListening()
    {
        Register();
        // Message pump runs in Program.Main
    }

    public void Dispose()
    {
        Unregister();

        if (_windowHandle != IntPtr.Zero)
        {
            DestroyWindow(_windowHandle);
            _windowHandle = IntPtr.Zero;
        }

        GC.SuppressFinalize(this);
    }
}
