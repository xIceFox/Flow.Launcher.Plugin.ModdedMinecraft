using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Flow.Launcher.Plugin.ModdedMinecraft.Views;

/// <summary>
/// 
/// </summary>
public partial class SettingsView : UserControl
{
    private readonly ModdedMinecraft _pluginBase;

    /// <summary>
    /// 
    /// </summary>
    public SettingsView(SettingsViewModel viewModel, ModdedMinecraft pluginBase)
    {
        InitializeComponent();
        _pluginBase = pluginBase;
        DataContext = viewModel;
        MaxRamAllocation.Maximum =
            (double)new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / 1024 / 1024;
    }

    private void DownloadCurseforgeIcons(object sender, RoutedEventArgs e)
    {
        Task.Run(() => _pluginBase.CollectCurseforgeIcons());
    }
}