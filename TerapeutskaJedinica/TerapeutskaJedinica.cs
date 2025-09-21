using Klase;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace TerapeutskaJedinica
{
    internal class TerapeutskaJedinica
    {
        static void Main(string[] args)
        {
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new IPEndPoint(IPAddress.Any, 60002));
            listener.Listen(5);
            Console.WriteLine("Terapeutska jedinica je spremna!");

            BinaryFormatter formatter = new BinaryFormatter();

            while (true)
            {
                Socket handler = listener.Accept();
                Console.WriteLine("Server povezan...");

                byte[] buffer = new byte[4096];
                int brBajta = handler.Receive(buffer);

                using (MemoryStream ms = new MemoryStream(buffer, 0, brBajta))
                {
                    Pacijent p = (Pacijent)formatter.Deserialize(ms);
                    Console.WriteLine($"Obrada pacijenta: {p.Ime} {p.Prezime}");

                    System.Threading.Thread.Sleep(3000);

                    p.StatusPacijenta = "pruzenaTerapija";
                    Console.WriteLine("Pacijentu je pružena terapija.");

                    using (MemoryStream msOut = new MemoryStream())
                    {
                        formatter.Serialize(msOut, p);
                        handler.Send(msOut.ToArray());
                    }
                }

                handler.Close();
            }
        }
    }
}

