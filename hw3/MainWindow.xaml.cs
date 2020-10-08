using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        private string takeout;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (rb.IsChecked == true)
            {
                this.takeout = rb.Content.ToString();
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StackPanel pl = new StackPanel();
            pl.Orientation = Orientation.Horizontal;

            CheckBox cb = new CheckBox();
            Slider sd = new Slider();
            Label lbl = new Label();
            string[] drinks_arr = { "咖啡大杯", "咖啡中杯", "紅茶大杯", "紅茶中杯", "綠茶大杯", "綠茶中杯" };
            int[] prices_arr = { };
            List<string> drinks = new List<string>(drinks_arr);
            List<int> prices = new List<int>(prices_arr);
            Binding bind = new Binding('value');

            BindingOperations.SetBinding(lbl, ContentProperty, );
        }
    }
}