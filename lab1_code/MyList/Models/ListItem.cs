using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace MyList.Models
{
    public class TodoItem : INotifyPropertyChanged
    {
        private string id;
        private string title { get; set; }
        public string _title
        {
            set
            {
                title = value;
                NotifyPropertyChanged("_title");
            }
            get
            {
                return title;
            }
        }

        public string _description { get; set; }
        public bool _finish { get; set; }
        public DateTime _date { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }

        private ImageSource _img;
        public ImageSource img
        {
            set
            {
                _img = value;
                NotifyPropertyChanged("img");
            }
            get
            {
                return _img;
            }
        }



        public TodoItem(DateTime date, ImageSource img, string title = "", string description = "", bool finish = false)
        {
            this.id = Guid.NewGuid().ToString();
            this._date = date;
            this._title = title;
            this._description = description;
            this._finish = finish;
            this.img = (img == null ? new BitmapImage(new Uri("ms-appx:///Assets/candy.jpg")) : img);
        }

        public void UpdateItem(DateTime date, ImageSource img, string title, string detail)
        {
            this._date = date;
            this.img = img;
            this._title = title;
            this._description = detail;
        }

        public void DeleteItem()
        {
            this.id = null;
            this._title = null;
            this._description = null;
            this._img = null;
            this._finish = false;
            this._date = new DateTime(0001, 1, 1);

        }
    }
}
