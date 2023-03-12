using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;


internal class Kill
{

    public struct KillInfo
    {
        public bool isNoScope;
        public bool isHeadShot;
        public double distance;
        public string killerName;
        public string victimName;
        public string weaponName;
        public int roundNumber;
        public int tickNumber;
    }

    public string KillerName { get; private set; }
    public string VictimName { get; private set; }
    public string WeaponName { get; private set; }
    public int TickNumber { get; private set; }
    public bool IsNoscope { get; private set; }
    public bool IsHeadshot { get; private set; }
    public bool IsSmokeKill { get; private set; }
    public double Distance { get; private set; }

    public int RoundNumber { get; private set; }

    public float killEvaluation;


    private const float DIST_THRESHOLD = 1500f;

    public Kill(KillInfo killData)
    {
        KillerName = killData.killerName;
        VictimName = killData.victimName;
        WeaponName = killData.weaponName;
        TickNumber = killData.tickNumber;
        IsNoscope = killData.isNoScope;
        Distance = killData.distance;
        IsHeadshot = killData.isHeadShot;
        RoundNumber = killData.roundNumber;
        killEvaluation = Evaluate();
    }


    public float Evaluate()
    {
        float eval = 1f; //base kill 1
        if(IsNoscope)
        {
            eval += 1;
            if(Distance > DIST_THRESHOLD)
            {
                eval += 1.5f;
            }
        }
        if(Distance > DIST_THRESHOLD)
        {
            eval += 0.5f;
        }
        if(IsSmokeKill)
        {
            eval += 0.25f;
        }
        if(IsHeadshot)
        {
            eval += 0.5f;
            if(Distance > DIST_THRESHOLD)
            {
                eval += 0.5f;
            }
        }

        return eval;
    }

}

