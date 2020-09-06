using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

[System.Serializable]
public class MageNormal : NormalAttack
{

    public MageNormal(SkillSet set) : base(set)
    {
        image = "battle/Mage/skill/MageAttackNormal";
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
public class SkillMage : SkillSet
{
    public float dam;
    [System.NonSerialized]
    public BattleChar lastTarget;



    public SkillMage() : base() { 
        normal = new MageNormal(this);

        act.Add( new MageActive1(1, this));
        act.Add(new MageActive2(1, this));
        act.Add(new MageActive3(1, this));
        act.Add(new MageActive4(1, this));
        act.Add(new MageActive5(1, this));

        pas.Add(new MagePassive1(1, this));
        pas.Add(new MagePassive2(1, this));
        pas.Add(new MagePassive3(1, this));
        pas.Add(new MagePassive4(1, this));
        pas.Add(new MagePassive5(1, this));

        set.Add(act[0]);
        set.Add(act[2]);
        set.Add(act[3]);
        set.Add(pas[1]);
    }

}


[System.Serializable]
public class MageActives : ActiveSkills {
    //public new SkillMage set;

    public MageActives(SkillMage set) : base(set) { 
        
    }
}


[System.Serializable]
public class MagePassives : PassiveSkills
{
    //public new SkillMage set;
    public bool isInit;

    public MagePassives(SkillMage set) : base(set)
    {

    }
}


[System.Serializable]
public class MageActive1 : MageActives
{
    [System.NonSerialized]
    List<BattleChar> enemies;
    [System.NonSerialized]
    List<Burn> skills;
    public MageActive1(int level, SkillMage set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.ENEMY;
        type = SKILLTYPE.ACT;
        image = "battle/Mage/Skill/MageActive1";
        name = "Fire Ball";
        explain = "";

        this.level = level;
        coef = 1;

        basicMana = 4;
        incMana = 2;

        enemies = new List<BattleChar>();
        skills = new List<Burn>();

        key = "act1";
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);

        foreach (BattleChar bc in target)
        {
            DamCoef = 1.0f + 0.05f * level;

            bc.setLastAttack(set.user);
            ((SkillMage)set).dam = bc.getTarget().SetMagicDamage(set.user, DamCoef, level, coef * set.user.getFinalDamage() * CriticalTest());
            if (((MagePassive4)set.pas[3]).isInit) ((MagePassive4)set.pas[3]).addAttack(bc, ((SkillMage)set).dam);
            ((SkillMage)set).lastTarget = bc;

            if (enemies.Exists(x => x == bc))
            {
                skills.ToArray()[enemies.IndexOf(bc)].setTurn(3);
            }
            else
            {
                Burn b = new Burn(3, set.user, bc, set.user.GetMgcATK() * (0.2f + 0.03f * level));
                enemies.Add(bc);
                skills.Add(b);
                bc.stateBurn.Add(b);
            }
        }
    }
}


[System.Serializable]
public class MageActive2 : MageActives
{

    public MageActive2(int level, SkillMage set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.ENEMY;
        type = SKILLTYPE.ACT;
        image = "battle/Mage/Skill/MageActive2";
        name = "Magic Arrow";
        explain = "";

        this.level = level;
        coef = 1;

        basicMana = 6;
        incMana = 3;

        key = "act2";
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);

        foreach (BattleChar bc in target)
        {
            DamCoef = 1.8f + 0.15f * level;

            bc.setLastAttack(set.user);
            ((SkillMage)set).dam = bc.getTarget().SetMagicDamage(set.user, DamCoef, level, coef * set.user.getFinalDamage() * CriticalTest());

            if (set.pas[3] != null) ((MagePassive4)set.pas[3]).addAttack(bc, ((SkillMage)set).dam);
            ((SkillMage)set).lastTarget = bc;
        }
    }
}

[System.Serializable]
public class MageActive3 : MageActives
{

    public MageActive3(int level, SkillMage set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.ENEMYALL;
        type = SKILLTYPE.ACT;
        image = "battle/Mage/Skill/MageActive3";
        name = "Mana Bombing";
        explain = "";

        this.level = level;
        coef = 1;

        basicMana = 10;
        incMana = 6;

        key = "act3";
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);

        foreach (BattleChar bc in target)
        {
            DamCoef = 1.0f + 0.1f * level;

            bc.setLastAttack(set.user);
            ((SkillMage)set).dam = bc.getTarget().SetMagicDamage(set.user, DamCoef, level, coef * set.user.getFinalDamage() * CriticalTest());

            if (set.pas[3] != null) ((MagePassive4)set.pas[3]).addAttack(bc, ((SkillMage)set).dam);
            ((SkillMage)set).lastTarget = bc;
        }
    }
}

[System.Serializable]
public class MageActive4 : MageActives
{

    public MageActive4(int level, SkillMage set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.FRIENDALL;
        type = SKILLTYPE.ACT;
        image = "battle/Mage/Skill/MageActive4";
        name = "Mana Charging";
        explain = "";

        this.level = level;
        coef = 1;

        basicMana = 0;
        incMana = 0;

        key = "act4";
    }

    public override void useSkill(List<BattleChar> target)
    {
        basicMana = (int)(set.user.DC.getMaxMP() * (0.2f - 0.01f * level));
        base.useSkill(target);
        set.user.BM.zoomPlayerCam();

        foreach (BattleChar bc in set.user.BM.myBC)
        {
            DamCoef = 1.0f + 0.1f * level;

            bc.setMP(bc.DC.getMaxMP() * 0.2f + bc.getMP());
        }
    }
}

[System.Serializable]
public class MageActive5 : MageActives
{

    public MageActive5(int level, SkillMage set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.ENEMY;
        type = SKILLTYPE.ACT;
        image = "battle/Mage/Skill/MageActive5";
        name = "Dispel";
        explain = "";

        this.level = level;
        coef = 1;

        basicMana = 6;
        incMana = 3;

        key = "act5";
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);
        set.user.BM.zoomPlayerCam();

        foreach (BattleChar bc in target)
        {
            foreach (Buff b in bc.buffStack) {
                if (Random.value > (0.5f + 0.03f * level)) break;

                b.turn = -1;

            }
        }
    }
}

[System.Serializable]
public class MagePassive1 : MagePassives
{

    public MagePassive1(int level, SkillMage set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.SELF;
        type = SKILLTYPE.PAS;
        image = "battle/Mage/Skill/MagePassive1";
        name = "Mana Drain";
        explain = "";

        this.level = level;
        coef = 1;

        
    }

    public override void initSkill()
    {
        set.user.killStack.Add(this);
    }

    public override void useSkill(List<BattleChar> target)
    {
        set.user.setMP(set.user.getMP() + (0.15f + level * 0.01f) * set.user.DC.getMaxMP());
    }
}

[System.Serializable]
public class MagePassive2 : MagePassives
{
    [System.NonSerialized]
    Buff selfBuff;

    public MagePassive2(int level, SkillMage set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.SELF;
        type = SKILLTYPE.PAS;
        image = "battle/Mage/Skill/MagePassive2";
        name = "Enlightenment";
        explain = "";

        this.level = level;
        coef = 1;

        
    }

    public override void initSkill()
    {
        this.useSkill(set.user.BM.myBC);
    }

    public override void useSkill(List<BattleChar> target)
    {
        //자버프
        if (selfBuff == null)
        {
            selfBuff = new MagePas2Buff(level);
            selfBuff.user = set.user;
            selfBuff.turn = 1;
            selfBuff.step = -1;

            set.user.buffStack.Add(selfBuff);
        }
        else
        {
            selfBuff.turn = 1;
        }
    }
}

[System.Serializable]
public class MagePassive3 : MagePassives
{
    [System.NonSerialized]
    Buff selfBuff;

    public MagePassive3(int level, SkillMage set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.SELF;
        type = SKILLTYPE.PAS;
        image = "battle/Mage/Skill/MagePassive3";
        name = "Glass Cannon";
        explain = "";

        this.level = level;
        coef = 1;

        
    }

    public override void initSkill()
    {
        this.useSkill(set.user.BM.myBC);
    }

    public override void useSkill(List<BattleChar> target)
    {
        //자버프
        if (selfBuff == null)
        {
            selfBuff = new Buff();
            selfBuff.turn = 1;
            selfBuff.step = -1;

            selfBuff.buffMgcATK = 1.1f + level * 0.02f;
            selfBuff.buffMgcDEF = 0.95f - level * 0.01f;
            selfBuff.buffPhyDEF = 0.95f - level * 0.01f;

            set.user.buffStack.Add(selfBuff);
        }
        else
        {
            selfBuff.turn = 1;
        }
    }
}

[System.Serializable]
public class MagePassive4 : MagePassives
{

    public MagePassive4(int level, SkillMage set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.SELF;
        type = SKILLTYPE.PAS;
        image = "battle/Mage/Skill/MagePassive4";
        name = "Mana Cyclone";
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

    public void addAttack(BattleChar bc, float damage) {
        if (Random.value < (0.1f + level * 0.01f))
        {
            bc.setLastAttack(set.user);
            bc.SetMagicDamage(set.user, damage/set.user.GetMgcATK() * 0.3f, level, coef * set.user.getFinalDamage() * CriticalTest());
        }
    }
}

[System.Serializable]
public class MagePassive5 : MagePassives
{
    [System.NonSerialized]
    Buff selfBuff;

    public MagePassive5(int level, SkillMage set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.SELF;
        type = SKILLTYPE.PAS;
        image = "battle/Mage/Skill/MagePassive5";
        name = "Efficient Casting";
        explain = "";

        this.level = level;
        coef = 1;

        
    }

    public override void initSkill()
    {
        this.useSkill(set.user.BM.myBC);
    }

    public override void useSkill(List<BattleChar> target)
    {
        //자버프
        if (selfBuff == null)
        {
            selfBuff = new MagePas5Buff(level);
            selfBuff.user = set.user;
            selfBuff.turn = 1;
            selfBuff.step = -1;

            set.user.buffStack.Add(selfBuff);
        }
        else
        {
            selfBuff.turn = 1;
        }
    }
}
