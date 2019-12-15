using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace MyList.ViewModels
{
    public class TodoItemViewModel
    {
        private ObservableCollection<Models.TodoItem> allItems = new ObservableCollection<Models.TodoItem>();
        public ObservableCollection<Models.TodoItem> AllItems { get { return this.allItems; } }

        private Models.TodoItem selectedItem;
        public Models.TodoItem SelectedItem
        {
            get { return selectedItem; }
            set { this.selectedItem = value; }
        }

        public void AddTodoItem(DateTime date, ImageSource img, string title, string description)
        {
             AllItems.Add(new Models.TodoItem(date, img, title, description));
        }


        public TodoItemViewModel()
        {
            this.SelectedItem = null;
            AllItems.Add(new Models.TodoItem(DateTime.Today, null, "heiheihei", "test11111111", true));
            AllItems.Add(new Models.TodoItem(DateTime.Today, null, "hehehe", "test22222"));
        }

        public void RemoveTodoItem(string id)
        {
            this.SelectedItem.DeleteItem();
            this.selectedItem = null;
        }

        public void UpdateTodoItem(string id, DateTime date, ImageSource img, string title, string description)
        {
            this.SelectedItem.UpdateItem(date, img, title, description);
            this.selectedItem = null;
        }
    }
}
