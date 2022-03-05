using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using cs_TcpClient;

namespace cs_TcpServer;

class Program
{
    private static BinaryReader? _reader;
    private static BinaryWriter? _writer;

    static void Main()
    {
        var ip = IPAddress.Parse("127.0.0.1");
        var port = 45678;

        var listener = new TcpListener(ip, port);
        listener.Start();

        var client = listener.AcceptTcpClient();

        var stream = client.GetStream();

        _reader = new BinaryReader(stream);
        _writer = new BinaryWriter(stream);

        FromClient();

    }

    public static void FromClient()
    {
        while (true)
        {
            Command? command = JsonSerializer.Deserialize<Command>(_reader.ReadString());

            switch (command.Text)
            {
                case Command.PROCLIST:
                    var processes = Process.GetProcesses();
                    string data = null;
                    foreach (var process in processes)
                    {
                        data += "\n" + process.ProcessName;
                    }
                    _writer.Write(data);
                    Console.WriteLine($"\'{Command.PROCLIST}\' was processed successfully");
                    break;
                case Command.KILL:
                    try
                    {
                        foreach (var process in Process.GetProcessesByName(command.Param))
                        {
                            process.Kill();
                        }
                        _writer.Write($"{command.Param} process successfully killed");
                    }
                    catch (Exception e)
                    {
                        _writer.Write(e.ToString());
                    }
                    break;
                case Command.RUN:
                    try
                    {
                        var p = new Process();
                        p.StartInfo = new ProcessStartInfo(command.Param);
                        p.Start();
                        _writer.Write($"{command.Param} process running");
                    }
                    catch
                    {
                        _writer.Write("Error");
                    }
                    break;
            }
        }
    }
}