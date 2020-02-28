using Android.App;
using Android.Content.PM;
using Android.OS;
using Plugin.CurrentActivity;
using Plugin.Permissions;
using Prism;
using Prism.Ioc;
using Syncfusion.SfBusyIndicator.XForms.Droid;
using Syncfusion.SfRating.XForms.Droid;

namespace Taxi.Prism.Droid
{
    [Activity(
        Label = "Taxi", 
        Icon = "@mipmap/ic_launcher", 
        Theme = "@style/MainTheme", 
        MainLauncher = false, 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            CrossCurrentActivity.Current.Init(this, bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            new SfBusyIndicatorRenderer();
            new SfRatingRenderer();
            LoadApplication(new App(new AndroidInitializer()));
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }

    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register any platform specific implementations
        }
    }
}

