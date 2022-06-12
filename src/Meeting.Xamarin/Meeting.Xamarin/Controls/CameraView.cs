using System;
using Xamarin.Forms;

namespace Meeting.Xamarin.Controls
{
	public class CaptureFrameEventArgs : EventArgs
    {
        public byte[] Data { get; }
		
		public DateTime Time { get; }

		public CaptureFrameEventArgs(byte[] data, DateTime time)
		{
			Data = data;
			Time = time;
		}
	}

	public enum CameraOptions
	{
		Rear,
		Front
	}

	public class CameraView : View
	{
		public CameraOptions Camera
		{
			get => (CameraOptions)GetValue(CameraProperty);
			set => SetValue(CameraProperty, value);
		}

		public static readonly BindableProperty CameraProperty = BindableProperty.Create(
			"Camera", typeof(CameraOptions), typeof(CameraView), CameraOptions.Rear);

		public static event EventHandler<CaptureFrameEventArgs> CaptureFrameChanged;

		// TODO : Temporary
		public void RaiseCaptureFrameChanged(CaptureFrameEventArgs captureFrameEventArgs)
        {
			CaptureFrameChanged?.Invoke(this, captureFrameEventArgs);
		}
	}
}
