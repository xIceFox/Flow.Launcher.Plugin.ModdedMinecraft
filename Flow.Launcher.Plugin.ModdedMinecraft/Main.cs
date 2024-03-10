using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Flow.Launcher.Plugin.ModdedMinecraft.Models;
using Flow.Launcher.Plugin.ModdedMinecraft.Views;
using JetBrains.Annotations;
using Microsoft.Win32;

namespace Flow.Launcher.Plugin.ModdedMinecraft
{
    /// <summary>
    /// 
    /// </summary> 
    [UsedImplicitly]
    public class ModdedMinecraft : IPlugin, ISettingProvider
    {
        private PluginInitContext _context;

        private string _curseforgeRoot;
        private List<MinecraftProfile> _curseForgeModpacks = new List<MinecraftProfile>();

        private Settings _settings;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void Init(PluginInitContext context)
        {
            _context = context;
            _settings = context.API.LoadSettingJsonStorage<Settings>();
            
            CollectCurseforgeModpacks();
        }

        /// <summary>
        /// 
        /// </summary>
        private void CollectCurseforgeModpacks()
        {
            _curseforgeRoot = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Overwolf\CurseForge",
                "minecraft_root", null);

            if (_curseforgeRoot is null) return;
            
            var directories = Directory.GetDirectories(Path.Combine(_curseforgeRoot, "Instances"));
            var missingIcon = false;
            _curseForgeModpacks = directories.Select(dirPath =>
            {
                var manifestJson = JsonDocument.Parse(File.ReadAllText(Path.Combine(dirPath, "manifest.json")));
                var lastVersionId = manifestJson.RootElement.GetProperty("minecraft").GetProperty("modLoaders")[0]
                    .GetProperty("id").GetString();

                if (!ModpackHasIcon(dirPath)) missingIcon = true;
                
                // Prefill only values that are not related to settings you can set in flow launcher.
                return new MinecraftProfile()
                {
                    Name = Path.GetFileName(dirPath),
                    Type = MinecraftProfileTypes.Custom,
                    LastUsed = DateTime.Now,
                    Created = DateTime.Now,
                    Icon = MinecraftProfileIcons.Furnace,
                    LastVersionId = lastVersionId,
                    GameDir = dirPath,
                    Resolution = new Resolution() { Width = 1024, Height = 768 }
                };
            }).ToList();

            if (missingIcon && _settings.FirstBoot)
            {
                _context.API.ShowMsg("ModdedMinecraft", "Some modpack icons are missing. To download them go to my plugin settings. This message is only shown once.");
                _settings.FirstBoot = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task CollectCurseforgeIcons()
        {
            var httpClient = new HttpClient();

            var tasks = _curseForgeModpacks
                .Where(modpack => !ModpackHasIcon(modpack.GameDir))
                .Select(modpack => GetCurseforgeIcon(httpClient, modpack))
                .ToArray();

            Task.WaitAll(tasks);
            _context.API.ShowMsg("Test", Thread.CurrentThread.Name);
            _context.API.ShowMsg("ModdedMinecraft", "Icons downloaded successfully!");
            return Task.CompletedTask;
        }

        private static bool ModpackHasIcon(string gameDir) =>
            Directory.GetFiles(gameDir, "modpackIcon.*").Length >= 1;
        
        private static async Task GetCurseforgeIcon(HttpClient httpClient, MinecraftProfile modpack)
        {
            var response = await httpClient.GetAsync(
                $"https://www.curseforge.com/api/v1/mods/search?gameId=432&index=0&classId=4471&filterText={modpack.Name}&pageSize=1&sortField=1");

            var jsonResponse = await response.Content.ReadAsStringAsync();

            var imgUrl = JsonDocument.Parse(jsonResponse).RootElement.GetProperty("data")[0].GetProperty("avatarUrl")
                .GetString();

            if (imgUrl == null)
                return;
            
            if (!Path.HasExtension(imgUrl))
                return;

            var fileExtension = Path.GetExtension(imgUrl);
            var imageResponse = await httpClient.GetAsync(imgUrl);
            var mediaStream = await imageResponse.Content.ReadAsStreamAsync();

            var fileStream = new FileStream(Path.Combine(modpack.GameDir, "modpackIcon" + fileExtension), FileMode.Create);
            await mediaStream.CopyToAsync(fileStream);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<Result> Query(Query query)
        {
            var result = _curseForgeModpacks
                .Where(profile => profile.Name
                    .ToLower()
                    .Replace(" ", "")
                    .Contains(query.Search
                        .ToLower()
                        .Replace(" ", "")
                    )
                )
                .Select(profile =>
                {
                    var modpackIcons = Directory.GetFiles(profile.GameDir, "modpackIcon.*");
                    var iconPath = Path.Combine(_context.CurrentPluginMetadata.PluginDirectory, "furnace.ico");
                    if (modpackIcons.Length >= 1)
                        iconPath = modpackIcons[0];
                    
                    return new Result()
                    {
                        Title = profile.Name,
                        SubTitle = "Curseforge",
                        IcoPath = iconPath,
                        Action = _ => StartModpack(profile)
                    };
                }).ToList();
            return result;
        }
        
        private bool StartModpack(MinecraftProfile profile)
        {
            profile.JavaArgs = MinecraftProfile.bindJvmArguments(_settings.RamAllocation, profile.GameDir,
                Path.Combine(_curseforgeRoot, @"Install\libraries"));

            var launcherProfile = new MinecraftLauncherProfiles()
            {
                Profiles = new Dictionary<string, MinecraftProfile>()
                {
                    { "latestRelease", MinecraftDefaultProfiles.LatestRelease },
                    { "latestSnapshot", MinecraftDefaultProfiles.LatestSnapshot },
                    { profile.Name, profile }
                },
                Settings = new MinecraftSettings()
            };

            var serializedProfile = JsonSerializer.Serialize(launcherProfile, new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            var minecraftInstallationPath = Path.Combine(_curseforgeRoot, "Install");
            var path = Path.Combine(minecraftInstallationPath, "launcher_profiles.json");
            File.WriteAllText(path, serializedProfile);

            var processStartInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(minecraftInstallationPath, "minecraft.exe"),
                Arguments = $"--workDir {minecraftInstallationPath}"
            };

            Process.Start(processStartInfo);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Control CreateSettingPanel()
        {
            return new SettingsView(new SettingsViewModel(_settings), this);
        }
    }
}