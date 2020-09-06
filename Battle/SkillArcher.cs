using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArcherNormal : NormalAttack
{

    public ArcherNormal(SkillSet set) : base(set)
    {
        image = "battle/Archer/Skill/ArcherAttackNormal";
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
            bc.SetPhysicalDamage(set.user, 1.0f, level, coef * set.user.getFinalDamage() * CriticalTest());
        }
    }
}

[System.Serializable]
public class SkillArcher : SkillSet
{
    [System.NonSerialized]
    public BattleChar lastTarget;
    int lastTargetHit = 0;

    public SkillArcher() : base() { 
        normal = new ArcherNormal(this);

        act.Add(new ArcherActive1(1, this));
        act.Add(new ArcherActive2(1, this));
        act.Add(new ArcherActive3(1, this));
        act.Add(new ArcherActive4(1, this));
        act.Add(new ArcherActive5(1, this));

        pas.Add(new ArcherPassive1(1, this));
        pas.Add(new ArcherPassive2(1, this));
        pas.Add(new ArcherPassive3(1, this));
        pas.Add(new ArcherPassive4(1, this));
        pas.Add(new ArcherPassive5(1, this));

        set.Add(act[0]);
        set.Add(act[2]);
        set.Add(pas[0]);
        set.Add(pas[3]);
    }

    public override float BuffedDamage(float damage, BattleChar target)
    {
        return damage * pas4Dam(target) * pas5Dam(target);
   
    }

    public float pas4Dam(BattleChar target) {
        if (!((ArcherPassive4)pas[3]).isInit) return 1.0f;

        return (target.DC.curHP / target.DC.getMaxHP() >= 0.3f ? 1.3f : 1.0f + (int)((target.DC.curHP / target.DC.getMaxHP()) / 0.05f) * (0.02f + pas[3].level * 0.002f));
    }

    public float pas5Dam(BattleChar target) {
        if (!((ArcherPassive5)pas[4]).isInit) return 1.0f;

        if (target.Equals(lastTarget))
        {
            lastTargetHit++;
            return 1.0f + (0.1f + pas[4].level * 0.01f) * (lastTargetHit > 3 ? 3 : lastTargetHit);
        }
        else {
            lastTargetHit = 0;
            return 1.0f;
        }
    }
}

[System.Serializable]
public class ArcherActives : ActiveSkills
{
    //public new SkillArcher set;

    public ArcherActives(SkillArcher set) : base(set)
    {

    }
}

[System.Serializable]
public class ArcherPassives : PassiveSkills
{
    //public new SkillArcher set;
    public bool isInit;

    public ArcherPassives(SkillArcher set) : base(set)
    {

    }
}

[System.Serializable]
public class ArcherActive1 : ArcherActives
{
    public ArcherActive1(int level, SkillArcher set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.ENEMY;
        type = SKILLTYPE.ACT;
        image = "battle/Archer/Skill/ArcherActive1";
        name = "Double shot";
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

        foreach (BattleChar bc in target)
        {
            DamCoef = 0.75f + 0.05f * level;

            bc.setLastAttack(set.user);
            bc.getTarget().SetPhysicalDamage(set.user, DamCoef, level, coef * set.BuffedDamage(set.user.getFinalDamage() * CriticalTest(), bc));
            bc.getTarget().SetPhysicalDamage(set.user, DamCoef, level, coef * set.BuffedDamage(set.user.getFinalDamage() * CriticalTest(), bc));
            ((SkillArcher)set).lastTarget = bc;
        }
    }
}

[System.Serializable]
public class ArcherActive2 : ArcherActives
{
    public ArcherActive2(int level, SkillArcher set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.ENEMY;
        type = SKILLTYPE.ACT;
        image = "battle/Archer/Skill/ArcherActive2";
        name = "Power shot";
        explain = "";

        this.level = level;
        coef = 1;

        basicMana = 8;
        incMana = 3;

        key = "act2";
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);

        foreach (BattleChar bc in target)
        {
            DamCoef = 1.8f + 0.1f * level;

            bc.setLastAttack(set.user);
            bc.getTarget().SetPhysicalDamage(set.user, DamCoef, level, coef * set.BuffedDamage(set.user.getFinalDamage() * CriticalTest(), bc));
            ((SkillArcher)set).lastTarget = bc;
        }
    }
}

[System.Serializable]
public class ArcherActive3 : ArcherActives
{
    public ArcherActive3(int level, SkillArcher set) : base(set)
    {
        targetNum = 2;
        targetTag = TARGETTAG.ENEMY;
        type = SKILLTYPE.ACT;
        image = "battle/Archer/Skill/ArcherActive3";
        name = "Multi shot";
        explain = "";

        this.level = level;
        coef = 1;

        basicMana = 6;
        incMana = 3;

        key = "act3";
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);

        foreach (BattleChar bc in target)
        {
            DamCoef = 0.8f + 0.05f * level;

            bc.setLastAttack(set.user);
            bc.getTarget().SetPhysicalDamage(set.user, DamCoef, level, coef * set.BuffedDamage(set.user.getFinalDamage() * CriticalTest(), bc));
            ((SkillArcher)set).lastTarget = bc;
        }
    }
}

[System.Serializable]
public class ArcherActive4 : ArcherActives
{
    [System.NonSerialized]
    List<BattleChar> enemies;
    [System.NonSerialized]
    List<Bleed> skills;
    public ArcherActive4(int level, SkillArcher set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.ENEMY;
        type = SKILLTYPE.ACT;
        image = "battle/Archer/Skill/ArcherActive4";
        name = "Bleeding shot";
        explain = "";

        this.level = level;
        coef = 1;

        basicMana = 6;
        incMana = 4;

        key = "act4";

        enemies = new List<BattleChar>();
        skills = new List<Bleed>();
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);

        foreach (BattleChar bc in target)
        {
            DamCoef = 0.7f + 0.05f * level;

            bc.setLastAttack(set.user);
            bc.getTarget().SetMagicDamage(set.user, DamCoef, level, coef * set.BuffedDamage(set.user.getFinalDamage() * CriticalTest(), bc));
            ((SkillArcher)set).lastTarget = bc;

            if (enemies.Exists(x => x == bc)) {
                skills.ToArray()[enemies.IndexOf(bc)].setTurn(3);
            }
            else {
                Bleed b = new Bleed(3, set.user, bc, set.user.GetMgcATK() * (0.3f + 0.02f * level));
                enemies.Add(bc);
                skills.Add(b);
                bc.stateBleed.Add(b);
            }
        }
    }
}

[System.Serializable]
public class ArcherActive5 : ArcherActives
{
    public ArcherActive5(int level, SkillArcher set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.ENEMY;
        type = SKILLTYPE.ACT;
        image = "battle/Archer/Skill/ArcherActive5";
        name = "Magic shot";
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

        foreach (BattleChar bc in target)
        {
            DamCoef = 1.25f + 0.1f * level;

            bc.setLastAttack(set.user);
            bc.getTarget().SetMagicDamage(set.user, DamCoef, level, coef * set.BuffedDamage(set.user.getFinalDamage() * CriticalTest(), bc));
            ((SkillArcher)set).lastTarget = bc;
        }
    }
}

[System.Serializable]
public class ArcherPassive1 : ArcherPassives
{
    [System.NonSerialized]
    Buff selfBuff;

    public ArcherPassive1(int level, SkillArcher set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.SELF;
        type = SKILLTYPE.PAS;
        image = "battle/Archer/Skill/ArcherPassive1";
        name = "Sharp-eye (Ver.Archer)";
        explain = "";

        this.level = level;
        coef = 1;
    }

    public override void initSkill()
    {
        //set.user.buffStack.Add(this);
    }

    public override void useSkill(List<BattleChar> target)
    {
        //자버프
        if (selfBuff == null)
        {
            selfBuff = new Buff();
            selfBuff.turn = 100;
            selfBuff.step = 0;
            selfBuff.buffCrit += 0.1f + level * 0.01f;

            set.user.buffStack.Add(selfBuff);
        }
        else
        {
            selfBuff.turn = 100;
        }
    }
}

[System.Serializable]
public class ArcherPassive2 : ArcherPassives
{
    [System.NonSerialized]
    List<Buff> debuff; //만들어진 버프 객체를 관리
    [System.NonSerialized]
    List<BattleChar> friends;

    public ArcherPassive2(int level, SkillArcher set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.FRIENDALL;
        type = SKILLTYPE.PAS;
        image = "battle/Archer/Skill/ArcherPassive2";
        name = "Capturing Weakness";
        explain = "";

        this.level = level;
        coef = 1;

        
        debuff = new List<Buff>();
        friends = new List<BattleChar>();
    }

    public override void initSkill()
    {
        useSkill(set.user.BM.myBC);
    }

    public override void useSkill(List<BattleChar> target)
    {
        foreach (BattleChar bc in target)
        {
            if (friends.Exists(x => x == bc))
            {
                debuff.ToArray()[friends.IndexOf(bc)].turn = 3;
            }
            else
            {
                Buff d = new Buff();
                d.turn = 3;
                d.step = 0;
                d.buffCrit += 0.05f + 0.005f * level;
                debuff.Add(d);
                friends.Add(bc);
                bc.buffStack.Add(d);
            }
        }
    }
}

[System.Serializable]
public class ArcherPassive3 : ArcherPassives
{
    [System.NonSerialized]
    Buff selfBuff;

    public ArcherPassive3(int level, SkillArcher set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.SELF;
        type = SKILLTYPE.PAS;
        image = "battle/Archer/Skill/ArcherPassive3";
        name = "Head Start";
        explain = "";

        this.level = level;
        coef = 1;

        
    }

    public override void initSkill()
    {
        useSkill(set.user.BM.myBC);
    }

    public override void useSkill(List<BattleChar> target)
    {
        //자버프
        if (selfBuff == null)
        {
            selfBuff = new Buff();
            selfBuff.turn = 1;
            selfBuff.step = 1;
            selfBuff.buffFinalDam += 0.3f + level * 0.03f;

            set.user.buffStack.Add(selfBuff);
        }
        else
        {
            selfBuff.turn = 0;
        }
    }
}

[System.Serializable]
public class ArcherPassive4 : ArcherPassives
{
    

    public ArcherPassive4(int level, SkillArcher set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.FRIENDALL;
        type = SKILLTYPE.PAS;
        image = "battle/Archer/Skill/ArcherPassive4";
        name = "Finish Attack";
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

   
}

[System.Serializable]
public class ArcherPassive5 : ArcherPassives
{


    public ArcherPassive5(int level, SkillArcher set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.FRIENDALL;
        type = SKILLTYPE.PAS;
        image = "battle/Archer/Skill/ArcherPassive5";
        name = "concentrated fire";
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
}