using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using SQLitePCL;

namespace MyList.Models
{
    public class TodoItem : INotifyPropertyChanged
    {
        private SQLiteConnection conn = App.conn;
        public string id;
        public string img_p;
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



        public TodoItem(DateTime date, ImageSource img, string title = "", string description = "", bool finish = false, string imgp = "")
        {
            this.id = Guid.NewGuid().ToString();
            this._date = date;
            this._title = title;
            this._description = description;
            this._finish = finish;
            this.img_p = imgp; 
            this.img = (img == null ? new BitmapImage(new Uri("ms-appx:///Assets/candy.jpg")) : img);
        }

        public void UpdateItem(string id, DateTime date, ImageSource img, string title, string detail, bool isFinished, string imgPath)
        {
            this._date = date;
            this.img = img;
            this._title = title;
            this._description = detail;
            this._finish = isFinished;

            if(imgPath != "")
            {
                this.img_p = imgPath;
            }

            using (var temp = conn.Prepare(@"UPDATE TodoItem SET title = ?, date = ?, detail = ?, finish = ?, img = ? WHERE id = ?"))
            {
                temp.Bind(1, title);
                temp.Bind(2, date.Date.ToString());
                temp.Bind(3, detail);
                temp.Bind(4, isFinished.ToString());
                temp.Bind(5, img_p);
                temp.Bind(6, id);
                temp.Step();
            }
        }

        public void DeleteItem()
        {
            this.id = null;
            this._title = null;
            this._description = null;
            this._img = null;
            this._finish = false;
            this._date = new DateTime(0001, 1, 1);
            this.img_p = null;
        }
    }
}
