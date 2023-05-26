using Microsoft.WindowsAPICodePack.Shell;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ximalayaMP3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _currentPage = 1;
        private long _totalPageCount = 0;

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                page.Text = value.ToString();
            }
        }

        public long TotalPageCount
        {
            get => _totalPageCount;
            set 
            {
                pageCount.Text = value.ToString();
                _totalPageCount = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            string downloadsPath = KnownFolders.Downloads.Path;
            downloadAddress.Text = downloadsPath;
        }

        private void audioGet_Click(object sender, RoutedEventArgs e)
        {
            listView.ItemsSource = null;
            CurrentPage = 1;
            
            getTrackInfo();
        }

        private async Task<string> getAudio(string id)
        {
            try
            {
                var client = new RestClient();
                var request = new RestRequest($"https://www.ximalaya.com/revision/play/v1/audio?id={id}&ptype=1");
                // The cancellation token comes from the caller. You can still make a call without it.
                var result = await client.GetAsync<ResultData<AudioData>>(request);
                return result.Data.Src;

            }
            catch (Exception ex)
            {
            }
            return null;
        }

        private async void getAlbum()
        {
            try
            {
                var pageSize = 30;
                listView.ItemsSource = null;
                var client = new RestClient();

                var request = new RestRequest($"https://www.ximalaya.com/revision/album/v1/getTracksList?albumId={albumId.Text}&pageNum={CurrentPage}&pageSize={pageSize}");

                // The cancellation token comes from the caller. You can still make a call without it.
                var result = await client.GetAsync<ResultData<AlbumData>>(request);

                var list = new ObservableCollection<TrackInfoDetail>();

                
                total.Text = result.Data.TrackTotalCount.ToString();

                var totalPage = result.Data.TrackTotalCount / pageSize;
                if (result.Data.TrackTotalCount % pageSize > 0)
                {
                    totalPage += 1;
                }
                TotalPageCount = totalPage;
                var no = 1;
                result.Data.Tracks.ForEach(m =>
                {
                    list.Add(new TrackInfoDetail()
                    {
                        No = no++ + "",
                        Title = m.Title,
                        TrackId = m.TrackId
                    });
                });

                listView.ItemsSource = list;

            }
            catch (Exception ex)
            {
            }
        }



        private async void getTrackInfo()
        {
            try
            {
                var client = new RestClient();

                var request = new RestRequest($"https://www.ximalaya.com/revision/track/simple?trackId={audioId.Text}");

                // The cancellation token comes from the caller. You can still make a call without it.
                var result = await client.GetAsync<ResultData<Track>>(request);

                var list = new ObservableCollection<TrackInfoDetail>();
                list.Add(new TrackInfoDetail()
                {
                    No = "1",
                    Title = result.Data.TrackInfo.Title,
                    TrackId = result.Data.TrackInfo.TrackId
                });
                listView.ItemsSource = list;
            }
            catch (Exception ex)
            {
            }
        }

        private void albumGet_Click(object sender, RoutedEventArgs e)
        {
            getAlbum();
        }

        private async void downloadAudio(string id, string title,string end) 
        {
            var src = await getAudio(id);
            if (src != null)
            {
                var client = new RestClient();

                var request = new RestRequest(src);

                // The cancellation token comes from the caller. You can still make a call without it.
                var result = await client.DownloadDataAsync(request);
                if (result == null)
                {
                    //MessageBox.Show("下载失败");
                    return;
                }


                var path = "";
                Dispatcher.Invoke(() =>
                {
                    path = System.IO.Path.Combine(downloadAddress.Text, title + end);
                });
                File.WriteAllBytes(path, result);
            }
        }

        private async void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var detail = listView.SelectedItem as TrackInfoDetail;
            if (detail != null) {
                downloadAudio(detail.TrackId + "", detail.Title,".m4a");
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            var detail = listView.SelectedItem as TrackInfoDetail;
            if (detail != null)
            {
                downloadAudio(detail.TrackId + "", detail.Title, ".mp3");
            }
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            var list = new List<TrackInfoDetail>();
            foreach (var m in listView.ItemsSource)
            {
                list.Add(m as TrackInfoDetail);
            }
            Parallel.ForEach(list, new ParallelOptions() 
            {
                MaxDegreeOfParallelism = 3
            }, detail => 
            {
                downloadAudio(detail.TrackId + "", detail.Title, ".m4a");

            });
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            var list = new List<TrackInfoDetail>();
            foreach (var m in listView.ItemsSource)
            {
                list.Add(m as TrackInfoDetail);
            }
            Parallel.ForEach(list, new ParallelOptions()
            {
                MaxDegreeOfParallelism = 3
            }, detail =>
            {
                downloadAudio(detail.TrackId + "", detail.Title, ".mp3");

            });
        }

        private void lastPage_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage > 1) 
            {
                CurrentPage -= 1;
                getAlbum();
            }
        }

        private void nextPage_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage < TotalPageCount)
            {
                CurrentPage += 1;
                getAlbum();
            }
        }


    }
}
