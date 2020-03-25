using System.Windows;
using System.Windows.Media;

namespace AddLogicalChildSample
{
    public class MySingleChildElement : FrameworkElement
    {
        protected override Visual GetVisualChild(int index)
        {
            return (UIElement)GetValue(ChildProperty);
        }

        public static void SetChild(DependencyObject obj, UIElement value)
        {
            obj.SetValue(ChildProperty, value);
        }

        public static readonly DependencyProperty ChildProperty =
            DependencyProperty.RegisterAttached("Child", typeof(UIElement),
                typeof(MyElement), new PropertyMetadata(null, OnChildChanged));

        public static void OnChildChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            MySingleChildElement myElement = obj as MySingleChildElement;

            Visual oldChild = e.OldValue as Visual;
            if (oldChild != null)
            {
                myElement.RemoveVisualChild(oldChild);
                myElement.RemoveLogicalChild(oldChild);
            }

            Visual newChild = e.NewValue as Visual;
            if (newChild != null)
            {
                myElement.AddVisualChild(newChild);
                myElement.AddLogicalChild(newChild);
            }

            myElement.InvalidateMeasure();
        }

        protected override int VisualChildrenCount
        {
            get
            {
                UIElement childElement = (UIElement)GetValue(ChildProperty);
                return childElement != null ? 1 : 0;
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            UIElement childElement = (UIElement)GetValue(ChildProperty);
            if (childElement != null)
                childElement.Measure(availableSize);

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            UIElement childElement = (UIElement)GetValue(ChildProperty);
            if (childElement != null)
                childElement.Arrange(new Rect(new Point(0.0, 0.0), finalSize));

            return finalSize;
        }

        //    // Render a "X"
        //    protected override void OnRender(DrawingContext dc)
        //    {
        //        dc.DrawLine(new Pen(Brushes.Blue, 2.0),
        //            new Point(0.0, 0.0),
        //            new Point(ActualWidth, ActualHeight));
        //        dc.DrawLine(new Pen(Brushes.Green, 2.0),
        //            new Point(ActualWidth, 0.0),
        //            new Point(0.0, ActualHeight));
        //    }
    }

}
