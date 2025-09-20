using System;

namespace Klase
{
    [Serializable]
    public class Pacijent
    {
        public int LBO { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Adresa { get; set; }
        public string VrstaZahteva { get; set; }
        public string StatusPacijenta { get; set; }

        public Pacijent(int lBO, string ime, string prezime, string adresa, string vrstaZahteva, string statusPacijenta)
        {
            LBO = lBO;
            Ime = ime;
            Prezime = prezime;
            Adresa = adresa;
            VrstaZahteva = vrstaZahteva;
            StatusPacijenta = statusPacijenta;
        }

        public Pacijent()
        {
            // prazan konstruktor 
        }

        public void Ispisi()
        {
            Console.WriteLine($"LBO: {LBO}");
            Console.WriteLine($"Ime: {Ime}");
            Console.WriteLine($"Prezime: {Prezime}");
            Console.WriteLine($"Adresa: {Adresa}");
            Console.WriteLine($"Vrsta zahteva: {VrstaZahteva}");
            Console.WriteLine($"Status pacijenta: {StatusPacijenta}");
        }

    }
}
