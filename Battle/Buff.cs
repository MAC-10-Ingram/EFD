using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Buff //: MonoBehaviour
{
    // Start is called before the first frame update
    public int turn { get; set; } = 0;
    public int step = 1;
    [System.NonSerialized]
    public BattleChar user;

    //public float buffAcc { get; set; } = 1.0f;
    //public float buffAvoid { get; set; } = 1.0f;
   // public float buffSpd = 1.0f;
    public float buffMgcATK = 1.0f;
    public float buffMgcDEF = 1.0f;
    public float buffPhyATK = 1.0f;
    public float buffPhyDEF = 1.0f;
    public float buffIncAP = 1.0f;
    public float buffPhyDAM = 1.0f;
    public float buffMgcDAM = 1.0f;
    public float buffCrit = 1.0f;
    public float buffFinalDam = 1.0f;

    public virtual float getMgcDam() {
        return buffMgcDAM;
    }

    public virtual float getPhyDam() {
        return buffPhyDAM;
    }

    public virtual void endTurn() { 

    }

    public virtual int updateTurn() {
        return turn -= step;
    }

    public void addBuff(Buff b)
    {
        buffMgcATK *= b.buffMgcATK;
        buffMgcDEF *= b.buffMgcDEF;
        buffPhyATK *= b.buffPhyATK;
        buffPhyDEF *= b.buffPhyDEF;
        buffIncAP *= b.buffIncAP;
        buffPhyDAM *= b.buffPhyDAM;
        buffMgcDAM *= b.buffMgcDAM;
        buffCrit *= b.buffCrit;
        buffFinalDam *= b.buffFinalDam;
    }

    public void reset() {
        buffMgcATK = 1.0f;
        buffMgcDEF = 1.0f;
        buffPhyATK = 1.0f;
        buffPhyDEF = 1.0f;
        buffIncAP = 1.0f;
        buffPhyDAM = 1.0f;
        buffMgcDAM = 1.0f;
        buffCrit = 1.0f;
        buffFinalDam = 1.0f;
    }
}

public class KnightPas2Buff : Buff {
    int lv;
    public KnightPas2Buff(int lv) {
        this.lv = lv;
    }

    public override float getMgcDam()
    {
        if(user.tauntTurn > 0) if(Random.value > 0.2 + 0.01 * lv) return base.getMgcDam() * (1.0f - (0.3f + 0.03f*lv));

        return base.getMgcDam();
    }

    public override float getPhyDam()
    {
        if (user.tauntTurn > 0) if (Random.value > 0.2 + 0.01 * lv) return base.getPhyDam() * (1.0f - (0.3f + 0.03f * lv));

        return base.getPhyDam();
    }
}

public class KnightPas4Buff : Buff
{
    int lv;
    public KnightPas4Buff(int lv)
    {
        this.lv = lv;
    }

    public override int updateTurn()
    {
        if (user.stateSturn.getTurn() > 0 || user.stateCount.getTurn() > 0) return turn;
        else return base.updateTurn();
    }
}

public class KnightPas5Buff : Buff
{
    int lv;
    public KnightPas5Buff(int lv)
    {
        this.lv = lv;
    }

    public override float getMgcDam()
    {
        float deDamage = (int)( ( user.DC.getMaxHP() - user.DC.curHP) * 100 / (user.DC.getMaxHP() * 5) );

        if (deDamage > 0.3)
        {
            return .7f;
        }
        else {
            return base.getMgcDam() - deDamage * (.02f + .002f * lv);
        }
    }

    public override float getPhyDam()
    {
        float deDamage = (int)((user.DC.getMaxHP() - user.DC.curHP) * 100 / (user.DC.getMaxHP() * 5));

        if (deDamage > 0.3)
        {
            return .7f;
        }
        else
        {
            return base.getPhyDam() - deDamage * (.02f + .002f * lv);
        }
    }
}


public class MagePas2Buff : Buff{
    int lv;
    public MagePas2Buff(int level) {
        lv = level;
    }

    public override int updateTurn()
    {
        user.setMP(user.getMP() + user.DC.getMaxMP() * (0.05f + lv * 0.005f));
        return turn;
    }
}

public class MagePas5Buff : Buff
{
    int lv;
    public MagePas5Buff(int level)
    {
        lv = level;
    }

    public override int updateTurn()
    {
        float tmp = 1.0f;
        tmp += (int)((user.getMP() / user.DC.getMaxMP()) / 0.2f) * (0.04f + lv * 0.01f);
        buffMgcATK = tmp;
        return turn;
    }
}

public class PriestAct4Buff : Buff {
    int lv;
    BattleChar bc;
    float addHP;
    SkillPriest set;

    public PriestAct4Buff(int level, BattleChar bc, SkillPriest set) {
        lv = level;
        this.bc = bc;
        addHP = 1.0f;
        this.set = set;
    }

    public override int updateTurn()
    {
        if (((PriestPassive1)set.pas[0]).isInit)
        {
            addHP *= ((PriestPassive1)set.pas[0]).getBuff();
        }

        if (((PriestPassive2)set.pas[1]).isInit)
        {
            addHP *= ((PriestPassive2)set.pas[1]).getBuff();
        }

        if (((PriestPassive5)set.pas[4]).isInit)
        {
            addHP *= ((PriestPassive5)set.pas[4]).getBuff(bc);
        }

        bc.SetHP(bc.getHP() + bc.DC.getMaxHP() * (0.1f + lv * 0.02f) * addHP);
        return turn;
    }
}

public class PriestPas4Buff : Buff
{
    int lv;
    BattleChar bc;

    public PriestPas4Buff(int level, BattleChar bc)
    {
        lv = level;
        this.bc = bc;
    }

    public override int updateTurn()
    {
        bc.SetHP(bc.getHP() + bc.DC.getMaxHP() * (0.01f + lv * 0.005f));
        bc.setMP(bc.getMP() + bc.DC.getMaxHP() * (0.01f + lv * 0.005f));
        return turn;
    }
}




