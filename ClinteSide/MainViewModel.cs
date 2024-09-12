using Microsoft.Win32;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace ClinteSide;

class MainViewModel : INotifyPropertyChanged
{
    private string ip = null!;
    private int port;
    private string image=null!;

    public string Ip { get => ip; set { ip = value; OnPropertyChanged(); } }
    public int Port { get => port; set { port = value; OnPropertyChanged(); } }
    public string Image { get => image; set { image = value; OnPropertyChanged(); } }
    public RelayCommand SelectFileCommand { get; set; }
    public RelayCommand Send{ get; set; }
    private string SelectFile()
    {
        var dlg = new OpenFileDialog()
        {
            Filter = "JPEG Files (*.jpeg;*.jpg) | *.jpeg;*.jpg",
            RestoreDirectory = true
        };

        if (dlg.ShowDialog() != true)
            return "";
        else return dlg.FileName;
    }
    

    public MainViewModel()
    {
        SelectFileCommand = new(executeFile);
        Send = new(executeSend);
    }

   

    private void executeSend(object obj)
    {
        using var client = new TcpClient();
        var ip = IPAddress.Parse(Ip);
        var ep = new IPEndPoint(ip, Port);
        try
        {
            client.Connect(ep);
            if (client.Connected)
            {
                using (var stream = client.GetStream()) 
                {
                    using (var source = new FileStream(Image, FileMode.Open, FileAccess.Read))
                    {
                        int len = 10000;
                        var bytes= new byte[len];
                        var fileSize= source.Length;
                        do
                        {
                            len=source.Read(bytes, 0, len);
                            stream.Write(bytes, 0, len);
                      
                         

                        }
                        while (len > 0);
                        
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred: {ex.Message}\n{ex.StackTrace}");
        }
    }


    private void executeFile(object obj)
    {
        Image = SelectFile();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
