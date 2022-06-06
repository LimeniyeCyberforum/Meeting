using Android.Content;
using Android.Views;
using Meeting.Xamarin.Droid.Renderers.GridSplitter;
using Meeting.Xamarin.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(GridSplitter), typeof(GridSplitterRenderer))]
namespace Meeting.Xamarin.Droid.Renderers.GridSplitter
{
    public class GridSplitterRenderer : VisualElementRenderer<Controls.GridSplitter>
    {
        private Point _lastPoint;

        public GridSplitterRenderer(Context context) : base(context)
        {
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            switch (e.Action)
            {
                case (int)MotionEventActions.Down:
                    {
                        _lastPoint = new Point(e.RawX, e.RawY);
                        break;
                    }

                case MotionEventActions.Move:
                    {
                        Element.UpdateGrid(Context.FromPixels(e.RawX - _lastPoint.X), Context.FromPixels(e.RawY - _lastPoint.Y));
                        _lastPoint = new Point(e.RawX, e.RawY);
                        break;
                    }
            }

            return true;
        }
    }
}