using Enumeracije;
using System;

namespace Klase
{
    [Serializable]
    public class Zahtev
    {
        public int IDpacijenta { get; set; }
        public int IDjedinice { get; set; }
        public StatusZahteva Status { get; set; }
        public Zahtev() { }
        public Zahtev(int iDpacijenta, int iDjedinice, StatusZahteva status)
        {
            IDpacijenta = iDpacijenta;
            IDjedinice = iDjedinice;
            Status = status;
        }
    }
}
