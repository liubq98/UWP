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
using Windows.Storage.Streams;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Text;
using Windows.UI.Xaml.Shapes;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.Storage;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using MyList.Services;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using System.Xml.Linq;
using Windows.ApplicationModel.DataTransfer;
using SQLitePCL;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace MyList
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ViewModels.TodoItemViewModel ViewModel = ViewModels.TodoItemViewModel.getInstance();
        DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();

        private string s_title;
        private string s_detail;
        private string s_date;
        private StorageFile s_img;

        private SQLiteConnection conn = App.conn;

        public MainPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (newpage.Visibility == Visibility.Collapsed)
            {
                ViewModel.SelectedItem = null;
                Frame.Navigate(typeof(NewPage));
            }
            else
            {
                Create.Content = "Create";
                DeleteAppBarButton.Visibility = Visibility.Collapsed;

                detail.Text = "";
                title.Text = "";
                date.Date = DateTime.Today;
                picture.Source = new BitmapImage(new Uri("ms-appx:///Assets/candy.jpg"));
            }
        }
        
        private async void DeleteAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteAppBarButton.Visibility = Visibility.Collapsed;
            Create.Content = "Create";
            ViewModel.RemoveTodoItem(ViewModel.SelectedItem.id);
            ViewModel.SelectedItem = null;

            detail.Text = "";
            title.Text = "";
            date.Date = DateTime.Today;
            picture.Source = new BitmapImage(new Uri("ms-appx:///Assets/candy.jpg"));
            
            TileService.SetBadgeCountOnTile(ViewModel.AllItems.Count());

            await new MessageDialog("Delete successfully！").ShowAsync();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            dataTransferManager.DataRequested += OnDataRequested;

            if (e.NavigationMode == NavigationMode.New)
            {
                ApplicationData.Current.LocalSettings.Values.Remove("newpage");
            }
            else
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("newpage"))
                {
                    var composite = ApplicationData.Current.LocalSettings.Values["newpage"] as ApplicationDataCompositeValue;
                    title.Text = (string)composite["title"];
                    detail.Text = (string)composite["detail"];
                    date.Date = (DateTimeOffset)composite["date"];
                    Common.imgPath = (string)composite["image"];
                    Create.Content = (string)composite["create"];

                    if(Common.imgPath != "")
                    {
                        picture.Source = new BitmapImage(new Uri("ms-appx:///Assets/" + Common.imgPath));
                    }
                    else
                    {
                        picture.Source = new BitmapImage(new Uri("ms-appx:///Assets/candy.jpg"));
                    }
                    
                    for (int i = 0; i < ViewModel.AllItems.Count(); i++)
                    {
                        ViewModel.AllItems[i]._finish = (bool)composite["ischecked" + i];
                    }

                    ApplicationData.Current.LocalSettings.Values.Remove("newpage");
                }
            }
            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            dataTransferManager.DataRequested += OnDataRequested;

            bool suspending = ((App)App.Current).issuspend;
            if(suspending)
            {
                ApplicationDataCompositeValue composite = new ApplicationDataCompositeValue();
                composite["title"] = title.Text;
                composite["detail"] = detail.Text;
                composite["date"] = date.Date;
                composite["image"] = Common.imgPath;
                composite["create"] = Create.Content;
                for(int i = 0; i < ViewModel.AllItems.Count(); i++)
                {
                    composite["ischecked" + i] = ViewModel.AllItems[i]._finish;
                }

                ApplicationData.Current.LocalSettings.Values["newpage"] = composite;
            }
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
            else if(Create.Content.ToString() == "Create")
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
                this.ViewModel.AddTodoItem(date.Date.DateTime, picture.Source, title.Text, detail.Text, false, Common.imgPath);
                Common.imgPath = "";

                detail.Text = "";
                title.Text = "";
                date.Date = DateTime.Today;
                picture.Source = new BitmapImage(new Uri("ms-appx:///Assets/candy.jpg"));

                ViewModel.SelectedItem = null;
                await new MessageDialog("Create successfully!").ShowAsync();
            }
            else
            {
                this.ViewModel.UpdateTodoItem(this.ViewModel.SelectedItem.id, date.Date.DateTime, picture.Source, title.Text, detail.Text, Common.imgPath);
                Common.imgPath = "";

                await new MessageDialog("Update successfully!").ShowAsync();
            }
        }

        private void Cancel_Clicked(object sender, RoutedEventArgs e)
        {
            if(Create.Content.ToString() == "Update")
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

        private void SelectPicture(object sender, RoutedEventArgs e)
        {
            var common = new Common();
            common.SelectPicture(picture);
        }
        
        private void TodoItem_ItemClicked(object sender, ItemClickEventArgs e)
        {
            ViewModel.SelectedItem = e.ClickedItem as Models.TodoItem;
            if (newpage.Visibility == Visibility.Collapsed)
            {
                Frame.Navigate(typeof(NewPage));
            }
            else
            {
                DeleteAppBarButton.Visibility = Visibility.Visible;
                Create.Content = "Update";

                DateTime d = DateTime.Today;
                title.Text = ViewModel.SelectedItem._title;
                detail.Text = ViewModel.SelectedItem._description;
                date.Date = ViewModel.SelectedItem._date;
                picture.Source = ViewModel.SelectedItem.img;
            }
        }

        private async void ShareItem(object sender, RoutedEventArgs e)
        {
            var s = sender as FrameworkElement;
            var item = (Models.TodoItem)s.DataContext;
            s_title = item._title;
            s_detail = item._description;
            var date = item._date;
            s_date = "\n" + date.Year + '-' + date.Month + '-' + date.Day;

            s_img = await Package.Current.InstalledLocation.GetFileAsync("Assets\\300p.png");

            DataTransferManager.ShowShareUI();
        }
        
        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            DataPackage requestData = request.Data;
            requestData.Properties.Title = s_title;
            request.Data.Properties.Description = s_detail;
            requestData.SetText(s_detail + s_date);

            DataRequestDeferral deferral = request.GetDeferral();

            requestData.SetBitmap(RandomAccessStreamReference.CreateFromFile(s_img));

            deferral.Complete();
        }

        private async void SearchItems(object sender, RoutedEventArgs e)
        {
            var str = SearchBox.Text;
            if (str == "")
            {
                return;
            }

            StringBuilder message = new StringBuilder("");
            
            using (var statement = conn.Prepare("SELECT date, title, detail FROM TodoItem WHERE date LIKE ? OR title LIKE ? OR detail LIKE ?"))
            {
                statement.Bind(1, "%%" + str + "%%");
                statement.Bind(2, "%%" + str + "%%");
                statement.Bind(3, "%%" + str + "%%");
                while (SQLiteResult.DONE != statement.Step())
                {
                    var date = statement[0].ToString();
                    string title = (string)statement[1];
                    string detail = (string)statement[2];
                    message.Append("Title: " + title + "; Detail: " + detail + "; Date: " + statement[0].ToString() + "\n");
                }
                if (message.Equals(""))
                    message.Append("Nothing!\n");
                
                await new MessageDialog(message.ToString()).ShowAsync();
            }
        }
    }
}
