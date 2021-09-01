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
using System.Windows.Threading;
using System.Windows.Media;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

namespace CampReservationforCheck
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string BasePath = "https://camp519-default-rtdb.firebaseio.com/";
        private const string FirebaseSecret = "LxLxifxo7yumTWUTil6gseRMqVSyCbX8QO6EoQjZ";
        private static FirebaseClient _client;
        private static string sValue1, sValue2, sValue3, sValue4, sValue5, sValue6, sValue7, sValue8, sValue9;
        public MainWindow()
        {
            InitializeComponent();

            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = FirebaseSecret,
                BasePath = BasePath
            };

            _client = new FirebaseClient(config);

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromTicks(6000000000);   // 10분
            timer.Tick += new EventHandler(Button_Click);
            timer.Start();          
        }
        
        private async void Printer1Button_Click(object sender, RoutedEventArgs e)
        {
            string id = "Finish";

            var todoName = new Todo
            {
                tmp = id,
            };

            var value = await _client.UpdateAsync("PrintData/printNum : 1", todoName);

            FirebaseResponse response1 = await _client.GetAsync("PrintData/printNum : 1/tmp");
            String value1 = response1.ResultAs<String>();
            Printer1Button.Content = value1;
        }
        
        private async void Printer2Button_Click(object sender, RoutedEventArgs e)
        {
            string id = "Finish";

            var todoName = new Todo
            {
                tmp = id,
            };

            var value = await _client.UpdateAsync("PrintData/printNum : 2", todoName);

            FirebaseResponse response2 = await _client.GetAsync("PrintData/printNum : 2/tmp");
            String value2 = response2.ResultAs<String>();
            Printer2Button.Content = value2;
        }

        private async void Printer3Button_Click(object sender, RoutedEventArgs e)
        {
            string id = "Finish";

            var todoName = new Todo
            {
                tmp = id,
            };

            var value = await _client.UpdateAsync("PrintData/printNum : 3", todoName);

            FirebaseResponse response3 = await _client.GetAsync("PrintData/printNum : 3/tmp");
            String value3 = response3.ResultAs<String>();
            Printer3Button.Content = value3;
        }

        private async void Printer4Button_Click(object sender, RoutedEventArgs e)
        {
            string id = "Finish";

            var todoName = new Todo
            {
                tmp = id,
            };

            var value = await _client.UpdateAsync("PrintData/printNum : 4", todoName);

            FirebaseResponse response4 = await _client.GetAsync("PrintData/printNum : 4/tmp");
            String value4 = response4.ResultAs<String>();
            Printer4Button.Content = value4;
        }

        private async void Printer5Button_Click(object sender, RoutedEventArgs e)
        {
            string id = "Finish";

            var todoName = new Todo
            {
                tmp = id,
            };

            var value = await _client.UpdateAsync("PrintData/printNum : 5", todoName);

            FirebaseResponse response5 = await _client.GetAsync("PrintData/printNum : 5/tmp");
            String value5 = response5.ResultAs<String>();
            Printer5Button.Content = value5;
        }

        private async void Printer6Button_Click(object sender, RoutedEventArgs e)
        {
            string id = "Finish";

            var todoName = new Todo
            {
                tmp = id,
            };

            var value = await _client.UpdateAsync("PrintData/printNum : 6", todoName);

            FirebaseResponse response6 = await _client.GetAsync("PrintData/printNum : 6/tmp");
            String value6 = response6.ResultAs<String>();
            Printer6Button.Content = value6;
        }

        private async void Printer7Button_Click(object sender, RoutedEventArgs e)
        {
            string id = "Finish";

            var todoName = new Todo
            {
                tmp = id,
            };

            var value = await _client.UpdateAsync("PrintData/printNum : 7", todoName);

            FirebaseResponse response7 = await _client.GetAsync("PrintData/printNum : 7/tmp");
            String value7 = response7.ResultAs<String>();
            Printer7Button.Content = value7;
        }

        private async void Printer8Button_Click(object sender, RoutedEventArgs e)
        {
            string id = "Finish";

            var todoName = new Todo
            {
                tmp = id,
            };

            var value = await _client.UpdateAsync("PrintData/printNum : 8", todoName);

            FirebaseResponse response8 = await _client.GetAsync("PrintData/printNum : 8/tmp");
            String value8 = response8.ResultAs<String>();
            Printer8Button.Content = value8;
        }

        private async void LaserButton_Click(object sender, RoutedEventArgs e)
        {
            string id = "Finish";

            var todoName = new Todo
            {
                Count = id,
            };

            var value = await _client.UpdateAsync("LaserData", todoName);

            FirebaseResponse response9 = await _client.GetAsync("LaserData/Count");
            String value9 = response9.ResultAs<String>();
            LaserButton.Content = value9;
        }       

        internal class Todo
        {
            public string tmp { get; set; }
            public string Count { get; set; }
        }

        private async void Button_Click(object sender, EventArgs e)
        {
            Boolean[] ReservationNumber = new Boolean[] { false, false, false, false, false, false, false, false, false };
            int count = 0;
            FirebaseResponse response1 = await _client.GetAsync("PrintData/printNum : 1/tmp");
            String value1 = response1.ResultAs<String>();
            Printer1Button.Content = value1;
            if (sValue1 != value1 && value1 != "Finish")
            {
                ReservationNumber[0] = true;
                count++;
            }
            sValue1 = value1;

            FirebaseResponse response2 = await _client.GetAsync("PrintData/printNum : 2/tmp");
            String value2 = response2.ResultAs<String>();
            Printer2Button.Content = value2;
            if (sValue2 != value2 && value2 != "Finish")
            {
                ReservationNumber[1] = true;
                count++;
            }
            sValue2 = value2;

            FirebaseResponse response3 = await _client.GetAsync("PrintData/printNum : 3/tmp");
            String value3 = response3.ResultAs<String>();
            Printer3Button.Content = value3;
            if (sValue3 != value3 && value3 != "Finish")
            {
                ReservationNumber[2] = true;
                count++;
            }
            sValue3 = value3;

            FirebaseResponse response4 = await _client.GetAsync("PrintData/printNum : 4/tmp");
            String value4 = response4.ResultAs<String>();
            Printer4Button.Content = value4;
            if (sValue4 != value4 && value4 != "Finish")
            {
                ReservationNumber[3] = true;
                count++;
            }
            sValue4 = value4;

            FirebaseResponse response5 = await _client.GetAsync("PrintData/printNum : 5/tmp");
            String value5 = response5.ResultAs<String>();
            Printer5Button.Content = value5;
            if (sValue5 != value5 && value5 != "Finish")
            {
                ReservationNumber[4] = true;
                count++;
            }
            sValue5 = value5;

            FirebaseResponse response6 = await _client.GetAsync("PrintData/printNum : 6/tmp");
            String value6 = response6.ResultAs<String>();
            Printer6Button.Content = value6;
            if (sValue6 != value6 && value6 != "Finish")
            {
                ReservationNumber[5] = true;
                count++;
            }
            sValue6 = value6;

            FirebaseResponse response7 = await _client.GetAsync("PrintData/printNum : 7/tmp");
            String value7 = response7.ResultAs<String>();
            Printer7Button.Content = value7;
            if (sValue7 != value7 && value7 != "Finish")
            {
                ReservationNumber[6] = true;
                count++;
            }
            sValue7 = value7;

            FirebaseResponse response8 = await _client.GetAsync("PrintData/printNum : 8/tmp");
            String value8 = response8.ResultAs<String>();
            Printer8Button.Content = value8;
            if (sValue8 != value8 && value8 != "Finish")
            {
                ReservationNumber[7] = true;
                count++;
            }
            sValue8 = value8;

            FirebaseResponse response9 = await _client.GetAsync("App/Laser/박성재/tmp");
            String value9 = response9.ResultAs<String>();
            LaserButton.Content = value9;
            if (sValue9 != value9 && value9 != "Finish")
            {
                ReservationNumber[8] = true;
                count++;
            }
            sValue9 = value9;
            
            string alert = "";
            if (count > 0)
            {
                for (int i = 0; i < 9; i++)
                {
                    if (ReservationNumber[i] == true)
                    {
                        alert += (i + 1) + " ";
                    }                  
                }
                alert += "번 새로 예약되었습니다.";
                MediaPlayer media = new MediaPlayer();
                media.Open(new Uri("C:/Alert.mp3", UriKind.Absolute));
                media.Play();
                MessageBox.Show(alert);
            }
        }
    }
}
