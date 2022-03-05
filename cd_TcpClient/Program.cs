using System.ComponentModel.Design;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;

namespace cs_TcpClient;

class Program
{
    private static string _jsonString;
    private static BinaryWriter _writer;
    private static BinaryReader _reader;
    private static string[] _subs;
    private static Command _command;

    static void Main()
    {
        var client = new TcpClient();
        client.Connect("127.0.0.1", 45678);

        var stream = client.GetStream();

        _writer= new BinaryWriter(stream);
        _reader= new BinaryReader(stream);

        Menu();
    }

    private static void ToListener()
    {
       _writer.Write(_jsonString);
    }

    private static void FromListener()
    {
        Console.WriteLine(_reader.ReadString());
    }

    private static void Menu()
    {

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Use \"help\" command in order to get command list");
            Console.Write(">>");
            _subs = Console.ReadLine().Split(' ');
            if (_subs[0].ToLower() == "help")
            {
                Console.WriteLine($"1) {Command.PROCLIST}\n2) {Command.KILL} <process name>\n3) {Command.RUN}  <process name>");
            }
            Console.Write(">>");
            switch (_subs[0].ToLower())
            {
                case Command.PROCLIST:
                    _command = new Command
                    {
                        Text = Command.PROCLIST,
                        Param = ""
                    };
                    break;
                case Command.KILL:
                    _command = new Command
                    {
                        Text = Command.KILL,
                        Param = _subs[1]
                    };
                    break;
                case Command.RUN:
                    _command = new Command
                    {
                        Text = Command.RUN,
                        Param = _subs[1]
                    };
                    break;
                default:
                    Console.WriteLine("Enter Correct Command");
                    Thread.Sleep(1500);
                    continue;
            }
            _jsonString = JsonSerializer.Serialize(_command);
            ToListener();
            FromListener();
            Console.ReadLine();
        }

    }
}