using System;
using System.Drawing;
using System.Windows.Forms;

namespace KeyboardTextConverter;

/// <summary>
/// Floating notification window that shows conversion results.
/// Automatically hides after 2 seconds.
/// </summary>
public class NotificationWindow : Form
{
    private Label _messageLabel;
    private System.Windows.Forms.Timer _hideTimer;
    private const int DISPLAY_DURATION_MS = 2000;
    private const int WINDOW_WIDTH = 400;
    private const int WINDOW_HEIGHT = 80;

    public NotificationWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        // Form settings
        this.FormBorderStyle = FormBorderStyle.None;
        this.BackColor = Color.FromArgb(45, 45, 48); // Dark gray
        this.TopMost = true;
        this.ShowInTaskbar = false;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.ControlBox = false;
        this.Width = WINDOW_WIDTH;
        this.Height = WINDOW_HEIGHT;
        this.StartPosition = FormStartPosition.Manual;

        // Center on screen
        PositionCenter();

        // Add border/rounded effect
        this.Padding = new Padding(10);
        this.AutoScaleMode = AutoScaleMode.Font;

        // Create message label
        _messageLabel = new Label
        {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.White,
            BackColor = Color.FromArgb(45, 45, 48),
            Font = new Font("Segoe UI", 11, FontStyle.Regular),
            AutoSize = false,
        };

        this.Controls.Add(_messageLabel);

        // Setup hide timer
        _hideTimer = new System.Windows.Forms.Timer();
        _hideTimer.Interval = DISPLAY_DURATION_MS;
        _hideTimer.Tick += (s, e) => Hide();
    }

    /// <summary>
    /// Position window in the center-top area of the screen.
    /// </summary>
    private void PositionCenter()
    {
        var workingArea = Screen.PrimaryScreen?.WorkingArea ?? Screen.GetWorkingArea(Point.Empty);
        this.Left = workingArea.Left + (workingArea.Width - WINDOW_WIDTH) / 2;
        this.Top = workingArea.Top + 50; // Top-center position
    }

    /// <summary>
    /// Show notification with the given message.
    /// </summary>
    public void ShowNotification(string message)
    {
        _messageLabel.Text = message;
        this.Show();
        this.BringToFront();
        this.Activate();

        // Restart the hide timer
        _hideTimer.Stop();
        _hideTimer.Start();
    }

    /// <summary>
    /// Show success notification for conversion.
    /// </summary>
    public void ShowSuccess(string direction, string original, string converted)
    {
        string message = $"✓ {direction}\n{original} → {converted}";
        ShowNotification(message);
    }

    /// <summary>
    /// Show error notification.
    /// </summary>
    public void ShowError(string message = "Cannot convert this text")
    {
        string fullMessage = $"✗ {message}";
        ShowNotification(fullMessage);
    }

    /// <summary>
    /// Show "nothing to convert" notification.
    /// </summary>
    public void ShowEmpty()
    {
        ShowNotification("ℹ️ Nothing to convert");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _hideTimer?.Dispose();
            _messageLabel?.Dispose();
        }

        base.Dispose(disposing);
    }
}
