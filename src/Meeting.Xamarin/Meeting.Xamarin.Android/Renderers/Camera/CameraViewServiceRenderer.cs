﻿using Android;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Meeting.Xamarin.Droid.Renderers.Camera;
using Meeting.Xamarin.Controls;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CameraView), typeof(CameraViewServiceRenderer))]
namespace Meeting.Xamarin.Droid.Renderers.Camera
{
	public class CameraViewServiceRenderer : ViewRenderer<CameraView, CameraDroid>
	{
		private CameraDroid _camera;
		private readonly Context _context;

		public CameraViewServiceRenderer(Context context) : base(context)
		{
			_context = context;
		}

		private CameraView _cameraView;

		protected override void OnElementChanged(ElementChangedEventArgs<CameraView> e)
		{
			base.OnElementChanged(e);

			var permissions = CameraPermissions();
			_camera = new CameraDroid(Context);
			_cameraView = e.NewElement;

			CameraOptions CameraOption = e.NewElement?.Camera??CameraOptions.Rear;


			if (Control == null)
			{
				if (permissions)
				{
					_camera.OpenCamera(CameraOption);

					SetNativeControl(_camera);
				}
				else
				{
					MainActivity.CameraPermissionGranted += (sender, args) =>
					{
						_camera.OpenCamera(CameraOption);

						SetNativeControl(_camera);
					};
				}
			}

			if (e.NewElement != null && _camera != null)
			{
				_camera.Photo += OnPhoto;
                _camera.CaptureFrameChanged += OnCaptureFrameChanged;
			}
		}

        private void OnCaptureFrameChanged(object sender, CaptureFrameEventArgs e)
        {
			_cameraView.RaiseCaptureFrameChanged(e);
		}

		private async void OnPhoto(object sender, ImageSource imgSource)
		{
			var imageData = await RotateImageToPortrait(imgSource);

			//Device.BeginInvokeOnMainThread(() =>
		 //  {
			//   MainPage.OnPhotoCaptured(imageData);
		 //  });
		}

		protected override void Dispose(bool disposing)
		{
			_camera.CaptureFrameChanged -= OnCaptureFrameChanged;
			_camera.Photo -= OnPhoto;

			base.Dispose(disposing);
		}

		private bool CameraPermissions()
		{
			const string permission = Manifest.Permission.Camera;

			if ((int)Build.VERSION.SdkInt < 23 || ContextCompat.CheckSelfPermission(Android.App.Application.Context, permission) == Permission.Granted)
			{
				return true;
			}

			ActivityCompat.RequestPermissions((MainActivity)_context, MainActivity.CameraPermissions, MainActivity.CameraPermissionsCode);

			return false;
		}

		// ReSharper disable once UnusedMember.Local
		private async Task<ImageSource> RotateImageToPortrait(ImageSource imgSource)
		{
			var imagesourceHandler = new StreamImagesourceHandler();
			var photoTask = imagesourceHandler.LoadImageAsync(imgSource, _context);

			var photo = await photoTask;

			var matrix = new Matrix();

			matrix.PreRotate(-90);
			photo = Bitmap.CreateBitmap(photo, 0, 0, photo.Width, photo.Height, matrix, false);
			matrix.Dispose();

			var stream = new MemoryStream();
			photo.Compress(Bitmap.CompressFormat.Jpeg, 50, stream);
			stream.Seek(0L, SeekOrigin.Begin);

			return ImageSource.FromStream(() => stream);
		}
	}
}
