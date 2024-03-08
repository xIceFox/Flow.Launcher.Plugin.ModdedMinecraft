using System.Windows.Controls;

namespace Flow.Launcher.Plugin.ModdedMinecraft.Views;

/// <summary>
/// 
/// </summary>
public partial class SettingsView : UserControl
{
    /// <summary>
    /// 
    /// </summary>
    public SettingsView(SettingsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        var ram = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / 1024 / 1024;
        MaxRamAllocation.Maximum = ram;
    }
    
}