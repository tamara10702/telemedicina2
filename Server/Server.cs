using Enumeracije;
using Klase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace Server
{
    public class Server
    {
        static Jedinica urgentna = new Jedinica(TipJedinice.Urgentna, 1, Zauzece.Slobodna);
        static Jedinica terapeutska = new Jedinica(TipJedinice.Terapeutska, 2, Zauzece.Slobodna);
        static Jedinica dijagnosticka = new Jedinica(TipJedinice.Dijagnosticka, 3, Zauzece.Slobodna);
        static List<Jedinica> jedinice = new List<Jedinica> { urgentna, terapeutska, dijagnosticka };
        static List<Pacijent> pacijentiZaIspis = new List<Pacijent>();
        static void Main(string[] args)
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverEP = new IPEndPoint(IPAddress.Any, 50002);

            serverSocket.Bind(serverEP);
            serverSocket.Listen(5);

            Console.WriteLine("Server je u stanju slušanja");

            Socket acceptedSocket = serverSocket.Accept();

            IPEndPoint clientEP = acceptedSocket.RemoteEndPoint as IPEndPoint;
            Console.WriteLine($"Povezan novi klijent, adresa {clientEP}");

            byte[] buffer = new byte[4096];
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            while (true)
            {
                try
                {
                    int brBajta = acceptedSocket.Receive(buffer);
                    if (brBajta == 0) break;

                    using (MemoryStream ms = new MemoryStream(buffer, 0, brBajta))
                    {
                        PaketPacijenata paket = (PaketPacijenata)binaryFormatter.Deserialize(ms);

                        foreach (var p in paket.Urgentni)
                        {
                            Pacijent odgovor = ProslediJedinici(p, urgentna);
                            pacijentiZaIspis.Add(odgovor);
                        }

                        foreach (var p in paket.Ostali)
                        {
                            Pacijent odgovor = ProslediJedinici(p, (p.VrstaZahteva=="terapija") ? terapeutska : dijagnosticka);
                            pacijentiZaIspis.Add(odgovor);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Greška: {ex.Message}");
                    break;
                }
            }

            Console.WriteLine("Server se zatvara");
            acceptedSocket.Close();
            serverSocket.Close();
            Console.ReadKey();
        }

        static Pacijent ProslediJedinici(Pacijent p, Jedinica jedinica)
        {
            int port = 0;

            switch (p.VrstaZahteva.ToLower())
            {
                case "terapija":
                    port = 60002;
                    break;
                case "dijagnostika":
                    port = 60001;
                    break;
                case "urgentna pomoc":
                    port = 60003;
                    break;
                case "urgentna pomoć":
                    port = 60003;
                    break;
            }

            try
            {
                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                client.Connect(IPAddress.Loopback, port);

                jedinica.Status = Zauzece.Zauzeta;
                IspisiStanje(pacijentiZaIspis, jedinice);

                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream())
                {
                    formatter.Serialize(ms, p);
                    client.Send(ms.ToArray());
                }

                byte[] buffer = new byte[4096];
                int received = client.Receive(buffer);
                using (MemoryStream msIn = new MemoryStream(buffer, 0, received))
                {
                    Pacijent odgovor = (Pacijent)formatter.Deserialize(msIn);

                    jedinica.Status = Zauzece.Slobodna;
                    odgovor.vremeObradeZahteva = DateTime.Now.AddSeconds(0);
                    IspisiStanje(pacijentiZaIspis, jedinice);
                    client.Close();
                    return odgovor;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška: {ex.Message}");
                return p;
            }
        }

        static void IspisiStanje(List<Pacijent> pacijenti, List<Jedinica> jedinice)
        {
            Console.Clear();

            Console.WriteLine("STANJE JEDINICA");
            Console.WriteLine("{0,-15} | {1,-10}", "Tip jedinice", "Status");
            Console.WriteLine(new string('-', 30));

            foreach (var j in jedinice)
            {
                Console.WriteLine("{0,-15} | {1,-10}", j.TipJedinice, j.Status);
            }

            Console.WriteLine();

            Console.WriteLine("STANJE PACIJENATA");
            Console.WriteLine(
                "{0,-10} | {1,-15} | {2,-15} | {3,-20} | {4,-20} | {5, -20}",
                "LBO", "Ime", "Prezime", "Vrsta zahteva", "Status", "Vreme obrade"
            );
            Console.WriteLine(new string('-', 90));

            foreach (var p in pacijenti)
            {
                string status = string.IsNullOrEmpty(p.StatusPacijenta) ? "Nepoznat" : p.StatusPacijenta;
                Console.WriteLine(
                    "{0,-10} | {1,-15} | {2,-15} | {3,-20} | {4,-20} | {5, -20}",
                    p.LBO, p.Ime, p.Prezime, p.VrstaZahteva, status, p.vremeObradeZahteva
                );
            }
        }

    }
}
