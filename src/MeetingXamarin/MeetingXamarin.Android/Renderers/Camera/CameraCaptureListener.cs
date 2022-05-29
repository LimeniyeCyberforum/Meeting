using Android.Hardware.Camera2;
using System;

namespace MeetingXamarin.Droid.Renderers.Camera
{
	public class CameraCaptureListener : CameraCaptureSession.CaptureCallback
	{
		public event EventHandler PhotoComplete;

		public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request,
			TotalCaptureResult result)
		{
			PhotoComplete?.Invoke(this, EventArgs.Empty);
		}
	}
}
