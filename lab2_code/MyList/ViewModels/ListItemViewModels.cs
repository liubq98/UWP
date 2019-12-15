using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using System.Xml.Linq;
using MyList.Services;
using Windows.ApplicationModel.DataTransfer;
using SQLitePCL;

namespace MyList.ViewModels
{
    public class TodoItemViewModel
    {
        private SQLiteConnection conn = App.conn;
        private static TodoItemViewModel instance;
        public  static TodoItemViewModel getInstance()
        {
            if(instance == null)
            {
                instance = new TodoItemViewModel();
            }
            return instance;
        }

        private ObservableCollection<Models.TodoItem> allItems = new ObservableCollection<Models.TodoItem>();
        public ObservableCollection<Models.TodoItem> AllItems { get { return this.allItems; } }

        private Models.TodoItem selectedItem;
        public Models.TodoItem SelectedItem
        {
            get { return selectedItem; }
            set { this.selectedItem = value; }
        }

        public void AddTodoItem(DateTime date, ImageSource img, string title, string description, bool finish = false, string imgp = "")
        {
             AllItems.Add(new Models.TodoItem(date, img, title, description, finish, imgp));

            var xmlDoc = TileService.CreateTiles(new Models.TodoItem(date, img, title, description, finish, imgp));

            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            TileNotification notification = new TileNotification(xmlDoc);
            updater.Update(notification);
            updater.EnableNotificationQueue(true);

            TileService.SetBadgeCountOnTile(AllItems.Count());
        }


        public TodoItemViewModel()
        {
            this.SelectedItem = null;
            using (var statement = conn.Prepare("SELECT id, title, date, detail, finish, img FROM TodoItem"))
            {
                while (SQLiteResult.DONE != statement.Step())
                {
                    string path = Convert.ToString(statement[5]);
                    ImageSource img = (path == "" ? new BitmapImage(new Uri("ms-appx:///Assets/candy.jpg")) : new BitmapImage(new Uri("ms-appx:///Assets/" + path)));
                    int temp = Convert.ToInt32(statement[0]);
                    SelectedItem = new Models.TodoItem(Convert.ToDateTime((string)statement[2]), img, (string)statement[1], (string)statement[3], Convert.ToBoolean((string)statement[4]), path);
                    SelectedItem.id = temp.ToString();
                    AllItems.Add(SelectedItem);
                }
            }
            this.SelectedItem = null;
        }

        public void RemoveTodoItem(string id)
        {
            using (var statement = conn.Prepare("DELETE FROM TodoItem WHERE id = ?"))
            {
                statement.Bind(1, Convert.ToInt32(id));
                statement.Step();
            }

            for(int i = 0; i < AllItems.Count(); i++)
            {
                if(AllItems[i].id == id)
                {
                    AllItems.Remove(AllItems[i]);
                }
            }
            this.selectedItem = null;
        }

        public void UpdateTodoItem(string id, DateTime date, ImageSource img, string title, string description, string imgPath)
        {
            bool isFinished = false;
            for (int i = 0; i < AllItems.Count(); i++)
            {
                if (AllItems[i].id == id)
                {
                    isFinished = AllItems[i]._finish;
                }
            }
            this.SelectedItem.UpdateItem(id, date, img, title, description, isFinished, imgPath);
            this.selectedItem = null;
        }
    }
}
