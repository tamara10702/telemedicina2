using Klase;
using System;
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

                    using (MemoryStream ms = new MemoryStream())
                    {
                        bf.Serialize(ms, pacijent);
                        byte[] data = ms.ToArray();
                        klijentSocket.Send(data);
                    }

                    Console.WriteLine("Zahtev uspešno poslat!");

                    Console.WriteLine("Da li želite da unesete još jednog pacijenta? (da/ne)");
                    string odgovor = Console.ReadLine().ToLower();
                    if (odgovor != "da")
                        break;
                }
                else
                {
                    Console.WriteLine("Nedostupna usluga. Unesite podatke ponovo.");
                }
            }

            Console.WriteLine("Klijent završava sa radom.");
            klijentSocket.Close();
            Console.ReadKey();
        }
    }
}
