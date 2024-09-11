using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

namespace ServerSide;

public class MainViewModel:INotifyPropertyChanged
{
    private string image;

    public string Image 
    { get => image; set
        {
            image = value;
            OnPropertyChanged();
        }
        }
    static TcpListener listener;
    public MainViewModel()
    {
        listener = new TcpListener(IPAddress.Parse("10.1.18.9"), 2701);
        listener.Start();
        var client = new Server.Net.Client(listener.AcceptTcpClient());
        var task = Task.Run(() =>
        {
            Console.WriteLine($"{client.Client.RemoteEndPoint} connected");
           
        });
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
