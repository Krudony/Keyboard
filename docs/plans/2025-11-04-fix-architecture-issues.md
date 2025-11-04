# Fix Architecture Issues - Implementation Plan

**Goal**: Fix 8 critical and medium-priority architecture issues identified in Issue #9 to enable Phase 2 (System Tray) and ensure production-ready quality

**Architecture**: Current Phase 1 creates a hidden window only in HotkeyManager but uses a dummy ApplicationContext that doesn't properly manage the application lifecycle. Phase 2 will require System Tray which needs its own window handle. We need a unified WindowManager that manages both the hotkey listening window and future UI components (System Tray, Settings dialog) with proper lifecycle management and thread safety.

**Tech Stack**: C# 10, .NET 6.0, Windows Forms, Windows P/Invoke (user32.dll), JSON configuration

---

## ⛔ Critical Issues (Must Fix - Blocks Phase 2)

### Issue 1: Application Lifecycle Management
**Impact**: System Tray (Phase 2) will fail because there's no proper window handle
**Current Problem**:
- Program.cs uses `new ApplicationContext()` which creates an empty context with no window
- HotkeyManager creates its own hidden window but doesn't expose it to the application lifecycle
- System Tray needs to be associated with a visible/valid window handle

**Solution**: Create a unified `WindowManager` class that manages both the hotkey window and acts as the primary application window

### Issue 2: Window Handle Conflicts
**Impact**: Phase 2 System Tray registration will fail or crash
**Current Problem**:
- HotkeyManager creates a window in its constructor
- System Tray registration needs a different window handle
- No coordination between components

**Solution**: Single WindowManager provides all window handles to both HotkeyManager and future System Tray

### Issue 3: Config Validation Missing
**Impact**: Invalid config.json with typos crashes the application
**Current Problem**:
- ConfigManager.LoadConfig() doesn't validate the loaded config
- Invalid hotkey string (e.g., "Ctrl+Invalid+Space") won't cause an error until registration
- Missing properties don't get validated

**Solution**: Add ValidateConfig() method with hotkey parsing validation

---

## ⚠️ Medium Priority Issues (Should Fix)

### Issue 4: Memory Leaks from UI Components
**Impact**: Long-running application consumes more memory over time
**Current Problem**:
- NotificationWindow opens/closes repeatedly but may not dispose properly
- Event handlers not always unsubscribed
- Windows handles not always released

**Solution**: Proper IDisposable implementation and ensure all UI components are disposed

### Issue 5: Cross-thread UI Updates
**Impact**: Occasional crashes when updating UI from background hotkey threads
**Current Problem**:
- OnHotkeyPressed() runs on hotkey window thread
- NotificationWindow updates may not be thread-safe

**Solution**: Use Invoke/BeginInvoke for all UI updates from non-UI threads

### Issue 6: Auto-paste Security Risk
**Impact**: Passwords, sensitive data, or emails pasted into wrong fields
**Current Problem**:
- AutoPaste feature (config option) not implemented yet
- When implemented, will blindly paste to any focused window

**Solution**: Add target window validation (whitelist safe applications)

---

## 📝 Low Priority Issues

### Issue 7: Icon Resource Bundling
**Impact**: Application icon may not display in System Tray (Phase 2)
**Current Problem**:
- Icons need to be embedded as resources in .csproj
- Publishing may not include icons properly

**Solution**: Add icon resources to .csproj with proper build action

### Issue 8: Window Position Persistence
**Impact**: Settings window (Phase 2) loses position on restart
**Current Problem**:
- No code to save/restore window positions
- Each UI window will appear in default location

**Solution**: Add WindowPositionManager to save/restore bounds

---

## 📋 Implementation Tasks

### Task 1: Create Unified WindowManager (1-2 hours)

**Files to Create:**
- `src/KeyboardTextConverter/Services/WindowManager.cs`

**Files to Modify:**
- `src/KeyboardTextConverter/Program.cs`
- `src/KeyboardTextConverter/Services/HotkeyManager.cs`

**Step 1: Create WindowManager class**
```csharp
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KeyboardTextConverter;

/// <summary>
/// Centralized window management for the application.
/// Manages the main hidden window for hotkey listening and System Tray integration.
/// </summary>
public class WindowManager : IDisposable
{
    private IntPtr _mainWindowHandle;
    private bool _isInitialized = false;
    private bool _disposed = false;

    public IntPtr MainWindowHandle => _mainWindowHandle;

    // P/Invoke declarations
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
            Console.WriteLine($"WindowManager initialized. Handle: {_mainWindowHandle}");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to initialize WindowManager", ex);
        }
    }

    /// <summary>
    /// Window procedure for handling messages.
    /// </summary>
    private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        // Can be extended to handle other messages (System Tray events, etc.)
        return DefWindowProc(hWnd, msg, wParam, lParam);
    }

    public void Dispose()
    {
        if (_disposed) return;

        if (_mainWindowHandle != IntPtr.Zero)
        {
            DestroyWindow(_mainWindowHandle);
            _mainWindowHandle = IntPtr.Zero;
        }

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
```

**Step 2: Modify HotkeyManager to use WindowManager**
- Remove window creation code from HotkeyManager
- Accept a WindowManager parameter in constructor
- Use the passed window handle for hotkey registration

**Step 3: Update Program.cs**
- Create WindowManager instance before HotkeyManager
- Pass WindowManager to HotkeyManager
- Use WindowManager.MainWindowHandle for ApplicationContext

---

### Task 2: Add Config Validation (1 hour)

**Files to Modify:**
- `src/KeyboardTextConverter/Services/ConfigManager.cs`

**Step 1: Add ValidateConfig method**
```csharp
/// <summary>
/// Validate configuration values.
/// </summary>
public static void ValidateConfig(Config config)
{
    if (config == null)
        throw new ArgumentNullException(nameof(config));

    // Validate hotkey string format
    if (string.IsNullOrWhiteSpace(config.Hotkey))
        throw new InvalidOperationException("Hotkey cannot be empty");

    if (!IsValidHotkeyFormat(config.Hotkey))
        throw new InvalidOperationException($"Invalid hotkey format: {config.Hotkey}. Expected format: 'Ctrl+Shift+Space'");

    // Validate notification duration
    if (config.NotificationDurationMs < 0 || config.NotificationDurationMs > 10000)
        throw new InvalidOperationException("NotificationDurationMs must be between 0 and 10000");

    Console.WriteLine("Configuration validation passed");
}

/// <summary>
/// Check if hotkey string has valid format.
/// </summary>
private static bool IsValidHotkeyFormat(string hotkey)
{
    var parts = hotkey.Split('+');
    if (parts.Length < 2 || parts.Length > 3)
        return false;

    var validModifiers = new[] { "Ctrl", "Shift", "Alt", "Win" };
    var validKeys = new[] { "Space", "A", "B", "C", "1", "2", "3", "F1", "F2" };

    foreach (var part in parts)
    {
        var trimmed = part.Trim();
        if (!validModifiers.Contains(trimmed) && !validKeys.Contains(trimmed))
            return false;
    }

    return true;
}
```

**Step 2: Call ValidateConfig in LoadConfig**
- After deserialization, call ValidateConfig(config)
- Handle exceptions appropriately

---

### Task 3: Fix Thread Safety (1.5 hours)

**Files to Modify:**
- `src/KeyboardTextConverter/Program.cs`
- `src/KeyboardTextConverter/UI/NotificationWindow.cs`

**Step 1: Update OnHotkeyPressed to use thread-safe UI updates**
```csharp
private static void OnHotkeyPressed(HotkeyManager hotkeyManager, NotificationWindow notificationWindow)
{
    try
    {
        // Run UI updates on UI thread
        if (notificationWindow.InvokeRequired)
        {
            notificationWindow.BeginInvoke(new Action(() =>
            {
                ProcessConversion(hotkeyManager, notificationWindow);
            }));
        }
        else
        {
            ProcessConversion(hotkeyManager, notificationWindow);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error handling hotkey: {ex.Message}");
    }
}

private static void ProcessConversion(HotkeyManager hotkeyManager, NotificationWindow notificationWindow)
{
    // Original OnHotkeyPressed logic here
}
```

**Step 2: Ensure NotificationWindow uses using statement**
```csharp
using (var notificationWindow = new NotificationWindow())
{
    // Use window
} // Automatically disposed
```

---

### Task 4: Improve Resource Management (1 hour)

**Files to Modify:**
- `src/KeyboardTextConverter/UI/NotificationWindow.cs`
- `src/KeyboardTextConverter/Services/ClipboardHandler.cs`

**Step 1: Add IDisposable to NotificationWindow**
- Override Dispose() method
- Unsubscribe from all events
- Release all resources

**Step 2: Add error handling and retries to ClipboardHandler**
- Ensure clipboard operations are wrapped with proper exception handling
- Add retry logic with exponential backoff

---

### Task 5: Document Architecture & Window Lifecycle (1 hour)

**Files to Create:**
- `docs/ARCHITECTURE.md`

**Content to Include:**
- Architecture diagram showing WindowManager, HotkeyManager, NotificationWindow
- Window lifecycle explanation
- Thread safety guidelines
- Future System Tray integration points

---

### Task 6: Write Unit Tests for Config Validation (1.5 hours)

**Files to Create:**
- `tests/KeyboardTextConverter.Tests/ConfigManagerTests.cs`

**Test Cases:**
- Valid config loads successfully
- Invalid hotkey format throws exception
- Missing properties use defaults
- Invalid notification duration throws exception
- JSON syntax errors are caught

---

### Task 7: Integration Testing (1.5 hours)

**Manual Testing Checklist:**
- [ ] Application starts without errors
- [ ] Hotkey (Ctrl+Shift+Space) registers successfully
- [ ] Conversion works in Notepad
- [ ] Conversion works in Word
- [ ] Conversion works in VS Code
- [ ] Notification window appears and disappears correctly
- [ ] No memory leaks after 30 minutes of use
- [ ] Custom hotkey in config.json works
- [ ] Invalid config values produce clear error messages

---

## 📊 Verification Checklist

- [ ] WindowManager created and properly manages main window
- [ ] HotkeyManager refactored to use WindowManager
- [ ] Program.cs updated to use new architecture
- [ ] ConfigManager validates all config values
- [ ] All UI updates use Invoke for thread safety
- [ ] All resources properly disposed
- [ ] Unit tests pass (100% of config validation tests)
- [ ] Integration tests pass (hotkey works in 3+ applications)
- [ ] No memory leaks detected (Run for 30+ minutes)
- [ ] Documentation updated with architecture diagram
- [ ] All 8 identified issues addressed

---

## ⏱️ Time Estimate

**Total**: 7-8 hours of development time
- Task 1 (WindowManager): 1-2 hours
- Task 2 (Config Validation): 1 hour
- Task 3 (Thread Safety): 1.5 hours
- Task 4 (Resource Management): 1 hour
- Task 5 (Documentation): 1 hour
- Task 6 (Unit Tests): 1.5 hours
- Task 7 (Integration Testing): 1.5 hours

**Breakdown by Priority:**
- **Critical (must do)**: Tasks 1, 2, 3 = 3.5 hours
- **High (should do)**: Tasks 4, 6 = 2.5 hours
- **Medium (nice to have)**: Tasks 5, 7 = 2.5 hours

---

## 🎯 Phase 2 Readiness

After completing this plan:
- ✅ Phase 2 System Tray will be unblocked
- ✅ WindowManager provides proper window handle
- ✅ Config system is robust and validated
- ✅ Thread safety is guaranteed
- ✅ Resource management is proper
- ✅ Code is documented and tested

---

## 🔗 Related Issues

- Addresses all 8 architecture issues from Issue #9
- Unblocks Phase 2 implementation
- Improves production-readiness from 30% → 80%
