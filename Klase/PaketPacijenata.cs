using Klase;
using System;
using System.Collections.Generic;

[Serializable]
public class PaketPacijenata
{
    public List<Pacijent> Urgentni { get; set; }
    public List<Pacijent> Ostali { get; set; }
}