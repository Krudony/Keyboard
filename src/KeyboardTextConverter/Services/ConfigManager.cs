using System;
using System.IO;
using Newtonsoft.Json;

namespace KeyboardTextConverter;

/// <summary>
/// Manages application configuration loading and saving.
/// </summary>
public class ConfigManager
{
    private static readonly string CONFIG_DIR = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "KeyboardTextConverter"
    );

    private static readonly string CONFIG_FILE = Path.Combine(CONFIG_DIR, "config.json");

    public class Config
    {
        [JsonProperty("hotkey")]
        public string Hotkey { get; set; } = "Ctrl+Shift+Space";

        [JsonProperty("autoPaste")]
        public bool AutoPaste { get; set; } = false;

        [JsonProperty("enableNotifications")]
        public bool EnableNotifications { get; set; } = true;

        [JsonProperty("notificationDurationMs")]
        public int NotificationDurationMs { get; set; } = 2000;
    }

    /// <summary>
    /// Load configuration from config.json or create default.
    /// </summary>
    public static Config LoadConfig()
    {
        try
        {
            // Create config directory if it doesn't exist
            if (!Directory.Exists(CONFIG_DIR))
            {
                Directory.CreateDirectory(CONFIG_DIR);
            }

            // Load existing config or create default
            if (File.Exists(CONFIG_FILE))
            {
                string json = File.ReadAllText(CONFIG_FILE);
                var config = JsonConvert.DeserializeObject<Config>(json);
                return config ?? new Config();
            }
            else
            {
                // Create default config
                var defaultConfig = new Config();
                SaveConfig(defaultConfig);
                return defaultConfig;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading config: {ex.Message}");
            return new Config();
        }
    }

    /// <summary>
    /// Save configuration to config.json.
    /// </summary>
    public static void SaveConfig(Config config)
    {
        try
        {
            if (!Directory.Exists(CONFIG_DIR))
            {
                Directory.CreateDirectory(CONFIG_DIR);
            }

            string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(CONFIG_FILE, json);
            Console.WriteLine($"Config saved to: {CONFIG_FILE}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving config: {ex.Message}");
        }
    }

    /// <summary>
    /// Get the path to the config file.
    /// </summary>
    public static string GetConfigPath()
    {
        return CONFIG_FILE;
    }
}
