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
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using CsvHelper;

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
            int totalCost = 0;
            output += $"{input_name.Text} 先生/小姐 您好\n";
            output += $"您要{takeout}飲料，清單如下:\n";
            for (int i = 0; i < orders.Count; i++)
            {
                output += $"第{i+1}項: {drinks[orders[i].Index].Name}{orders[i].Quantity}杯" +
                    $"，每杯{drinks[orders[i].Index].Price}元" +
                    $"，總共{orders[i].Cost}元\n";
                totalCost += orders[i].Cost;
            }
            output += $"總價{totalCost}元";
            if (totalCost >= 500)
            {
                totalCost = (int)(totalCost * 0.8);
                output += $"，消費超過500元打8折，售價{totalCost}元";
            }
            else if (totalCost >= 300)
            {
                totalCost = (int)(totalCost * 0.85);
                output += $"消費超過300元打85折，售價{totalCost}元";
            }
            else if (totalCost >= 200) {
                totalCost = (int)(totalCost * 0.9);
                output += $"消費超過200元打9折，售價{totalCost}元";
            }

            outputBlock.Text = output;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeSpMenu(drinks);
            
        }

        private static void ReadDrinks()
        {
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
                SetPropertyForSd(sd);
                AddValueChangedForSd(i, sd, cb);
                ImplementDataBindingForSdAndLbl(sd, lbl);
                pl.Children.Add(cb);
                pl.Children.Add(sd);
                pl.Children.Add(lbl);
                sp_menu.Children.Add(pl);
            }

        }

        private static void ImplementDataBindingForSdAndLbl(Slider sd, Label lbl)
        {
            Binding bind = new Binding("Value");
            bind.Source = sd;
            lbl.SetBinding(ContentProperty, bind);
        }

        private static void SetPropertyForSd(Slider sd)
        {
            sd.Width = 150;
            sd.Maximum = 10;
            sd.Minimum = 0;
            sd.TickFrequency = 1;
            sd.IsSnapToTickEnabled = true;
        }

        private static void AddValueChangedForSd(int i, Slider sd, CheckBox cb)
        {
            sd.ValueChanged += (sender, e) =>
            {
                Slider slider = (Slider)sender;
                int value = (int)slider.Value;
                if (cb.IsChecked == true)
                {
                    orders.Find(o => o.Index == i).Quantity = value;
                }
            };
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
            public int Cost { get => drinks[Index].Price * Quantity; }

            public OrderItem(int index, int quantity)
            {
                Index = index;
                Quantity = quantity;
            }
        }
    }
    
}