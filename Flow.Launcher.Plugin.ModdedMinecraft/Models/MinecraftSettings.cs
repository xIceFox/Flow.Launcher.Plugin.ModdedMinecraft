namespace Flow.Launcher.Plugin.ModdedMinecraft.Models;

public class MinecraftSettings
{
    public bool CrashAssistance { get; set; } = false;
    public bool EnableAdvanced { get; set; } = false;
    public bool EnableAnalytics { get; set; } = false;
    public bool EnableHistorical { get; set; } = false;
    public bool EnableReleases { get; set; } = true;
    public bool EnableSnapshots { get; set; } = false;
    public bool KeepLauncherOpen { get; set; } = false;
    public string ProfileSorting { get; set; } = "ByLastPlayed";
    public bool ShowGameLog { get; set; } = false;
    public bool ShowMenu { get; set; } = false;
    public bool SoundOn { get; set; } = false;
}