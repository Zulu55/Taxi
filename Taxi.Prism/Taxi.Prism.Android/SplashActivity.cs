using Android.App;
using Android.OS;
using System.Threading;

namespace Taxi.Prism.Droid
{
    [Activity(
        Theme = "@style/Theme.Splash",
        MainLauncher = true,
        NoHistory = true)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Thread.Sleep(1800);
            StartActivity(typeof(MainActivity));
        }
    }
}
