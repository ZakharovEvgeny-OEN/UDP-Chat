using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDPChat
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.Write("Введите свое имя: ");
            string? username = Console.ReadLine();

            Console.Write("Введите порт для приема сообщений: ");
            if (!int.TryParse(Console.ReadLine(), out var localPort)) return;

            Console.Write("Введите IP-адрес получателя: ");
            if (!IPAddress.TryParse(Console.ReadLine(), out var remoteAddress))
            {
                Console.WriteLine("Неверный IP-адрес!");
                return;
            }

            Console.Write("Введите порт для отправки сообщений: ");
            if (!int.TryParse(Console.ReadLine(), out var remotePort)) return;
            Console.WriteLine();

            Task.Run(ReceiveMessageAsync);
            await SendMessageAsync();

            async Task SendMessageAsync()
            {
                using Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                Console.WriteLine("Для отправки сообщений введите сообщение и нажмите Enter");

                while (true)
                {
                    var message = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(message)) break;

                    message = $"{username}: {message}";
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    await sender.SendToAsync(data, new IPEndPoint(remoteAddress, remotePort));
                }
            }

            async Task ReceiveMessageAsync()
            {
                byte[] data = new byte[65535];
                using Socket receiver = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                receiver.Bind(new IPEndPoint(IPAddress.Any, localPort));

                while (true)
                {
                    var result = await receiver.ReceiveFromAsync(data, new IPEndPoint(IPAddress.Any, 0));
                    var message = Encoding.UTF8.GetString(data, 0, result.ReceivedBytes);
                    Console.WriteLine(message);
                }
            }
        }
    }
}
