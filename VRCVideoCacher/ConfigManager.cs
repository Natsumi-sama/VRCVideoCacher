﻿using Newtonsoft.Json;
using Serilog;

namespace VRCVideoCacher;

public class ConfigManager
{
    public static ConfigModel config;
    public static ILogger Log = Program.Logger.ForContext("SourceContext", "ConfigManager");
    public static void Load()
    {
        Log.Information("Loading config...");
        if (!File.Exists("Config.json"))
        {
            config = new ConfigModel();
            SaveConfig();
        }
        else
        {
            config = JsonConvert.DeserializeObject<ConfigModel>(File.ReadAllText("Config.json"));
            SaveConfig();
        }
        Log.Information("Loaded config.");
    }

    public static void SaveConfig()
    {
        Log.Information("Saving config...");
        File.WriteAllText("Config.json", JsonConvert.SerializeObject(config, Formatting.Indented));
    }
}

public class ConfigModel
{
    public string ytdlWebServerURL = "http://localhost:9696/";
    public string ytdlPath = "Utils/yt-dlp.exe";
    public string CachedAssetPath = "CachedAssets";
    public string[] BlockedUrls = new[] { "https://na2.vrdancing.club/sampleurl.mp4" };
    public bool EnableCache = true;
    public bool RUMode = false;
    public string RUModeToken = "";
}