using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




internal class Highlight
{
    public List<Kill> Kills = new List<Kill>();
    public int StartTick { get; private set; }
    public int EndTick { get; private set; }

    public int RoundNumber { get; private set; }

    public string PlayerName { get; set; }

    public float HighlightEvaluation { get; set; }

    public Highlight(int roundNumber, string playerName)
    {
        RoundNumber = roundNumber;
        PlayerName = playerName;
    }

    public void AddKill(Kill kill)
    {
        foreach(var k in Kills)
        {
            if(k.RoundNumber == RoundNumber)
            {
                Kills.Add(kill);
                return;
            }
        }
        Kills.Add(kill);
    }

    public void PrintHighlight()
    {
        Console.WriteLine();
        Console.WriteLine("::::" + PlayerName + "::::");
        Console.WriteLine("::::" + HighlightEvaluation + "::::");
        Console.WriteLine("::::" + "ROUND " + RoundNumber + "::::");
        foreach(var k in Kills)
        {
            Console.Write(k.KillerName + "(" + k.WeaponName + ") -> " + k.VictimName + "[" + k.killEvaluation + "]");
            if (k.IsNoscope) Console.Write("*NOSCOPE* \n");
            Console.Write("\n");
        }
    }

    public double EvaluateHighlight()
    {
        float eval = 0;
        Kill k, k2;
        k2 = null;
        for(int i = 0; i < Kills.Count; i++)
        {
            k = Kills[i];
            if (Kills.Count > 1 && k2 == null)
            {
                k2 = Kills[1];
            }
            if (k2 != null)
            {
                if (SubstractSmallerNumber(k.TickNumber, k2.TickNumber) < 500)
                {
                    //quick multikill territory?
                    eval += 0.5f;
                    k2 = k;
                }
            }
            eval += k.killEvaluation;
        }
        HighlightEvaluation = eval;
        return eval;
    }


    double SubstractSmallerNumber(double num1, double num2)
    {
        if (num1 < num2) return num2 - num1;
        else return num1 - num2;
    }
}

