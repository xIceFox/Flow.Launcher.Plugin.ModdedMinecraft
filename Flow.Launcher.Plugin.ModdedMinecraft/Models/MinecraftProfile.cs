using System;

namespace Flow.Launcher.Plugin.ModdedMinecraft.Models;

public class MinecraftProfile
{
    public string Name { get; set; }
    public string Type { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastUsed { get; set; }
    public string Icon { get; set; }
    public string LastVersionId { get; set; }
    public string GameDir { get; set; }
    public string JavaArgs { get; set; }
    public Resolution Resolution { get; set; }
    
    public static string bindJvmArguments(double ramAllocation, string targetDir, string libraryDir)
    {
        return
            $"-Xmx{ramAllocation}m -Xms256m -Dminecraft.applet.TargetDirectory=\"{targetDir}\" -Dfml.ignorePatchDiscrepancies=true -Dfml.ignoreInvalidMinecraftCertificates=true -Duser.language=en -Duser.country=US -DlibraryDirectory=\"{libraryDir}\"";
    }
}

public class Resolution
{
    public int Height { get; set; }
    public int Width { get; set; }
}

public static class MinecraftProfileTypes
{
    public static string Custom = "custom";
    public static string LatestSnapshot = "latest-snapshot";
    public static string LatestRelease = "latest-release";
}

public static class MinecraftProfileIcons
{
    public static string Dirt = "Dirt";
    public static string Grass = "Grass";
    public static string Furnace = "Furnace";
}

public static class MinecraftDefaultProfiles
{
    public static MinecraftProfile LatestRelease = new MinecraftProfile()
    {
        Name = "",
        Type = MinecraftProfileTypes.LatestRelease,
        Created = DateTime.MinValue,
        LastUsed = DateTime.MinValue,
        Icon = MinecraftProfileIcons.Grass,
        LastVersionId = MinecraftProfileTypes.LatestRelease
    };
    
    public static MinecraftProfile LatestSnapshot = new MinecraftProfile()
    {
        Name = "",
        Type = MinecraftProfileTypes.LatestSnapshot,
        Created = DateTime.MinValue,
        LastUsed = DateTime.MinValue,
        Icon = MinecraftProfileIcons.Dirt,
        LastVersionId = MinecraftProfileTypes.LatestSnapshot
    };
}