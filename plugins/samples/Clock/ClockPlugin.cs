using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WitchDrawer.PluginSdk;

namespace WitchDrawer.Clock;

public sealed class ClockPlugin : IWidgetPlugin
{
    private readonly DispatcherTimer _timer = new(DispatcherPriority.Background) { Interval = TimeSpan.FromSeconds(1) };
    private readonly TextBlock _time = new() { FontSize = 42, FontWeight = FontWeights.Light, MinWidth = 240, TextAlignment = TextAlignment.Center };
    private readonly TextBlock _date = new() { FontSize = 14, Margin = new Thickness(0, 4, 0, 0), TextAlignment = TextAlignment.Center };
    private WidgetContext? _context;
    private bool _showSeconds = true;
    private bool _use24Hours = true;

    public FrameworkElement CreateView(WidgetContext context)
    {
        _context = context;
        _showSeconds = context.Settings.GetValueOrDefault("showSeconds") != "false";
        _use24Hours = context.Settings.GetValueOrDefault("use24Hours") != "false";
        _time.SetResourceReference(TextBlock.ForegroundProperty, "TextPrimaryBrush");
        _date.SetResourceReference(TextBlock.ForegroundProperty, "TextMutedBrush");
        var panel = new StackPanel { Margin = new Thickness(4, 8, 4, 12) };
        panel.Children.Add(_time); panel.Children.Add(_date);
        Update();
        _timer.Tick += OnTick;
        _timer.Start();
        return panel;
    }

    private void OnTick(object? sender, EventArgs e) => Update();
    private void Update()
    {
        var now = DateTime.Now;
        var format = _use24Hours ? (_showSeconds ? "HH:mm:ss" : "HH:mm") : (_showSeconds ? "hh:mm:ss tt" : "hh:mm tt");
        var culture = _context?.Culture ?? CultureInfo.CurrentCulture;
        var time = now.ToString(format, culture);
        var date = now.ToString("D", culture);
        if (_time.Text != time) _time.Text = time;
        if (_date.Text != date) _date.Text = date;
    }

    public FrameworkElement CreateSettingsView()
    {
        var chinese = (_context?.Culture ?? CultureInfo.CurrentUICulture).Name.StartsWith("zh", StringComparison.OrdinalIgnoreCase);
        var panel = new StackPanel { Margin = new Thickness(20) };
        var hours = new CheckBox { Content = chinese ? "24 小时制" : "24-hour time", IsChecked = _use24Hours, Margin = new Thickness(0, 0, 0, 12) };
        var seconds = new CheckBox { Content = chinese ? "显示秒数" : "Show seconds", IsChecked = _showSeconds };
        foreach (var control in new[] { hours, seconds }) control.SetResourceReference(Control.ForegroundProperty, "TextPrimaryBrush");
        hours.Click += async (_, _) => { _use24Hours = hours.IsChecked == true; await SaveAsync(); };
        seconds.Click += async (_, _) => { _showSeconds = seconds.IsChecked == true; await SaveAsync(); };
        panel.Children.Add(hours); panel.Children.Add(seconds);
        return panel;
    }
    private async Task SaveAsync()
    {
        Update();
        try
        {
            if (_context is not null) await _context.SaveSettingsAsync(new Dictionary<string, string>
            { ["showSeconds"] = _showSeconds ? "true" : "false", ["use24Hours"] = _use24Hours ? "true" : "false" });
        }
        catch (Exception exception) { _context?.ReportError(exception); }
    }
    public void Dispose() { _timer.Stop(); _timer.Tick -= OnTick; _context = null; }
}
