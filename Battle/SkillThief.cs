using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ThiefNormal : NormalAttack
{

    public ThiefNormal(SkillSet set) : base(set)
    {
        image = "battle/Thief/skill/ThiefAttackNormal";
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
            set.user.BM.player2target(bc.transform.position);
            bc.setLastAttack(set.user);
            bc.SetPhysicalDamage(set.user, 1.0f, level, coef * set.user.getFinalDamage() * CriticalTest());
        }
    }
}

[System.Serializable]
public class SkillThief : SkillSet
{
    public float dam;
    public int attack;

    [System.NonSerialized]
    public BattleChar lastTarget;



    public SkillThief() : base()
    {
        attack = 0;
        normal = new ThiefNormal(this);

        act.Add( new ThiefActive1(1, this));
        act.Add( new ThiefActive2(1, this));
        act.Add( new ThiefActive3(1, this));
        act.Add( new ThiefActive4(1, this));
        act.Add( new ThiefActive5(1, this));

        pas.Add( new ThiefPassive1(1, this));
        pas.Add( new ThiefPassive2(1, this));
        pas.Add( new ThiefPassive3(1, this));
        pas.Add( new ThiefPassive4(1, this));
        pas.Add( new ThiefPassive5(1, this));

        set.Add(act[0]);
        set.Add(act[1]);
        set.Add(pas[0]);
        set.Add(pas[1]);
    }

}

[System.Serializable]
public class ThiefActives : ActiveSkills
{
    //public new SkillThief set;

    public ThiefActives(SkillThief set) : base(set)
    {

    }
}

[System.Serializable]
public class ThiefPassives : PassiveSkills
{
    //public new SkillThief set;
    public bool isInit;

    public ThiefPassives(SkillThief set) : base(set)
    {

    }
}

[System.Serializable]
public class ThiefActive1 : ThiefActives
{
    public ThiefActive1(int level, SkillThief set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.ENEMY;
        type = SKILLTYPE.ACT;
        image = "battle/Thief/Skill/ThiefActive1";
        name = "Assassination";
        explain = "";

        this.level = level;
        coef = 1;

        basicMana = 9;
        incMana = 3;

        

        key = "act1";
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);

        foreach (BattleChar bc in target)
        {
            set.user.BM.player2target(bc.transform.position);
            bc.setLastAttack(set.user);
            bc.getTarget().SetPhysicalDamage(set.user, (2.0f + 0.15f * level), level, coef * set.user.getFinalDamage() * CriticalTest());
        }
    }
}

[System.Serializable]
public class ThiefActive2 : ThiefActives
{
    public ThiefActive2(int level, SkillThief set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.ENEMY;
        type = SKILLTYPE.ACT;
        image = "battle/Thief/Skill/ThiefActive2";
        name = "backstab";
        explain = "";

        this.level = level;
        coef = 1;

        basicMana = 5;
        incMana = 2;

        
        key = "act2";
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);

        foreach (BattleChar bc in target)
        {
            set.user.BM.player2target(bc.transform.position);
            bc.setLastAttack(set.user);
            bc.getTarget().SetPhysicalDamage(set.user, (1.5f + 0.1f * level), level, coef * set.user.getFinalDamage() * CriticalTest());
        }
    }
}

[System.Serializable]
public class ThiefActive3 : ThiefActives
{
    [System.NonSerialized]
    List<ThiefAct3Buff> debuff; //만들어진 버프 객체를 관리
    [System.NonSerialized]
    List<BattleChar> enemies;

    public ThiefActive3(int level, SkillThief set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.ENEMY;
        type = SKILLTYPE.ACT;
        image = "battle/Thief/Skill/ThiefActive3";
        name = "Throwing poison Dart";
        explain = "";

        this.level = level;
        coef = 1;

        basicMana = 6;
        incMana = 2;

        
        debuff = new List<ThiefAct3Buff>();
        enemies = new List<BattleChar>();
        key = "act3";
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);

        foreach (BattleChar bc in target)
        {
            set.user.BM.player2target(bc.transform.position);
            bc.setLastAttack(set.user);
            bc.getTarget().SetPhysicalDamage(set.user, (0.75f + 0.05f * level), level, coef * set.user.getFinalDamage() * CriticalTest());

            if (enemies.Exists(x => x == bc))
            {
                debuff.ToArray()[enemies.IndexOf(bc)].turn = 3;
            }
            else
            {
                ThiefAct3Buff d = new ThiefAct3Buff();

                d.target = bc;
                d.turn = 3;
                d.buffIncAP -= 0.2f + 0.02f * level;
                debuff.Add(d);
                enemies.Add(bc);
                bc.buffStack.Add(d);
            }
        }
    }
}

public class ThiefAct3Buff : Buff {
    public BattleChar target;
    public override int updateTurn()
    {
        target.poisoned();
        return base.updateTurn();
    }
}

[System.Serializable]
public class ThiefActive4 : ThiefActives
{
    [System.NonSerialized]
    List<Buff> debuff; //만들어진 버프 객체를 관리
    [System.NonSerialized]
    List<BattleChar> enemies;

    public ThiefActive4(int level, SkillThief set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.ENEMYALL;
        type = SKILLTYPE.ACT;
        image = "battle/Thief/Skill/ThiefActive4";
        name = "bloodthirsthy";
        explain = "";

        this.level = level;
        coef = 1;

        basicMana = 5;
        incMana = 3;

        
        debuff = new List<Buff>();
        enemies = new List<BattleChar>();
        key = "act4";
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);
        set.user.BM.zoomPlayerCam();

        foreach (BattleChar bc in target)
        {
            bc.setLastAttack(set.user);

            if (enemies.Exists(x => x == bc))
            {
                debuff.ToArray()[enemies.IndexOf(bc)].turn = 3;
            }
            else
            {
                Buff d = new Buff();
                d.turn = 2;
                d.buffMgcDEF -= 0.1f + 0.02f * level;
                d.buffPhyDEF -= 0.1f + 0.02f * level;
                debuff.Add(d);
                enemies.Add(bc);
                bc.buffStack.Add(d);
            }
        }
    }
}

[System.Serializable]
public class ThiefActive5 : ThiefActives
{
    [System.NonSerialized]
    List<ThiefActive5Damaged> counter; //만들어진 버프 객체를 관리
    [System.NonSerialized]
    List<BattleChar> friends;

    public ThiefActive5(int level, SkillThief set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.ENEMY;
        type = SKILLTYPE.ACT;
        image = "battle/Thief/Skill/ThiefActive5";
        name = "Vengeance";
        explain = "";

        this.level = level;
        coef = 1;

        basicMana = 5;
        incMana = 3;

        
        counter = new List<ThiefActive5Damaged>();
        friends = new List<BattleChar>();
        key = "act5";
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);
        set.user.BM.zoomPlayerCam();

        foreach (BattleChar bc in target)
        {
            bc.setLastAttack(set.user);

            if (friends.Exists(x => x == bc))
            {
                counter.ToArray()[friends.IndexOf(bc)].setTurn(1);
            }
            else
            {
                ThiefActive5Damaged d = new ThiefActive5Damaged(level, set, 1);
                counter.Add(d);
                friends.Add(bc);
                bc.damagedStack.Add(d);
            }
        }
    }
}

[System.Serializable]
public class ThiefActive5Damaged : CounterSkills {
    public ThiefActive5Damaged(int level, SkillSet set, int turn) : base(set, turn)
    {
        targetNum = 1;
        targetTag = TARGETTAG.ENEMY;
        type = SKILLTYPE.ACT;

        this.level = level;
        coef = 1;

        basicMana = 0;
        incMana = 0;
        turn = 1;

        key = "act5";
    }


    public override void useSkill(BattleChar target)
    {
        base.useSkill(target);

        if(turn > 0)
        {
            set.user.BM.player2target(target.transform.position);
            target.setLastAttack(set.user);
            target.SetMagicDamage(set.user, 1.0f, level, coef * set.user.getFinalDamage() * CriticalTest());
        }
    }
}

[System.Serializable]
public class ThiefPassive1 : ThiefPassives
{
    [System.NonSerialized]
    Buff selfBuff;

    public ThiefPassive1(int level, SkillThief set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.SELF;
        type = SKILLTYPE.PAS;
        image = "battle/Thief/Skill/ThiefPassive1";
        name = "preceding movement";
        explain = "";

        this.level = level;
        coef = 1;

        
    }

    public override void initSkill()
    {
        useSkill(set.user.BM.enemyBC);
    }
    public override void useSkill(List<BattleChar> target)
    {

        //자버프
        if (selfBuff == null)
        {
            selfBuff = new Buff();
            selfBuff.turn = 1;
            selfBuff.buffIncAP = 1.3f + 0.03f * level;
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
public class ThiefPassive2 : ThiefPassives
{
    [System.NonSerialized]
    Buff selfBuff;

    public ThiefPassive2(int level, SkillThief set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.SELF;
        type = SKILLTYPE.PAS;
        image = "battle/Thief/Skill/ThiefPassive2";
        name = "sharp-eye (Ver.Rogue)";
        explain = "";

        this.level = level;
        coef = 1;

        
    }
    public override void initSkill()
    {
        useSkill(set.user.BM.enemyBC);
    }

    public override void useSkill(List<BattleChar> target)
    {

        //자버프
        if (selfBuff == null)
        {
            selfBuff = new Buff();
            selfBuff.turn = 1;
            selfBuff.buffCrit = 1.1f + 0.01f * level;
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
public class ThiefPassive3 : ThiefPassives
{
    [System.NonSerialized]
    Buff selfBuff;

    public ThiefPassive3(int level, SkillThief set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.SELF;
        type = SKILLTYPE.PAS;
        image = "battle/Thief/Skill/ThiefPassive3";
        name = "Confusion";
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
        //자버프
        if (selfBuff == null)
        {
            selfBuff = new Buff();
            selfBuff.turn = 1;
            selfBuff.step = 1;
            selfBuff.buffIncAP += 0.3f + level * 0.02f;

            set.user.buffStack.Add(selfBuff);
        }
        else
        {
            selfBuff.turn = 1;
        }
    }
}

[System.Serializable]
public class ThiefPassive4 : ThiefPassives
{
    [System.NonSerialized]
    List<Buff> debuff; //만들어진 버프 객체를 관리
    [System.NonSerialized]
    List<BattleChar> enemies;

    public ThiefPassive4(int level, SkillThief set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.SELF;
        type = SKILLTYPE.PAS;
        image = "battle/Thief/Skill/ThiefPassive4";
        name = "Explosive Trap";
        explain = "";

        this.level = level;
        coef = 1;

        
        debuff = new List<Buff>();
        enemies = new List<BattleChar>();
    }

    public override void initSkill()
    {
        useSkill(set.user.BM.enemyBC);
    }

    public override void useSkill(List<BattleChar> target)
    {
        foreach (BattleChar bc in target)
        {
            bc.SetMagicDamage(set.user, (0.6f + 0.03f * level), level, coef * set.user.getFinalDamage() * CriticalTest());
        }
    }
}

[System.Serializable]
public class ThiefPassive5 : ThiefPassives
{
    int atkNum;

    public ThiefPassive5(int level, SkillThief set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.SELF;
        type = SKILLTYPE.PAS;
        image = "battle/Thief/Skill/ThiefPassive5";
        name = "final blow";
        explain = "";

        this.level = level;
        coef = 1;

        atkNum = 0;

        
    }

    public override void resetSkill()
    {
        isInit = false;
    }

    public override void initSkill()
    {
        isInit = true;
    }

    public void pas5(BattleChar bc) {
        atkNum++;
        if (atkNum > 2) {
            bc.SetPhysicalDamage(set.user, (1.0f + 0.1f * level), level, coef * set.user.getFinalDamage() * CriticalTest());
            atkNum = 0;
        }
    }
}