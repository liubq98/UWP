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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace MyList
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NewPage : Page
    {
        ViewModels.TodoItemViewModel ViewModel { get; set; }
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
                ViewModel.AddTodoItem(date.Date.DateTime, picture.Source, title.Text, detail.Text);

                detail.Text = "";
                title.Text = "";
                date.Date = DateTime.Today;
                picture.Source = new BitmapImage(new Uri("ms-appx:///Assets/candy.jpg"));

                ViewModel.SelectedItem = null;
                await new MessageDialog("Create successfully!").ShowAsync();

                Frame.Navigate(typeof(MainPage),ViewModel);
            }
            else
            {
                ViewModel.SelectedItem.UpdateItem(date.Date.DateTime, picture.Source, title.Text, detail.Text);
                ViewModel.SelectedItem = null;

                await new MessageDialog("Update successfully!").ShowAsync();
                
                Frame.Navigate(typeof(MainPage), ViewModel);
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
                picture.Source = new BitmapImage(new Uri("ms-appx:///Assets/todo.png"));
            }

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

            ViewModel = e.Parameter as ViewModels.TodoItemViewModel;
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

        private void SelectPicture(object sender, RoutedEventArgs e)
        {
            var common = new Common();
            common.SelectPicture(picture);
        }

        private async void DeleteAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AllItems.Remove(ViewModel.SelectedItem);
            ViewModel.SelectedItem = null;
            await new MessageDialog("Delete successfully！").ShowAsync();
            Frame.Navigate(typeof(MainPage), ViewModel);
        }
    }
}
