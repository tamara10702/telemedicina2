using Klase;
using System.Collections.Generic;
using System;

[Serializable]
public class PaketPacijenata
{
    public List<Pacijent> Urgentni { get; set; }
    public List<Pacijent> Ostali { get; set; }
}