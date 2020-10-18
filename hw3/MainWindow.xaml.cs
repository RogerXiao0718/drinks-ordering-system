using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
//由於直接匯入Systems.Windows.Forms會發生命名衝突，所以只匯入OpenFileDialog
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
//匯入CsvHelper用來讀取.csv檔
using CsvHelper;
using System.IO;
using System.Globalization;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        //初始化takeout欄位來記錄內用或外帶
        private string takeout = "";
        //宣告drinks用於存放飲料的的資訊
        private static List<Drink> drinks;
        //初始化orders用於存放訂單
        private static List<OrderItem> orders = new List<OrderItem>();
        
        public MainWindow()
        {
            InitializeComponent();
        }
      

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //在視窗載入後先讀取csv檔
            ReadDrinksFromCsv();
            //根據drinks的內容動態新增Control到飲料清單內
            InitializeSpMenu(drinks);         
        }

        private static void ReadDrinksFromCsv()
        {
            //建立OpenFileDialog物件
            OpenFileDialog fileDialog = new OpenFileDialog();
            //用來過濾掉非.csv檔，讓使用者比較好瀏覽
            fileDialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            //顯示Dialog，並確認使用者點選的是確認，否則drinks會是空的
            if(fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //.csv檔的完整路徑
                string fileName = fileDialog.FileName;
                using (StreamReader reader = new StreamReader(fileName, Encoding.Default)) //建立StreamReader物件並將編碼機制設為Default
                using (CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))//用reader來初始化CsvReader
                {
                    //從.csv檔讀資料並將IEnumerable<Drink>透過呼叫toList()轉換為List<Drink>
                   drinks = csvReader.GetRecords<Drink>().ToList();
                }
            } else
            {
                drinks = new List<Drink>();
            }
        }   

        private static void WriteOutputToTextFile(string output)
        {
            //寫入的檔案位置
            string path = @"D:\大二視窗應用程式\result\output.txt";
            //建立StreamWriter的物件並給予檔案位置
            using (StreamWriter writer = new StreamWriter(path))
            {
                //將output輸出到檔案
                writer.Write(output);
            }
        }
        private void InitializeSpMenu(List<Drink> drinks)
        {
            //利用迴圈讀取drinks，並用這些資料來動態建立Control
            for (int i = 0; i < drinks.Count; i++)
            {
                StackPanel pl = new StackPanel();
                //將pl內的Control的擺放方向改成水平擺放
                pl.Orientation = Orientation.Horizontal;
                CheckBox cb = new CheckBox();
                Slider sd = new Slider();
                Label lbl = new Label();
                cb.Content = $"{drinks[i].Name}{drinks[i].Size}{drinks[i].Price}元";
                //-----------加一些Margin在Checkbox的右側-----------
                Thickness thick = new Thickness();
                thick.Right = 10;
                cb.Margin = thick;
                //------------------------------------------------
                AddClickHandlerForCb(i, cb, sd); //綁定cb的Click事件
                SetPropertyForSd(sd); //設定Slider的屬性
                AddValueChangedForSd(i, sd, cb); //綁定Slider的ValueChanged事件
                ImplementDataBindingForSdAndLbl(sd, lbl); //實作Slider和Label的Data Binding
                //---------將Control加入StackPanel內--------
                pl.Children.Add(cb);
                pl.Children.Add(sd);
                pl.Children.Add(lbl);
                sp_menu.Children.Add(pl);
                //---------------------------------------
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
            //若Checkbox被check，Slider的值改變時會修改已經在orders內的訂單的Quantity
            sd.ValueChanged += (sender, e) =>  //lambda
            {
                Slider slider = (Slider)sender;
                int value = (int)slider.Value;
                if (cb.IsChecked == true)
                {
                    //若orders內有資料的Index == i，那筆資料的Quantity就會被修改
                    orders.Find(o => o.Index == i).Quantity = value;
                }
            };
        }

        private static void AddClickHandlerForCb(int i, CheckBox cb, Slider sd)
        {
            //當cb被Click時，若被Check則新增訂單到orders內，若被uncheck則透過i尋找同一筆訂單並將它移除
            cb.Click += (sender, e) =>
            {
                CheckBox checkbox = (CheckBox)sender;
                if (checkbox.IsChecked == true)
                {
                    orders.Add(new OrderItem(i, (int)sd.Value));
                }
                else
                {
                    //移除訂單
                    orders = orders.FindAll((order) =>
                    {
                        if (order.Index != i) return true;
                        else return false;
                    });
                }
            };
        }
        private void radiobutton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            //將內用/外帶存放在takeout
            if (rb.IsChecked == true)
            {
                this.takeout = rb.Content.ToString();
            }

        }

        private void Checkin_Click(object sender, RoutedEventArgs e)
        {
            string output = ""; //output用來存放要輸出的字串
            int totalCost = 0; //總價
            output += $"{input_name.Text} 先生/小姐 您好\n";
            output += $"您要{takeout}飲料，清單如下:\n";
            for (int i = 0; i < orders.Count; i++)
            {
                output += $"第{i + 1}項: {drinks[orders[i].Index].Name + drinks[orders[i].Index].Size}{orders[i].Quantity}杯" +
                    $"，每杯{drinks[orders[i].Index].Price}元" +
                    $"，總共{orders[i].Cost}元\n";
                totalCost += orders[i].Cost;
            }
            output += $"總價{totalCost}元";

            //計算打折
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
            else if (totalCost >= 200)
            {
                totalCost = (int)(totalCost * 0.9);
                output += $"消費超過200元打9折，售價{totalCost}元";
            }

            //將output顯示在outputBlock上面
            outputBlock.Text = output;
            //將output輸出到.txt檔案
            WriteOutputToTextFile(output);
        }

        //飲料類別
        private class Drink
        {
            public string Name { get; set; }

            public string Size { get; set; }

            public int Price { get; set; }

        }

        //訂單類別
        private class OrderItem
        {
            public int Index { get; set; }
            public int Quantity { get; set; }
            public int Cost { get => drinks[Index].Price * Quantity; } //用來取得這筆訂單的花費

            public OrderItem(int index, int quantity)
            {
                Index = index;
                Quantity = quantity;
            }
        }
    }
    
}