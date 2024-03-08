using System;
using System.Globalization;
using System.Windows.Data;

namespace Flow.Launcher.Plugin.ModdedMinecraft.ValueConverters;

/// <summary>
/// 
/// </summary>
public class SliderConverter : IValueConverter
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    /// <exception cref="ApplicationException"></exception>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) throw new ApplicationException();
        return ((double)value).ToString("0") + " MB";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return double.Parse(((string) value)?.Replace(" MB", "") ?? throw new InvalidOperationException());
    }
}