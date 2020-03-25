using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace AddLogicalChildSample
{

    public class UIElementCollection : ObservableCollection<UIElement>
    {
        internal UIElementCollection()
        {

        }
    }

    [ContentProperty("Children")]
    public class MyElement : FrameworkElement, IAddChild // IAddChild is Obsolete
    {
        private List<object> internalChildrenSnapshot = new List<object>();

        protected override int VisualChildrenCount
        {
            get { return Children.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            var child = Children[index];
            return child;
        }

        public UIElementCollection Children
        {
            get { return (UIElementCollection)GetValue(ChildrenProperty); }
            set { SetValue(ChildrenProperty, value); }
        }

        public static readonly DependencyProperty ChildrenProperty =
            DependencyProperty.Register("Children", typeof(UIElementCollection), typeof(MyElement));

        public MyElement()
        {
            Children = new UIElementCollection();
            Children.CollectionChanged += Children_CollectionChanged;
        }

        private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddVisualChildInternal(e.NewItems[0]);
                    internalChildrenSnapshot.Insert(e.NewStartingIndex, e.NewItems[0]);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveVisualChildInternal(e.OldItems[0]);
                    internalChildrenSnapshot.Insert(e.OldStartingIndex, e.OldItems[0]);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    ClearVisualChildrenInternal(e.OldItems);
                    internalChildrenSnapshot.Clear();
                    break;
                default:
                    throw new NotSupportedException();
            }
            Invalidate();
        }

        public void Invalidate()
        {
            InvalidateMeasure();
        }

        private void AddVisualChildInternal(object newItem)
        {
            Visual newChild = newItem as Visual;
            if (newChild != null)
            {
                AddVisualChild(newChild);
                AddLogicalChild(newChild);
            }
        }

        private void RemoveVisualChildInternal(object oldItem)
        {
            Visual oldChild = oldItem as Visual;
            if (oldChild != null)
            {
                RemoveVisualChild(oldChild);
                RemoveLogicalChild(oldChild);
            }
        }

        private void ClearVisualChildrenInternal(IEnumerable oldItems)
        {
            foreach (var oldItem in internalChildrenSnapshot)
            {
                Visual oldChild = oldItem as Visual;
                if (oldChild != null)
                {
                    RemoveVisualChild(oldChild);
                    RemoveLogicalChild(oldChild);
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (var child in Children)
            {
                child.Measure(availableSize);
            }

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double y = 0.0;
            foreach (var child in Children)
            {
                double childHeight = child.DesiredSize.Height;
                child.Arrange(new Rect(0.0, y, finalSize.Width, childHeight));
                y += childHeight;
            }

            return finalSize;
        }

        public void AddChild(object value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            UIElement element = value as UIElement;
            if (element != null)
                Children.Add(element);
        }

        public void AddText(string text)
        {
            XamlSerializerUtil.ThrowIfNonWhiteSpaceInAddText(text, this);
        }
    }

    internal static class XamlSerializerUtil
    {
        internal static void ThrowIfNonWhiteSpaceInAddText(string s, object parent)
        {
            if (s != null)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (!Char.IsWhiteSpace(s[i]))
                    {
                        throw new ArgumentException();
                    }
                }
            }
        }
    }

}
