using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal class yPlayer
{

    public List<Highlight> Highlights = new List<Highlight>();
    public List<Kill> Kills { get; set; }

    public string Name { get; set; }
    public yPlayer(string name)
    {
        Name = name;
    }

    public void AddKill(Kill k)
    {
        foreach (Highlight h in Highlights)
        {
            if(h.RoundNumber == k.RoundNumber)
            {
                h.AddKill(k); 
                return;
            }
        }
        StartHighlight(k);
    }

    public void StartHighlight(Kill k)
    {
        Highlight h = new Highlight(k.RoundNumber, Name);
        h.AddKill(k);
        Highlights.Add(h);
    }

    public void PrintHighlights()
    {
        foreach (Highlight h in Highlights)
        {
            Console.WriteLine("Round #" + h.RoundNumber + " :: START TICK: " + h.StartTick + " :: EVAL: " + h.HighlightEvaluation);
            for(int i = 0; i < h.Kills.Count; i++)
            {
                Console.WriteLine(h.Kills[i].KillerName + "(" + h.Kills[i].WeaponName + ") -> " + h.Kills[i].VictimName + "[" + h.Kills[i].killEvaluation + "]");
            }
            Console.WriteLine("::::::::::::::::::::::::::::::::");
        }
    }

}

