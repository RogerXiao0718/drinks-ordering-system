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
        private string takeout = "";
        private static Drink[] drinks =
            {
                new Drink("咖啡大杯", 60),
                new Drink("咖啡中杯", 50),
                new Drink("紅茶大杯", 30),
                new Drink("紅茶中杯", 20),
                new Drink("綠茶大杯", 25),
                new Drink("綠茶中杯", 20),
            };
        private static List<OrderItem> orders = new List<OrderItem>();
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void radiobutton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (rb.IsChecked == true)
            {
                this.takeout = rb.Content.ToString();
            }

        }

        private void Checkin_Click(object sender, RoutedEventArgs e)
        {
            string output = "";

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            InitializeSpMenu(drinks);
            
        }

        private void InitializeSpMenu(Drink[] drinks)
        {
            for (int i = 0; i < drinks.Length; i++)
            {
                StackPanel pl = new StackPanel();
                pl.Orientation = Orientation.Horizontal;
                CheckBox cb = new CheckBox();
                Slider sd = new Slider();
                Label lbl = new Label();
                cb.Content = $"{drinks[i].Name}{drinks[i].Price}元";
                Thickness thick = new Thickness();
                thick.Right = 10;
                cb.Margin = thick;
                AddClickHandlerForCb(i, cb, sd);
                sd.Width = 150;
                sd.Maximum = 10;
                sd.Minimum = 0;
                sd.TickFrequency = 1;
                sd.IsSnapToTickEnabled = true;
                Binding bind = new Binding("Value");
                bind.Source = sd;
                lbl.SetBinding(ContentProperty, bind);
                pl.Children.Add(cb);
                pl.Children.Add(sd);
                pl.Children.Add(lbl);
                sp_menu.Children.Add(pl);
            }

        }

        private static void AddClickHandlerForCb(int i, CheckBox cb, Slider sd)
        {
            cb.Click += (sender, e) =>
            {
                CheckBox checkbox = (CheckBox)sender;
                if (checkbox.IsChecked == true)
                {
                    orders.Add(new OrderItem(i, (int)sd.Value));
                }
                else
                {
                    orders = orders.FindAll((order) =>
                    {
                        if (order.Index != i) return true;
                        else return false;
                    });
                }
            };
        }

        private class Drink
        {
            public string Name { get; set; }
            public int Price { get; set; }

            public Drink(string name, int price)
            {
                Name = name;
                Price = price;
            }
        }
        private class OrderItem
        {
            public int Index { get; set; }
            public int Quantity { get; set; }
            public int Cost { get; }

            public OrderItem(int index, int quantity)
            {
                Index = index;
                Quantity = quantity;
                Cost = drinks[index].Price * quantity;
            }
        }
    }
    
}