using Klase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace telemedicina2
{
    public class Klijent
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Klijent je pokrenut!");
            Socket klijentSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 50002);

            klijentSocket.Connect(serverEP);
            Console.WriteLine("Klijent uspešno povezan sa serverom!");

            BinaryFormatter bf = new BinaryFormatter();

            PaketPacijenata Pacijenti = new PaketPacijenata
            {
                Urgentni = new List<Pacijent>(),
                Ostali = new List<Pacijent>()
            };

            while (true)
            {
                Console.Write("Unesite LBO: ");
                int LBO = Convert.ToInt32(Console.ReadLine());

                Console.Write("Ime pacijenta: ");
                string imePacijenta = Console.ReadLine();

                Console.Write("Prezime pacijenta: ");
                string prezimePacijenta = Console.ReadLine();

                Console.Write("Adresa pacijenta: ");
                string adresaPacijenta = Console.ReadLine();

                Console.Write("Izaberite uslugu (terapija, dijagnostika, urgentna pomoć): ");
                string izbor = Console.ReadLine();

                if (izbor.ToLower() == "terapija" || izbor.ToLower() == "dijagnostika" || izbor.ToLower() == "urgentna pomoc" || izbor.ToLower() == "urgentna pomoć")
                {
                    Pacijent pacijent = new Pacijent
                    {
                        LBO = LBO,
                        Ime = imePacijenta,
                        Prezime = prezimePacijenta,
                        Adresa = adresaPacijenta,
                        VrstaZahteva = izbor,
                    };

                    if (pacijent.VrstaZahteva.ToLower() == "urgentna pomoc" || pacijent.VrstaZahteva.ToLower() == "urgentna pomoć")
                    {
                        Pacijenti.Urgentni.Add(pacijent);
                    }
                    else
                    {
                        Pacijenti.Ostali.Add(pacijent);
                    }

                    if (Pacijenti.Urgentni.Count + Pacijenti.Ostali.Count == 5)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            bf.Serialize(ms, Pacijenti);
                            byte[] data = ms.ToArray();
                            klijentSocket.Send(data);
                        }

                        Console.WriteLine("Paketi sa 5 pacijenata poslati serveru!");

                        Pacijenti = new PaketPacijenata
                        {
                            Urgentni = new List<Pacijent>(),
                            Ostali = new List<Pacijent>()
                        };
                    }

                    Console.WriteLine("Da li želite da unesete još pacijenata ? (da / ne)");
                    string odgovor = Console.ReadLine().ToLower();
                    if (odgovor != "da")
                        break;
                }
                else
                {
                    Console.WriteLine("Nedostupna usluga. Unesite podatke ponovo.");
                    continue;
                }
            }

            if (Pacijenti.Urgentni.Count + Pacijenti.Ostali.Count > 0)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bf.Serialize(ms, Pacijenti);
                    byte[] data = ms.ToArray();
                    klijentSocket.Send(data);
                }
                Console.WriteLine("Preostali pacijenti poslati serveru!");
            }

            Console.WriteLine("Klijent završava sa radom.");
            klijentSocket.Close();
            Console.ReadKey();
        }
    }
}
