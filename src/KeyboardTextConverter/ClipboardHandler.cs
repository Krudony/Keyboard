using System;
using System.Windows.Forms;

namespace KeyboardTextConverter;

/// <summary>
/// Safely handles clipboard operations (get/set text).
/// Implements retry logic and error handling for clipboard access.
/// </summary>
public class ClipboardHandler
{
    private const int MAX_RETRIES = 3;
    private const int RETRY_DELAY_MS = 50;

    /// <summary>
    /// Get text from clipboard safely with retry logic.
    /// </summary>
    public static string? GetText()
    {
        for (int i = 0; i < MAX_RETRIES; i++)
        {
            try
            {
                if (Clipboard.ContainsText(TextDataFormat.UnicodeText))
                {
                    return Clipboard.GetText(TextDataFormat.UnicodeText);
                }
                else if (Clipboard.ContainsText(TextDataFormat.Text))
                {
                    return Clipboard.GetText(TextDataFormat.Text);
                }

                return null;
            }
            catch (System.Runtime.InteropServices.ExternalException ex)
            {
                if (i < MAX_RETRIES - 1)
                {
                    System.Threading.Thread.Sleep(RETRY_DELAY_MS);
                    continue;
                }

                System.Diagnostics.Debug.WriteLine($"Clipboard read failed after {MAX_RETRIES} retries: {ex.Message}");
                return null;
            }
        }

        return null;
    }

    /// <summary>
    /// Set text in clipboard safely with retry logic.
    /// </summary>
    public static bool SetText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return false;

        for (int i = 0; i < MAX_RETRIES; i++)
        {
            try
            {
                Clipboard.SetText(text, TextDataFormat.UnicodeText);
                return true;
            }
            catch (System.Runtime.InteropServices.ExternalException ex)
            {
                if (i < MAX_RETRIES - 1)
                {
                    System.Threading.Thread.Sleep(RETRY_DELAY_MS);
                    continue;
                }

                System.Diagnostics.Debug.WriteLine($"Clipboard write failed after {MAX_RETRIES} retries: {ex.Message}");
                return false;
            }
        }

        return false;
    }

    /// <summary>
    /// Check if clipboard has text content.
    /// </summary>
    public static bool HasText()
    {
        try
        {
            return Clipboard.ContainsText(TextDataFormat.UnicodeText) ||
                   Clipboard.ContainsText(TextDataFormat.Text);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Clear clipboard.
    /// </summary>
    public static void Clear()
    {
        try
        {
            Clipboard.Clear();
        }
        catch (System.Runtime.InteropServices.ExternalException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to clear clipboard: {ex.Message}");
        }
    }
}
