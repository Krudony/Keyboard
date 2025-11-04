using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KeyboardTextConverter;

/// <summary>
/// Centralized window management for the application.
/// Manages the main hidden window for hotkey listening and System Tray integration.
/// This solves the Application Lifecycle issue by providing a single window handle
/// that both HotkeyManager and future System Tray components can use.
/// </summary>
public class WindowManager : IDisposable
{
    private IntPtr _mainWindowHandle;
    private bool _isInitialized = false;
    private bool _disposed = false;

    public IntPtr MainWindowHandle => _mainWindowHandle;

    // P/Invoke declarations for window creation
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

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern ushort RegisterClass(ref WNDCLASS lpWndClass);

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

    public WindowManager()
    {
        InitializeWindow();
    }

    /// <summary>
    /// Initialize the main application window for hotkey and System Tray.
    /// </summary>
    private void InitializeWindow()
    {
        if (_isInitialized) return;

        try
        {
            var hInstance = GetModuleHandle(null);

            // Register window class
            var wndClass = new WNDCLASS
            {
                lpszClassName = "KeyboardConverterMainWindow",
                lpfnWndProc = WndProc
            };

            if (RegisterClass(ref wndClass) == 0)
            {
                throw new Exception("Failed to register window class");
            }

            // Create main window (hidden)
            _mainWindowHandle = CreateWindowEx(
                0,
                "KeyboardConverterMainWindow",
                "KeyboardConverter",
                0,
                0, 0, 0, 0,
                IntPtr.Zero,
                IntPtr.Zero,
                hInstance,
                IntPtr.Zero
            );

            if (_mainWindowHandle == IntPtr.Zero)
            {
                throw new Exception("Failed to create main window");
            }

            _isInitialized = true;
            Console.WriteLine($"WindowManager initialized. Main window handle: {_mainWindowHandle}");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to initialize WindowManager", ex);
        }
    }

    /// <summary>
    /// Window procedure for handling messages.
    /// Can be extended to handle System Tray events, settings window messages, etc.
    /// </summary>
    private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        // This is the central message pump for the application
        // Future phases can add handling for:
        // - WM_TRAY_ICON (System Tray events)
        // - WM_SETTINGS_CHANGED (Settings dialog events)
        // - WM_CUSTOM_EVENTS (Other component events)

        return DefWindowProc(hWnd, msg, wParam, lParam);
    }

    public void Dispose()
    {
        if (_disposed) return;

        try
        {
            if (_mainWindowHandle != IntPtr.Zero)
            {
                DestroyWindow(_mainWindowHandle);
                _mainWindowHandle = IntPtr.Zero;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error disposing WindowManager: {ex.Message}");
        }

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
