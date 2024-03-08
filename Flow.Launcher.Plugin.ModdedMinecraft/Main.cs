using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using Flow.Launcher.Plugin.ModdedMinecraft.Models;
using Flow.Launcher.Plugin.ModdedMinecraft.Views;

namespace Flow.Launcher.Plugin.ModdedMinecraft
{
    /// <summary>
    /// 
    /// </summary>
    public class ModdedMinecraft : IPlugin, ISettingProvider
    {
        private PluginInitContext _context;
        

        private List<string> _curseForgeModpacks;

        private Settings _settings;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void Init(PluginInitContext context)
        {
            var userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var directories = Directory.GetDirectories(userPath + @"\curseforge\minecraft\Instances");
            _curseForgeModpacks = directories.Select(Path.GetFileName).ToList();
            _context = context;
            _settings = context.API.LoadSettingJsonStorage<Settings>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<Result> Query(Query query)
        {
            var result = _curseForgeModpacks
                .Select(name => new Result()
                {
                    Title = name,
                    Action = ac => StartModpack(ac, name)
                }).ToList();
            return result;
        }


        private static bool StartModpack(ActionContext actionContext, string name)
        {
            var processStartInfo = new ProcessStartInfo();
            var userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            processStartInfo.FileName = $@"{userPath}\curseforge\minecraft\Install\minecraft.exe";
            processStartInfo.Arguments = @$"--workDir {userPath}\curseforge\minecraft\Install";
            var proc = Process.Start(processStartInfo);
            return true;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Control CreateSettingPanel()
        {
            return new SettingsView(new SettingsViewModel(_settings));
        }
    }
}