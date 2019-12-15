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

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace MyList
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ViewModels.TodoItemViewModel ViewModel { get; set; }
        public MainPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            ViewModel = new ViewModels.TodoItemViewModel();
        }

        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (newpage.Visibility == Visibility.Collapsed)
            {
                ViewModel.SelectedItem = null;
                Frame.Navigate(typeof(NewPage), this.ViewModel);
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
            ViewModel.AllItems.Remove(ViewModel.SelectedItem);
            ViewModel.SelectedItem = null;

            detail.Text = "";
            title.Text = "";
            date.Date = DateTime.Today;
            picture.Source = new BitmapImage(new Uri("ms-appx:///Assets/candy.jpg"));

            await new MessageDialog("Delete successfully！").ShowAsync();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame.CanGoBack)
            {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
            }
            else
            {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
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
                this.ViewModel.AddTodoItem(date.Date.DateTime, picture.Source, title.Text, detail.Text);

                detail.Text = "";
                title.Text = "";
                date.Date = DateTime.Today;
                picture.Source = new BitmapImage(new Uri("ms-appx:///Assets/candy.jpg"));

                ViewModel.SelectedItem = null;
                await new MessageDialog("Create successfully!").ShowAsync();
            }
            else
            {
                this.ViewModel.SelectedItem.UpdateItem(date.Date.DateTime, picture.Source, title.Text, detail.Text);

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
                Frame.Navigate(typeof(NewPage), ViewModel);
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
    }
}
