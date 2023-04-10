using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoInfo;


internal class Program
{

    public static List<string> demos = new List<string>();
    public static string currentDemoFileName;
    public static DemoParser currentDemo;
    public static int currentGameRoundNumber = 0;

    public static List<yPlayer> playerList = new List<yPlayer>();  

    public static List<Highlight> highlightList = new List<Highlight>();

    public static bool isGameLive = false;

    public static int currentScore;

    static void Main(string[] args)
    {
        demos = GetAllDemoFiles();
        ParseHeader();
    }


    public static void ParseHeader()
    {

        for (int i = 0; i < demos.Count; i++)
        {
            Console.WriteLine(demos[i] + "" + (i + 1));
            FileStream fs = File.Open(demos[i], FileMode.Open);
            currentDemoFileName = demos[i];
            currentDemo = new DemoParser(fs);
            //uniquePlayers = new List<yPlayer>();
            RegisterEvents();
            currentGameRoundNumber = 0;
            currentDemo.ParseHeader();
            AnalyzeDemo();
        }
        //PrintInfo();

        foreach(yPlayer p in playerList)
        {
            foreach(Highlight h in p.Highlights)
            {
                h.EvaluateHighlight();
                highlightList.Add(h);
            }
        }
        List<Highlight> bestHighlights = highlightList.OrderByDescending(o => o.HighlightEvaluation).ToList();

        for(int i = 0; i < 5; i++)
        {
            bestHighlights[i].PrintHighlight();
        }

        /*double bestEval = 0f;
        Highlight bestHighlight = null;


        foreach (yPlayer p in playerList)
        {
            foreach(Highlight h in p.Highlights)
            {
                if(bestHighlight == null)
                {
                    bestHighlight = h;
                }


                bestEval = h.EvaluateHighlight();
                if(bestEval > bestHighlight.EvaluateHighlight())
                {
                    bestHighlight = h;
                    bestEval = h.EvaluateHighlight();
                }
            }
        }

        bestHighlight.PrintHighlight();*/


        Console.ReadKey();
        Environment.Exit(0);
    }

    public static void AnalyzeDemo()
    {
        while (true)
        {
            if (!currentDemo.ParseNextTick()) break;
        }
    }

    public static void RegisterEvents()
    {
        currentDemo.PlayerKilled += CurrentDemo_PlayerKilled;
        currentDemo.PlayerBuy += CurrentDemo_PlayerBuy;
        currentDemo.RoundOfficiallyEnd += CurrentDemo_RoundOfficiallyEnd;
        currentDemo.GamePhaseChanged += CurrentDemo_GamePhaseChanged;
    }

    private static void CurrentDemo_GamePhaseChanged(object sender, GamePhaseChangedArgs e)
    {
        if (e.NewGamePhase == GamePhase.GameEnded)
        {
            isGameLive = false;
        }
    }

    private static void CurrentDemo_RoundOfficiallyEnd(object sender, RoundOfficiallyEndedEventArgs e)
    {
        currentGameRoundNumber++;
    }

    private static void CurrentDemo_PlayerBuy(object sender, PlayerBuyEventArgs e)
    {
        if (e.Player.Money < 1000 && !isGameLive)
        {
            isGameLive = true;
        } else
        {
            return; //basically check for pistol round
        }

        //Console.WriteLine("GAME OFFICAL START");
        yPlayer p;

        for (int i = 0; i < currentDemo.Participants.Count(); i++)
        {
            bool found = false;
            p = new yPlayer(currentDemo.Participants.ElementAt(i).Name);
            for (int j = 0; j < playerList.Count(); j++)
            {
                if (playerList[j].Name == p.Name)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                playerList.Add(p);
            }
        }
    }

    private static void CurrentDemo_PlayerKilled(object sender, PlayerKilledEventArgs e)
    {
        if (currentGameRoundNumber == 0) return;
        if (e.Killer == null || e.Victim == null) return;
        if (e.Killer.Team == e.Victim.Team) return;
        if (e.Weapon.Weapon == EquipmentElement.Bomb || e.Weapon.Weapon == EquipmentElement.Unknown) return;

        bool nosc = false;
        if(e.Weapon.Weapon == EquipmentElement.AWP || e.Weapon.Weapon == EquipmentElement.Scout)
        {
            if(!e.Killer.IsScoped)
            {
                nosc = true;
            }
        }

        double dist = Math.Sqrt(
               Math.Pow(e.Victim.LastAlivePosition.X - e.Killer.Position.X, 2)
             + Math.Pow(e.Victim.LastAlivePosition.Y - e.Killer.Position.Y, 2)
             + Math.Pow(e.Victim.LastAlivePosition.Z - e.Killer.Position.Z, 2));

        Kill.KillInfo killData = new Kill.KillInfo {
            distance = dist,
            isNoScope = nosc,
            victimName = e.Victim.Name,
            killerName = e.Killer.Name,
            tickNumber = currentDemo.IngameTick,
            roundNumber = currentGameRoundNumber,
            weaponName = e.Weapon.Weapon.ToString(),
            isHeadShot = e.Headshot
        };
        Kill kill = new Kill(killData);
        foreach(yPlayer p in playerList)
        {
            if(p.Name == e.Killer.Name)
            {
                p.AddKill(kill);
            }
        } 
        //float eval = kill.Evaluate();
        //Console.WriteLine(killerName + "(" + weapon + ") -> " + victimName + "| Distance: " + dist + ":::EVALUATION:" + eval.ToString("F2"));
    }

    public static List<string> GetAllDemoFiles()
    {
        string[] files = Directory.GetFiles(Directory.GetCurrentDirectory());
        List<string> fileList = new List<string>();
        foreach (string s in files)
        {
            if (s.Contains(".dem"))
            {
                fileList.Add(s);
            }
        }
        return fileList;
    }
}

