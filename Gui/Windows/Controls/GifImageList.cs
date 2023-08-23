using GifFingTool.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GifFingTool.Gui.Windows.Controls
{
    internal class GifImageList
    {
        private ObservableCollection<GifBitmap> TestList = new ObservableCollection<GifBitmap>();

        public GifImageList(DataGrid dataGrid)
        {
            dataGrid.Columns.Add(new PreviewImageDisplayColumn());
            dataGrid.ItemsSource = TestList;
        }

        public void InsertBitmap(int index, GifBitmap bitmap)
        {
            TestList.Insert(index, bitmap);
        }

        public void AttemptRefreshDisplays(GifBitmap gifBitmap)
        {
            int index = TestList.IndexOf(gifBitmap);
            if (index >= 0)
            {
                //TODO: Replace this by making the DataGrid properly process the INotifyPropertyChanged implementation of GifBitmap
                TestList.RemoveAt(index);
                TestList.Insert(index, gifBitmap);
            }
        }

        public void AddBitmap(GifBitmap bitmap)
        {
            TestList.Add(bitmap);
        }

        public void RefreshGifBitmap(int index)
        {
            TestList[index].Refresh();
        }

        public List<GifBitmap> GetCurrentList()
        {
            return TestList.ToList();
        }

        public bool IsEmpty()
        {
            return TestList.Count == 0;
        }

        public int Count { get => TestList.Count; }
    }
}
