using Enumeracije;
using System;

namespace Klase
{
    [Serializable]
    public class Jedinica
    {
        public TipJedinice TipJedinice { get; set; }
        public int IDjedinice { get; set; }
        public Zauzece Status { get; set; }

        public Jedinica() { }

        public Jedinica(TipJedinice tipJedinice, int iDjedinice, Zauzece status)
        {
            TipJedinice = tipJedinice;
            IDjedinice = iDjedinice;
            Status = status;
        }
    }
}
