using Flow.Launcher.Plugin.ModdedMinecraft.Models;

namespace Flow.Launcher.Plugin.ModdedMinecraft.Views;

/// <summary>
/// 
/// </summary>
public class SettingsViewModel : BaseModel
{
    private readonly Settings _settings;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="settings"></param>
    public SettingsViewModel(Settings settings)
    {
        _settings = settings;
    }

    /// <summary>
    /// 
    /// </summary>
    public double RamAllocation
    {
        get => _settings.RamAllocation;
        set
        {
            _settings.RamAllocation = value;
            OnPropertyChanged();
        }
    }
}