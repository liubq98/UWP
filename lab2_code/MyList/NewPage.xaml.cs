using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage.Streams;
using Windows.Storage.Pickers;
using System.Text;
using System.Threading.Tasks;
using MyList.Services;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using System.Xml.Linq;
using MyList.Models;
using SQLitePCL;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace MyList
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NewPage : Page
    {
        ViewModels.TodoItemViewModel ViewModel = ViewModels.TodoItemViewModel.getInstance();
        private SQLiteConnection conn = App.conn;
        public NewPage()
        {
            this.InitializeComponent();
        }

        private async void Create_Clicked(object sender, RoutedEventArgs e)
        {
            string str = "";
            if (title.Text == "")
            {
                str = "Title can't be empty!\n";
            }
            if (detail.Text == "")
            {
                str += "Detail can't be empty!\n";
            }
            if (date.Date < DateTime.Today)
            {
                str += "Date is invalid!";
            }

            if (str != "")
            {
                var messageDialog = new MessageDialog(str);
                await messageDialog.ShowAsync();
            }
            else if (Create.Content.ToString() == "Create")
            {
                using (var temp = conn.Prepare("INSERT INTO TodoItem (title, date, detail, finish, img) VALUES (?,?,?,?,?)"))
                {
                    temp.Bind(1, title.Text);
                    temp.Bind(2, date.Date.ToString());
                    temp.Bind(3, detail.Text);
                    temp.Bind(4, "false");
                    temp.Bind(5, Common.imgPath);
                    temp.Step();
                }
                ViewModel.AddTodoItem(date.Date.DateTime, picture.Source, title.Text, detail.Text, false, Common.imgPath);
                Common.imgPath = "";

                detail.Text = "";
                title.Text = "";
                date.Date = DateTime.Today;
                picture.Source = new BitmapImage(new Uri("ms-appx:///Assets/candy.jpg"));

                ViewModel.SelectedItem = null;
                await new MessageDialog("Create successfully!").ShowAsync();

                Frame.Navigate(typeof(MainPage));
            }
            else
            {
                ViewModel.UpdateTodoItem(ViewModel.SelectedItem.id, date.Date.DateTime, picture.Source, title.Text, detail.Text, Common.imgPath);
                ViewModel.SelectedItem = null;
                Common.imgPath = "";

                await new MessageDialog("Update successfully!").ShowAsync();
                
                Frame.Navigate(typeof(MainPage));
            }
        }

        private void Cancel_Clicked(object sender, RoutedEventArgs e)
        {
            if (Create.Content.ToString() == "Update")
            {
                title.Text = ViewModel.SelectedItem._title;
                detail.Text = ViewModel.SelectedItem._description;
                date.Date = ViewModel.SelectedItem._date;
                picture.Source = ViewModel.SelectedItem.img;
            }
            else
            {
                detail.Text = "";
                title.Text = "";
                date.Date = DateTime.Today;
                picture.Source = new BitmapImage(new Uri("ms-appx:///Assets/candy.jpg"));
            }

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.NavigationMode == NavigationMode.New)
            {
                ApplicationData.Current.LocalSettings.Values.Remove("newpage");
            }
            else
            {
                if(ApplicationData.Current.LocalSettings.Values.ContainsKey("newpage"))
                {
                    var composite = ApplicationData.Current.LocalSettings.Values["newpage"] as ApplicationDataCompositeValue;
                    title.Text = (string)composite["title"];
                    detail.Text = (string)composite["detail"];
                    date.Date = (DateTimeOffset)composite["date"];
                    Common.imgPath = (string)composite["image"];
                    Create.Content = (string)composite["create"];
                    if (Common.imgPath != "")
                    {
                        picture.Source = new BitmapImage(new Uri("ms-appx:///Assets/" + Common.imgPath));
                    }
                    else
                    {
                        picture.Source = new BitmapImage(new Uri("ms-appx:///Assets/candy.jpg"));
                    }

                    ApplicationData.Current.LocalSettings.Values.Remove("newpage");
                    
                }
            }
            
            
            
            if (ViewModel.SelectedItem != null)
            {
                Create.Content = "Update";
                title.Text = ViewModel.SelectedItem._title;
                detail.Text = ViewModel.SelectedItem._description;
                date.Date = ViewModel.SelectedItem._date;
                picture.Source = ViewModel.SelectedItem.img;
                DeleteAppBarButton.Visibility = Visibility.Visible;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            bool suspending = ((App)App.Current).issuspend;
            if (suspending)
            {
                ApplicationDataCompositeValue composite = new ApplicationDataCompositeValue();
                composite["title"] = title.Text;
                composite["detail"] = detail.Text;
                composite["date"] = date.Date;
                composite["image"] = Common.imgPath;
                composite["create"] = Create.Content;
                ApplicationData.Current.LocalSettings.Values["newpage"] = composite;
            }
        }

        private void SelectPicture(object sender, RoutedEventArgs e)
        {
            var common = new Common();
            common.SelectPicture(picture);
        }

        private async void DeleteAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.RemoveTodoItem(ViewModel.SelectedItem.id);
            ViewModel.SelectedItem = null;
            
            TileService.SetBadgeCountOnTile(ViewModel.AllItems.Count());

            await new MessageDialog("Delete successfully！").ShowAsync();
            Frame.Navigate(typeof(MainPage));
        }
    }
}
