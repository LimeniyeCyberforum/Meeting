using Meeting.Business.Common.Abstractions;
using Meeting.Business.Common.Abstractions.FrameCapture;
using Meeting.WPF;
using MvvmCommon.WindowsDesktop;
using System;
using System.Windows;
using System.Windows.Threading;

namespace Meeting.Wpf.CaptureFrames
{
    public class CaptureFramesViewModel : BaseInpc
    {
        private readonly Dispatcher dispatcher = Application.Current.Dispatcher;
        private readonly CaptureFramesServiceAbstract _captureFramesService;

        public ObservableDictionary<Guid, byte[]?> CaptureFrameAreas { get; } = new ObservableDictionary<Guid, byte[]?>();

        public CaptureFramesViewModel(CaptureFramesServiceAbstract captureFramesService)
        {
            _captureFramesService = captureFramesService;
            _captureFramesService.CaptureFrameStateChanged += OnCaptureFrameStateChanged;
            _captureFramesService.CaptureFrameChanged += OnCaptureFrameChanged;
        }

        private void OnCaptureFrameStateChanged(object? sender, Business.Common.EventArgs.CaptureFrameStateEventArgs e)
        {
            dispatcher.BeginInvoke(() =>
            {
                if (e.IsOn)
                {
                    CaptureFrameAreas.Add(e.CaptureAreadGuid, null);
                }
                else
                {
                    CaptureFrameAreas.Remove(e.CaptureAreadGuid);
                }
            });
        }

        private void OnCaptureFrameChanged(object? sender, Business.Common.EventArgs.CaptureFrameEventArgs e)
        {
            dispatcher.BeginInvoke(() =>
            {
                CaptureFrameAreas[e.CaptureAreadGuid] = e.Bytes;
            });
        }
    }
}
