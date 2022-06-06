using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Essentials = Xamarin.Essentials;
using Android;
using System;
using Google.Android.Material.Snackbar;
using Android.Views;

namespace Meeting.Xamarin.Droid
{
    [Activity(Label = "Meeting.Xamarin", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public const int CameraPermissionsCode = 1;
        public static readonly string[] CameraPermissions =
        {
            Manifest.Permission.Camera
        };

        public static event EventHandler CameraPermissionGranted;
        private View _layout;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);


            if (requestCode == CameraPermissionsCode && grantResults[0] == Permission.Denied)
            {
                Snackbar.Make(_layout, "Camera permission is denied. Please allow Camera use.", Snackbar.LengthIndefinite)
                    .SetAction("OK", v => RequestPermissions(CameraPermissions, CameraPermissionsCode))
                    .Show();
                return;
            }

            CameraPermissionGranted?.Invoke(this, EventArgs.Empty);
        }
    }
}