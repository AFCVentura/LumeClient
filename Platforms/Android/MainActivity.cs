using Android.App;
using Android.Content.PM;
using Android.OS;

namespace LumeClient;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        // Initialize any platform-specific services or configurations here if needed
        Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#FBD206")); // Cor da barra
    }
}
