using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace WPFView
{
    public static class FrmElem
    {
        public static double GetWidthToHeight(FrameworkElement element)
        {
            return (double)element.GetValue(WidthToHeightProperty);
        }

        public static void SetWidthToHeight(FrameworkElement element, double widthToHeight)
        {
            element.SetValue(WidthToHeightProperty, widthToHeight);
        }

        public static readonly DependencyProperty WidthToHeightProperty =
            DependencyProperty.RegisterAttached("WidthToHeight", typeof(double), typeof(FrmElem), new PropertyMetadata(-1.0, ProportionalChanged));


        private static void ProportionalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FrameworkElement element))
                throw new ArgumentException("Must be a FrameworkElement");

            SizeChangedElement changedElement = GetSizeChangeElement(element);

            if (changedElement == null)
                SetSizeChangeElement(element, changedElement = new SizeChangedElement(element));

            changedElement.SetWidthToHeight((double)e.NewValue);
        }


        private static SizeChangedElement GetSizeChangeElement(FrameworkElement obj)
        {
            return (SizeChangedElement)obj.GetValue(SizeChangeElementPropertyKey.DependencyProperty);
        }

        private static void SetSizeChangeElement(FrameworkElement obj, SizeChangedElement value)
        {
            obj.SetValue(SizeChangeElementPropertyKey, value);
        }

        private static readonly DependencyPropertyKey SizeChangeElementPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("SizeChangeElement", typeof(SizeChangedElement), typeof(FrmElem), new PropertyMetadata(null));


        private class SizeChangedElement
        {
            public FrameworkElement Element { get; }

            public double WidthToHeight { get; private set; } = 1;

            public SizeChangedElement(FrameworkElement element)
            {
                Element = element ?? throw new ArgumentNullException(nameof(element));
                element.LayoutUpdated += Element_LayoutUpdated;
            }

            private void Element_LayoutUpdated(object sender = null, EventArgs e = null)
            {
                if (WidthToHeight <= 0)
                    return;

                Rect rect = LayoutInformation.GetLayoutSlot(Element);

                double widthArea = rect.Width - Element.Margin.Left - Element.Margin.Right;
                double heightArea = rect.Height - Element.Margin.Top - Element.Margin.Bottom;

                double width = widthArea;
                double height = width * WidthToHeight;
                if (height > heightArea)
                {
                    height = heightArea;
                    width = height / WidthToHeight;
                }
                Element.Width = width > 0.0 ? width : 0.0;
                Element.Height = height > 0.0 ? height : 0.0;
            }

            public void SetWidthToHeight(double widthToHeight)
            {
                WidthToHeight = widthToHeight;
                Element_LayoutUpdated();
            }

        }

    }
}
