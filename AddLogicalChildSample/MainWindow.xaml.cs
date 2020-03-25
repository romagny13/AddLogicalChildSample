using System.Windows;
using System.Windows.Controls;

namespace AddLogicalChildSample
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnChangeChild(object sender, RoutedEventArgs e)
        {
            MySingleChildElement.SetChild(MySingleElement1, new TextBlock { Text = "Sample" });
        }

        private void OnAddChild(object sender, RoutedEventArgs e)
        {
            // MyElement1.AddChild(new TextBlock { Text = $"Item {MyElement1.Children.Count + 1}" });
            MyElement1.Children.Add(new TextBlock { Text = $"Item {MyElement1.Children.Count + 1}" });
        }

        private void OnRemoveChild(object sender, RoutedEventArgs e)
        {
            if (MyElement1.Children.Count > 0)
            {
                MyElement1.Children.RemoveAt(MyElement1.Children.Count - 1);
            }
        }

        private void OnClear(object sender, RoutedEventArgs e)
        {
            MyElement1.Children.Clear();
        }

    }
}
