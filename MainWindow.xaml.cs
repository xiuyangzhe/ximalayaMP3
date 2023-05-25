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
        public MainWindow()
        {
            InitializeComponent();

            string downloadsPath = KnownFolders.Downloads.Path;
            downloadAddress.Text = downloadsPath;
        }

        private void audioGet_Click(object sender, RoutedEventArgs e)
        {
            listView.ItemsSource = null;
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
                listView.ItemsSource = null;
                var client = new RestClient();

                var request = new RestRequest($"https://www.ximalaya.com/revision/album/v1/getTracksList?albumId={albumId.Text}&pageNum=1&pageSize=30");

                // The cancellation token comes from the caller. You can still make a call without it.
                var result = await client.GetAsync<ResultData<AlbumData>>(request);

                var list = new ObservableCollection<TrackInfoDetail>();

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

        private async void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var detail = listView.SelectedItem as TrackInfoDetail;
            if (detail != null) {
                var src = await getAudio(detail.TrackId + "");
                if (src != null) 
                {
                    var client = new RestClient();

                    var request = new RestRequest(src);

                    // The cancellation token comes from the caller. You can still make a call without it.
                    var result = await client.DownloadDataAsync(request);
                    var path = System.IO.Path.Combine(downloadAddress.Text, detail.Title + ".m4a");
                    File.WriteAllBytes(path, result);
                }
            }
            

        }
    }
}
