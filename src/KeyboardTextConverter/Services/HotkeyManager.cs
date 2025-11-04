using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KeyboardTextConverter;

/// <summary>
/// Manages global hotkey registration and events.
/// Allows the application to listen for Ctrl+Shift+Space regardless of active application.
/// Now uses WindowManager for proper application lifecycle management.
/// </summary>
public class HotkeyManager : IDisposable
{
    private const int WM_HOTKEY = 0x0312;
    private const int HOTKEY_ID = 9000;
    private const uint MOD_CONTROL = 0x0002;
    private const uint MOD_SHIFT = 0x0004;
    private const uint VK_SPACE = 0x20;

    private WindowManager _windowManager;
    private IntPtr _windowHandle;
    private bool _isRegistered = false;
    private bool _disposed = false;

    public event EventHandler? HotkeyPressed;

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    /// <summary>
    /// Initialize with an existing WindowManager.
    /// This allows proper lifecycle management where WindowManager is created first.
    /// </summary>
    public HotkeyManager(WindowManager windowManager)
    {
        _windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
        _windowHandle = _windowManager.MainWindowHandle;
        Console.WriteLine($"HotkeyManager initialized with WindowManager. Window handle: {_windowHandle}");
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
        Console.WriteLine("Hotkey registered: Ctrl+Shift+Space");
    }

    /// <summary>
    /// Unregister the global hotkey.
    /// </summary>
    public void Unregister()
    {
        if (!_isRegistered) return;

        UnregisterHotKey(_windowHandle, HOTKEY_ID);
        _isRegistered = false;
        Console.WriteLine("Hotkey unregistered");
    }

    /// <summary>
    /// Start listening for the hotkey.
    /// Must be called from the main message loop.
    /// </summary>
    public void StartListening()
    {
        Register();
        Console.WriteLine("HotkeyManager listening for hotkey events");
    }

    /// <summary>
    /// Raise the HotkeyPressed event.
    /// Called from WindowManager's message pump.
    /// </summary>
    public void OnHotkeyPressed()
    {
        HotkeyPressed?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        if (_disposed) return;

        try
        {
            Unregister();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error unregistering hotkey during dispose: {ex.Message}");
        }

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
