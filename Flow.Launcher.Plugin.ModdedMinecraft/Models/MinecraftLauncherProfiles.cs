using System.Collections.Generic;

namespace Flow.Launcher.Plugin.ModdedMinecraft.Models;

public class MinecraftLauncherProfiles
{
    public Dictionary<string, MinecraftProfile> Profiles { get; set; }
    public MinecraftSettings Settings { get; set; }
    public int version => 3;
}