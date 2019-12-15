using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Xml;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace HW3
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void SearchClicked(object sender, RoutedEventArgs e)
        {
            RootObject myWeather = await OpenWeatherMapProxy.GetWeather(city.Text);
            weather.Text = myWeather.results[0].location.name + " - "+ myWeather.results[0].now.text + " - " + myWeather.results[0].now.temperature + "℃";
        }

        private async void search2_Click(object sender, RoutedEventArgs e)
        {
            string myArea = await XmlAPI.GetArea(number.Text);
            area.Text = myArea;
        }
    }
}
