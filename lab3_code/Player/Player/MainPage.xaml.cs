using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.System.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.ViewManagement;
using Windows.UI;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Player
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DisplayRequest appDisplayRequest = null;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void startClicked(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();
            EllStoryboard.Begin();
        }

        private void pauseClicked(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
            EllStoryboard.Pause();
        }

        private void stopClicked(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            EllStoryboard.Stop();
        }

        private void fullClicked(object sender, RoutedEventArgs e)
        {
            if(picture.Visibility == Visibility.Visible)
            {
                return;
            }

            if (mediaPlayer.IsFullWindow == true)
            {
                mediaPlayer.IsFullWindow = false;
            }
            else
            {
                mediaPlayer.IsFullWindow = true;
            }
        }

        private async void choseClicked(object sender, RoutedEventArgs e)
        {
            await SetLocalMedia();
        }

        async private System.Threading.Tasks.Task SetLocalMedia()
        {
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();

            openPicker.FileTypeFilter.Add(".wmv");
            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".wma");
            openPicker.FileTypeFilter.Add(".mp3");

            var file = await openPicker.PickSingleFileAsync();

            // mediaPlayer is a MediaElement defined in XAML
            if (file != null)
            {
                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                mediaPlayer.SetSource(stream, file.ContentType);

                mediaPlayer.Play();
                EllStoryboard.Begin();

                if (file.FileType == ".mp3" || file.FileType == ".wma")
                {
                    picture.Visibility = Visibility.Visible;
                    //_mediaPlayerElement.Visibility = Visibility.Collapsed;
                }
                else
                {
                    picture.Visibility = Visibility.Collapsed;
                    //_mediaPlayerElement.Visibility = Visibility.Visible;
                }
            }
        }

        private void mediaOpened(object sender, RoutedEventArgs e)
        {
            time.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
        }

    }
}
