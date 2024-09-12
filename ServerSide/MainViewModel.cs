using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ServerSide;

public class MainViewModel:INotifyPropertyChanged
{
    private ImageSource _imageOpenClose;
    public ImageSource ImageOpenClose
    {
        get
        {
            return _imageOpenClose;
        }
        set
        {
            _imageOpenClose = value;
            OnPropertyChanged(nameof(ImageOpenClose));
        }
    }
    static TcpListener listener;
    public MainViewModel()
    {
        listener = new TcpListener(IPAddress.Parse("10.2.13.5"), 2701);
        listener.Start();
         //TcpClient client = listener.AcceptTcpClient();

        Task.Run(() => { AcceptClientAsync(); });
    }

    private async Task AcceptClientAsync()
    {
      while (true)
        {
            using (TcpClient client = listener.AcceptTcpClient())
            {
                using (var stream = client.GetStream())
                {
                
                    using (var target = new FileStream($"received_image1.jpeg{DateTime.Now:yyyyMMdd_HHmmss}", FileMode.Create, FileAccess.Write))
                    {
                        byte[] buffer = new byte[10000];
                        int bytesRead;
                        do
                        {
                            bytesRead = stream.Read(buffer, 0, buffer.Length);
                            target.Write(buffer, 0, bytesRead);
                        }
                        while (bytesRead > 0);
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            BitmapImage bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.UriSource = new Uri("received_image1.jpeg{DateTime.Now:yyyyMMdd_HHmmss}", UriKind.Relative);
                            bitmap.EndInit();
                            ImageOpenClose = bitmap;
                        });
                    }
                }
            }
        }
    }

    

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
