using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PriestNormal : NormalAttack
{

    public PriestNormal(SkillSet set) : base(set)
    {
        image = "battle/Archer/Skill/PriestAttackNormal";
    }

    public override void initSkill()
    {
        base.initSkill();
        setTag(set.user.tg);
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);
        foreach (BattleChar bc in target)
        {
            bc.setLastAttack(set.user);
            bc.SetMagicDamage(set.user, 1.0f, level, coef * set.user.getFinalDamage() * CriticalTest());
        }
    }
}

[System.Serializable]
public class SkillPriest : SkillSet
{
    public float dam;
    [System.NonSerialized]
    public BattleChar lastTarget;



    public SkillPriest() : base()
    {
        normal = new PriestNormal(this);

        act.Add( new PriestActive1(1, this));
        act.Add( new PriestActive2(1, this));
        act.Add( new PriestActive3(1, this));
        act.Add( new PriestActive4(1, this));
        act.Add( new PriestActive5(1, this));

        pas.Add( new PriestPassive1(1, this));
        pas.Add( new PriestPassive2(1, this));
        pas.Add( new PriestPassive3(1, this));
        pas.Add( new PriestPassive4(1, this));
        pas.Add( new PriestPassive5(1, this));

        set.Add(act[0]);
        set.Add(act[1]);
        set.Add(act[3]);
        set.Add(pas[3]);

    }

}

[System.Serializable]
public class PriestActives : ActiveSkills
{
    //public new SkillPriest set;

    public PriestActives(SkillPriest set) : base(set)
    {

    }
}

[System.Serializable]
public class PriestPassives : PassiveSkills
{
    //public new SkillPriest set;
    public bool isInit;

    public PriestPassives(SkillPriest set) : base(set)
    {

    }
}

[System.Serializable]
public class PriestActive1 : PriestActives
{
    public PriestActive1(int level, SkillPriest set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.FRIEND;
        type = SKILLTYPE.ACT;
        image = "battle/Priest/Skill/PriestActive1";
        name = "Healing";
        explain = "";

        this.level = level;
        coef = 1;

        basicMana = 5;
        incMana = 3;

        
        key = "act1";

    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);
        set.user.BM.zoomPlayerCam();

        foreach (BattleChar bc in target)
        {
            DamCoef = bc.getHP() + bc.DC.getMaxHP() * (0.2f + level * 0.02f);
            bc.Healed();

            if (((PriestPassive1)set.pas[0]).isInit) {
                DamCoef *= ((PriestPassive1)set.pas[0]).getBuff();
            }

            if (((PriestPassive2)set.pas[1]).isInit) {
                DamCoef *= ((PriestPassive2)set.pas[1]).getBuff();
            }

            if( ((PriestPassive3)set.pas[2]).isInit) ((PriestPassive3)set.pas[2]).getBuff();

            if (((PriestPassive5)set.pas[4]).isInit)
            {
                DamCoef *= ((PriestPassive5)set.pas[4]).getBuff(bc);
            }

            DamCoef += set.user.DC.charEquipSet.getHealEFC();

            bc.SetHP(DamCoef);
            foreach (Buff b in bc.debuffStack){
                b.turn = 0;
            }


            bc.stateSturn.setTurn(0);

            foreach (Bleed b in bc.stateBleed)
            {
                b.setTurn(0);
            }

            foreach (Poison b in bc.statePoison)
            {
                b.setTurn(0);
            }

            foreach (Burn b in bc.stateBurn)
            {
                b.setTurn(0);
            }

            foreach (Slow b in bc.stateSlow)
            {
                b.setTurn(0);
            }
        }
    }
}

[System.Serializable]
public class PriestActive2 : PriestActives
{
    [System.NonSerialized]
    List<BattleChar> friends;
    [System.NonSerialized]
    List<Buff> skills;
    public PriestActive2(int level, SkillPriest set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.FRIEND;
        type = SKILLTYPE.ACT;
        image = "battle/Priest/Skill/PriestActive2";
        name = "The Grace of Protection";
        explain = "";

        this.level = level;
        coef = 1;

        basicMana = 6;
        incMana = 4;

        
        key = "act2";

        friends = new List<BattleChar>();
        skills = new List<Buff>();
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);
        set.user.BM.zoomPlayerCam();

        foreach (BattleChar bc in target)
        {
            DamCoef = 1.0f + 0.05f * level;
            bc.buffed();

            Buff d = new Buff();
            d.buffPhyDEF = 1.15f + level * 0.03f;
            d.buffMgcDEF = 1.15f + level * 0.03f;
            d.turn = 3;
            d.step = 1;

            if (((PriestPassive1)set.pas[0]).isInit)
            {
                d.buffPhyDEF *= ((PriestPassive1)set.pas[0]).getBuff();
                d.buffMgcDEF *= ((PriestPassive1)set.pas[0]).getBuff();
            }

            if (((PriestPassive3)set.pas[2]).isInit) ((PriestPassive3)set.pas[2]).getBuff();


            if (friends.Exists(x => x == bc))
            {
                skills.ToArray()[friends.IndexOf(bc)].turn = 3;
            }
            else
            {
                friends.Add(bc);
                skills.Add(d);
                bc.buffStack.Add(d);
            }
        }
    }
}

[System.Serializable]
public class PriestActive3 : PriestActives
{
    [System.NonSerialized]
    List<BattleChar> friends;
    [System.NonSerialized]
    List<Buff> skills;
    public PriestActive3(int level, SkillPriest set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.FRIEND;
        type = SKILLTYPE.ACT;
        image = "battle/Priest/Skill/PriestActive3";
        name = "The Grace of Courage";
        explain = "";

        this.level = level;
        coef = 1;

        basicMana = 6;
        incMana = 4;

        
        key = "act3";

        friends = new List<BattleChar>();
        skills = new List<Buff>();
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);
        set.user.BM.zoomPlayerCam();

        foreach (BattleChar bc in target)
        {
            DamCoef = 1.0f + 0.05f * level;
            bc.buffed();

            Buff d = new Buff();
            d.buffPhyATK = 1.1f + level * 0.03f;
            d.buffMgcATK = 1.1f + level * 0.03f;
            d.turn = 3;
            d.step = 1;

            if (((PriestPassive1)set.pas[0]).isInit)
            {
                d.buffPhyATK *= ((PriestPassive1)set.pas[0]).getBuff();
                d.buffMgcATK *= ((PriestPassive1)set.pas[0]).getBuff();
            }

            if (((PriestPassive3)set.pas[2]).isInit) ((PriestPassive3)set.pas[2]).getBuff();

            if (friends.Exists(x => x == bc))
            {
                skills.ToArray()[friends.IndexOf(bc)].turn = 3;
            }
            else
            {
                friends.Add(bc);
                skills.Add(d);
                bc.buffStack.Add(d);
            }
        }
    }
}

[System.Serializable]
public class PriestActive4 : PriestActives
{
    [System.NonSerialized]
    List<BattleChar> friends;
    [System.NonSerialized]
    List<Buff> skills;
    public PriestActive4(int level, SkillPriest set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.FRIENDALL;
        type = SKILLTYPE.ACT;
        image = "battle/Priest/Skill/PriestActive4";
        name = "sanctuary";
        explain = "";

        this.level = level;
        coef = 1;

        basicMana = 13;
        incMana = 6;

        
        key = "act4";

        friends = new List<BattleChar>();
        skills = new List<Buff>();
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);
        set.user.BM.zoomPlayerCam();

        foreach (BattleChar bc in target)
        {
            DamCoef = 1.0f + 0.05f * level;
            bc.Healed();

            Buff d = new PriestAct4Buff(level, bc, (SkillPriest)set);
            d.user = set.user;
            d.turn = 2;
            d.step = 1;

            if (((PriestPassive3)set.pas[2]).isInit) ((PriestPassive3)set.pas[2]).getBuff();

            if (friends.Exists(x => x == bc))
            {
                skills.ToArray()[friends.IndexOf(bc)].turn = 2;
            }
            else
            {
                friends.Add(bc);
                skills.Add(d);
                bc.buffStack.Add(d);
            }
        }
    }
}

[System.Serializable]
public class PriestActive5 : PriestActives
{
    public PriestActive5(int level, SkillPriest set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.FRIENDDEAD;
        type = SKILLTYPE.ACT;
        image = "battle/Priest/Skill/PriestActive5";
        name = "Resurrection";
        explain = "";

        this.level = level;
        coef = 1;

        
        key = "act5";

        basicMana = 0;
        incMana = 0;
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);

        set.user.setMP(0);
        if (set.user.stateSturn == null)
        {
            set.user.stateSturn = new Sturn(2, set.user, set.user);
        }
        else {
            set.user.stateSturn.setSturn(2, set.user, set.user);
        }

        foreach (BattleChar bc in target)
        {
            bc.SetHP(bc.DC.getMaxHP());
            bc.setMP(bc.DC.getMaxMP() * (0.2f + level * 0.03f));
            set.user.BM.Revive(bc);
        }
    }
}

[System.Serializable]
public class PriestPassive1 : PriestPassives
{
    public PriestPassive1(int level, SkillPriest set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.FRIEND;
        type = SKILLTYPE.PAS;
        image = "battle/Priest/Skill/PriestPassive1";
        name = "Faithful";
        explain = "";

        this.level = level;
        coef = 1;

        

    }

    public override void initSkill()
    {
        isInit = true;
    }

    public override void resetSkill()
    {
        isInit = false;
    }

    public override void useSkill(List<BattleChar> target)
    {

    }

    public float getBuff() {
        return 1.2f + level * 0.05f;
    }
}

[System.Serializable]
public class PriestPassive2 : PriestPassives
{
    public PriestPassive2(int level, SkillPriest set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.FRIEND;
        type = SKILLTYPE.PAS;
        image = "battle/Priest/Skill/PriestPassive2";
        name = "Calling of angel";
        explain = "";

        this.level = level;
        coef = 1;

        

    }

    public override void initSkill()
    {
        isInit = true;
    }

    public override void resetSkill()
    {
        isInit = false;
    }

    public override void useSkill(List<BattleChar> target)
    {

    }

    public float getBuff()
    {
        return Random.value < (0.1f + level * 0.02f) ? 2.0f : 1.0f ;
    }
}

[System.Serializable]
public class PriestPassive3 : PriestPassives
{
    public PriestPassive3(int level, SkillPriest set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.SELF;
        type = SKILLTYPE.PAS;
        image = "battle/Priest/Skill/PriestPassive3";
        name = "Agent of angel";
        explain = "";

        this.level = level;
        coef = 1;

        

    }

    public override void initSkill()
    {
        isInit = true;
    }

    public override void resetSkill()
    {
        isInit = false;
    }

    public override void useSkill(List<BattleChar> target)
    {

    }

    public float getBuff()
    {
        if (Random.value < 1.0f + level * 0.02f) set.user.setMP(set.user.DC.getMaxMP());

        return 0f;
    }
}

[System.Serializable]
public class PriestPassive4 : PriestPassives
{
    [System.NonSerialized]
    List<BattleChar> friends;
    [System.NonSerialized]
    List<Buff> skills;
    public PriestPassive4(int level, SkillPriest set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.FRIENDALL;
        type = SKILLTYPE.ACT;
        image = "battle/Priest/Skill/PriestPassive4";
        name = "Sacred light";
        explain = "";

        this.level = level;
        coef = 1;


        friends = new List<BattleChar>();
        skills = new List<Buff>();
    }



    public override void initSkill()
    {
        useSkill(set.user.BM.myBC);
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);

        foreach (BattleChar bc in target)
        {
            DamCoef = 1.0f + 0.05f * level;

            Buff d = new PriestPas4Buff(level, bc);
            d.turn = 100;
            d.step = 0;

            if (friends.Exists(x => x == bc))
            {
                skills.ToArray()[friends.IndexOf(bc)].turn = 100;
            }
            else
            {
                friends.Add(bc);
                skills.Add(d);
                bc.buffStack.Add(d);
            }
        }
    }
}

[System.Serializable]
public class PriestPassive5 : PriestPassives
{
    public PriestPassive5(int level, SkillPriest set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.FRIEND;
        type = SKILLTYPE.PAS;
        image = "battle/Priest/Skill/PriestPassive5";
        name = "Concentration";
        explain = "";

        this.level = level;
        coef = 1;

        

    }

    public override void initSkill()
    {
        isInit = true;
    }

    public override void resetSkill()
    {
        isInit = false;
    }

    public override void useSkill(List<BattleChar> target)
    {

    }

    public float getBuff(BattleChar bc)
    {
        float buff = (bc.getHP() / bc.DC.getMaxHP()) / 0.04f * (0.02f + level * 0.005f);

        return buff > 0.2f ? 1.2f : 1.0f + buff;
    }
}
