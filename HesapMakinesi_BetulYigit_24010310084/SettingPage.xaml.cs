
namespace HesapMakinesi_BetulYigit_24010310084;

public partial class SettingsPage : ContentPage
{
    private bool _isInitializing = true;

    public SettingsPage()
    {
        InitializeComponent();

        string savedTheme = Preferences.Get("AppTheme", "System");

        LightRadio.IsChecked = savedTheme == "Light";
        DarkRadio.IsChecked = savedTheme == "Dark";
        SystemRadio.IsChecked = savedTheme == "System";

        _isInitializing = false;
    }

    private void OnThemeChanged(object sender, CheckedChangedEventArgs e)
    {
        if (_isInitializing || !e.Value)
            return;

        if (sender == LightRadio)
        {
            ChangeTheme("Light", AppTheme.Light);
        }
        else if (sender == DarkRadio)
        {
            ChangeTheme("Dark", AppTheme.Dark);
        }
        else if (sender == SystemRadio)
        {
            ChangeTheme("System", AppTheme.Unspecified);
        }
    }

    private void ChangeTheme(string themeName, AppTheme theme)
    {
        Application.Current!.UserAppTheme = theme;

        Preferences.Set("AppTheme", themeName);
    }
}
