using System;
using System.Windows.Forms;

namespace KeyboardTextConverter;

/// <summary>
/// Main application entry point.
/// Initializes all components and runs the message loop.
/// </summary>
static class Program
{
    [STAThread]
    static void Main()
    {
        // Enable visual styles
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        try
        {
            // Load configuration
            var config = ConfigManager.LoadConfig();
            Console.WriteLine($"Configuration loaded. Hotkey: {config.Hotkey}");

            // Create components
            var hotkeyManager = new HotkeyManager();
            var notificationWindow = new NotificationWindow();

            // Setup hotkey event handler
            hotkeyManager.HotkeyPressed += (sender, e) =>
            {
                OnHotkeyPressed(hotkeyManager, notificationWindow);
            };

            // Start listening for hotkey
            hotkeyManager.StartListening();
            Console.WriteLine("Hotkey listener started.");

            // Run message pump
            Application.Run(new ApplicationContext());

            // Cleanup
            hotkeyManager.Dispose();
            notificationWindow.Dispose();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error starting application:\n\n{ex.Message}\n\n{ex.StackTrace}",
                "Keyboard Text Converter Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    /// <summary>
    /// Handle hotkey press event.
    /// Gets clipboard, converts, and shows notification.
    /// </summary>
    private static void OnHotkeyPressed(HotkeyManager hotkeyManager, NotificationWindow notificationWindow)
    {
        try
        {
            // Get text from clipboard
            var originalText = ClipboardHandler.GetText();

            // Check if clipboard is empty
            if (string.IsNullOrWhiteSpace(originalText))
            {
                notificationWindow.ShowEmpty();
                return;
            }

            // Convert text
            var convertedText = ThaiEnglishConverter.Convert(originalText);

            // Check if conversion changed anything
            if (convertedText == originalText)
            {
                notificationWindow.ShowError("No mappable characters found");
                return;
            }

            // Update clipboard with converted text
            if (!ClipboardHandler.SetText(convertedText))
            {
                notificationWindow.ShowError("Failed to update clipboard");
                return;
            }

            // Show success notification with conversion direction and preview
            var direction = ThaiEnglishConverter.GetConversionDirection(originalText);
            string originalPreview = originalText.Length > 15
                ? originalText.Substring(0, 15) + "..."
                : originalText;
            string convertedPreview = convertedText.Length > 15
                ? convertedText.Substring(0, 15) + "..."
                : convertedText;

            notificationWindow.ShowSuccess(direction, originalPreview, convertedPreview);
            Console.WriteLine($"Converted: {originalPreview} → {convertedPreview}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during conversion: {ex.Message}");
            notificationWindow.ShowError("Conversion error");
        }
    }
}
