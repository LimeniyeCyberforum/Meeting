using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Essentials = Xamarin.Essentials;
using Android;
using System;
using Google.Android.Material.Snackbar;
using Android.Views;
using Meeting.Business.Common.Abstractions;
using Meeting.Business.GrpcClient;
using DependencyService = Xamarin.Forms.DependencyService;
using Meeting.Business.Common.Abstractions.HibernateSessions;

namespace Meeting.Xamarin.Droid
{
    [Activity(Label = "Meeting.Xamarin", Icon = "@mipmap/icon", Theme = "@style/Theme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private IMeetingService _meetingService;
        public const int CameraPermissionsCode = 1;
        public static readonly string[] CameraPermissions =
        {
            Manifest.Permission.Camera
        };

        public static event EventHandler CameraPermissionGranted;
        private View _layout;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            OnCreate();

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

        #region Lifecycle

        private void OnCreate()
        {
            _meetingService = new MeetingService();
            _meetingService.OnCreate();
            DependencyService.RegisterSingleton(_meetingService);
        }

        protected override void OnResume()
        {
            try
            {
                if (_meetingService is not null)
                    _meetingService.OnResume();
            }
            catch
            {
                // TODO : Should be handling
                throw;
            }
            finally
            {
                base.OnResume();
            }
        }

        protected override void OnPause()
        {
            try
            {
                if (_meetingService is not null)
                    _meetingService.OnPause();
            }
            catch
            {
                // TODO : Should be handling
                throw;
            }
            finally
            {
                base.OnPause();
            }
        }

        protected override void OnDestroy()
        {
            try
            {
                if (_meetingService is not null)
                    _meetingService.OnDestroy();
            }
            catch
            {
                // TODO : Should be handling
                throw;
            }
            finally
            {
                base.OnDestroy();
            }
        }

        #endregion
    }
}