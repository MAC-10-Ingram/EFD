using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KnightNormal : NormalAttack {

    public KnightNormal( SkillSet set) : base(set) {
        image = "battle/Knight/Skill/KnightAttackNormal";
    }

    public override void setTag(TARGETTAG tg)
    {
        if (tg == TARGETTAG.ENEMY) base.setTag(TARGETTAG.FRIEND);
        else base.setTag(TARGETTAG.ENEMY);
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

            ((KnightPassive3)set.pas[2]).pas3(bc);
        }
    }
}

[System.Serializable]
public class SkillKnight : SkillSet
{

    public SkillKnight() : base() {
        normal = new KnightNormal(this);

        act.Add(new KnightActive1(1, this));
        act.Add(new KnightActive2(1, this));
        act.Add(new KnightActive3(1, this));
        act.Add(new KnightActive4(1, this));
        act.Add(new KnightActive5(1, this));


        pas.Add(new KnightPassive1(1, this));
        pas.Add(new KnightPassive2(1, this));
        pas.Add(new KnightPassive3(1, this));
        pas.Add(new KnightPassive4(1, this));
        pas.Add(new KnightPassive5(1, this));


        set.Add(act[0]);
        set.Add(act[1]);
        set.Add(pas[0]);
        set.Add(pas[1]);

    }

    public override void setSkillSet()
    {
        base.setSkillSet();
    }
}

[System.Serializable]
public class KnightActives : ActiveSkills
{
    //public new SkillKnight set;

    public KnightActives(SkillKnight set) : base(set)
    {

    }
}

[System.Serializable]
public class KnightPassives : PassiveSkills
{
    //public new SkillKnight set;
    public bool isInit;

    public KnightPassives(SkillKnight set) : base(set)
    {

    }
}

[System.Serializable]
public class KnightActive1 : KnightActives
{

    public KnightActive1(int level, SkillKnight set) : base(set) { 
        targetNum = 1;
        targetTag = TARGETTAG.ENEMY;
        type = SKILLTYPE.ACT;
        image = "battle/Knight/Skill/KnightActive1";
        name = "Power Bash";
        explain = "";

        this.level = level;
        coef = 1;

        basicMana = 5;
        incMana = 2;

        key = "act1";
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);

        foreach (BattleChar bc in target)
        {
            set.user.BM.player2target(bc.transform.position);
            bc.setLastAttack(set.user);
            bc.SetPhysicalDamage(set.user, 1.0f, level, coef * set.user.getFinalDamage() * CriticalTest());

            if (Random.value <= (0.3 + 0.01 * level))
            {
                if (bc.stateSturn == null) bc.stateSturn = new Sturn(1, set.user, bc);
                else bc.stateSturn.setSturn(1, set.user, bc);
            }

            ((KnightPassive3)set.pas[2]).pas3(bc);
        }
    }
}

[System.Serializable]
public class KnightActive2 : KnightActives
{
    [System.NonSerialized]
    //list와 buffed는 같은 순서대로 index를 받음
    Buff selfBuff; //만들어진 버프 객체를 관리

    [System.NonSerialized]
    List<Taunt> list;
    [System.NonSerialized]
    List<BattleChar> guarded; //버프를 받아간 캐릭터를 관리



    public KnightActive2(int level, SkillKnight set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.SELF;
        type = SKILLTYPE.ACT;
        image = "battle/Knight/Skill/KnightActive2";
        name = "Defense Mode";
        explain = "";

        this.level = level;
        coef = 1;

        basicMana = 3;
        incMana = 1;

        list = new List<Taunt>();
        guarded = new List<BattleChar>();

        key = "act2";
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);
        set.user.BM.zoomPlayerCam();

        //자버프
        if (selfBuff == null)
        {
            selfBuff = new Buff();
            selfBuff.turn = 3;
            selfBuff.user = set.user;

            selfBuff.buffMgcDEF += .1f + 0.05f * level;
            selfBuff.buffPhyDEF += .1f + 0.05f * level;
            selfBuff.buffPhyATK -= .1f + 0.01f * level;
            set.user.buffStack.Add(selfBuff);
        }
        else {
            selfBuff.turn = 3;
        }

        //도발
        set.user.tauntTurn = 3;
        foreach(BattleChar g in set.user.BM.getGuarded(set.user)) {
            if (guarded.Exists(x => x == g))
            {
                list.ToArray()[guarded.IndexOf(g)].setTurn(3);
            }
            else
            {
                Taunt t = new Taunt(3, set.user, g);
                list.Add(t);
                guarded.Add(g);
                g.stateTaunt.Add(t);
            }

        }
        


    }
}

[System.Serializable]
public class KnightActive3 : KnightActives
{
    //list와 buffed는 같은 순서대로 index를 받음
    [System.NonSerialized]
    List<Buff> debuff; //만들어진 버프 객체를 관리
    [System.NonSerialized]
    List<BattleChar> enemies;

    [System.NonSerialized]
    List<Taunt> list;
    [System.NonSerialized]
    List<BattleChar> guarded; //보호받는 캐릭터를 관리

    public KnightActive3(int level, SkillKnight set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.ENEMYALL;
        type = SKILLTYPE.ACT;
        image = "battle/Knight/Skill/KnightActive3";
        name = "Taunt";
        explain = "";

        this.level = level;
        coef = 1;

        basicMana = 5;
        incMana = 3;

        list = new List<Taunt>();
        guarded = new List<BattleChar>();
        enemies = new List<BattleChar>();
        debuff = new List<Buff>();

        key = "act3";
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);
        set.user.BM.zoomPlayerCam();

        //디버프
        foreach (BattleChar bc in target) {
            if (enemies.Exists(x => x == bc)) {
                debuff.ToArray()[enemies.IndexOf(bc)].turn = 3;
            }
            else {
                Buff d = new Buff();
                d.turn = 3;
                d.buffPhyATK -= 0.1f + 0.02f * level;
                debuff.Add(d);
                enemies.Add(bc);
                bc.buffStack.Add(d);
            }
        }

        //도발
        set.user.tauntTurn = 3;
        foreach (BattleChar g in set.user.BM.getGuarded(set.user))
        {
            if (guarded.Exists(x => x == g))
            {
                list.ToArray()[guarded.IndexOf(g)].setTurn(3);
            }
            else
            {
                Taunt t = new Taunt(3, set.user, g);
                list.Add(t);
                guarded.Add(g);
                g.stateTaunt.Add(t);
            }

        }

    }
}

[System.Serializable]
public class KnightActive4 : KnightActives
{
    [System.NonSerialized]
    Buff selfBuff; //만들어진 버프 객체를 관리

    public KnightActive4(int level, SkillKnight set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.SELF;
        type = SKILLTYPE.ACT;
        image = "battle/Knight/Skill/KnightActive4";
        name = "Fury";
        explain = "";

        this.level = level;
        coef = 1;

        basicMana = 2;
        incMana = 2;
        key = "act4";
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);
        set.user.BM.zoomPlayerCam();

        //자버프
        if (selfBuff == null)
        {
            selfBuff = new Buff();
            selfBuff.turn = 3;
            selfBuff.user = set.user;

            selfBuff.buffMgcDEF -= .2f + 0.02f * level;
            selfBuff.buffPhyDEF -= .2f + 0.02f * level;
            selfBuff.buffPhyATK += .2f + 0.04f * level;
            set.user.buffStack.Add(selfBuff);
        }
        else
        {
            selfBuff.turn = 3;
        }
    }
}

[System.Serializable]
public class KnightActive5 : KnightActives
{
    [System.NonSerialized]
    Counter count;

    public KnightActive5(int level, SkillKnight set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.SELF;
        type = SKILLTYPE.ACT;
        image = "battle/Knight/Skill/KnightActive5";
        name = "Counter Attack";
        explain = "";

        this.level = level;
        coef = 1;

        basicMana = 2;
        incMana = 2;
        key = "act5";

    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);
        set.user.BM.zoomPlayerCam();

        //자버프
        if (count == null)
        {
            count = new Counter(3, set.user, set.user, level, coef);
            set.user.stateCount = count;
        }
        else
        {
            count.setCounter(3);
        }
    }
}

[System.Serializable]
public class KnightPassive1 : KnightPassives
{
    [System.NonSerialized]
    Buff selfBuff;

    public KnightPassive1(int level, SkillKnight set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.SELF;
        type = SKILLTYPE.PAS;
        image = "battle/Knight/Skill/KnightPassive1";
        name = "Enduring";
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
            selfBuff.turn = 100;
            selfBuff.buffPhyDAM -= 0.05f + 0.02f * level;
            selfBuff.buffMgcDAM -= 0.05f + 0.02f * level;
            selfBuff.step = 0;

            set.user.buffStack.Add(selfBuff);
        }
        else
        {
            selfBuff.turn = 100;
        }
    }
}

[System.Serializable]
public class KnightPassive2 : KnightPassives
{
    [System.NonSerialized]
    Buff selfBuff;

    public KnightPassive2(int level, SkillKnight set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.SELF;
        type = SKILLTYPE.PAS;
        image = "battle/Knight/Skill/KnightPassive2";
        name = "Fast Guard";
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
            selfBuff = new KnightPas2Buff(level);
            selfBuff.user = set.user;
            selfBuff.turn = 100;
            selfBuff.step = 0;

            set.user.buffStack.Add(selfBuff);
        }
        else
        {
            selfBuff.turn = 100;
        }
    }
}

[System.Serializable]
public class KnightPassive3 : KnightPassives
{
    public KnightPassive3(int level, SkillKnight set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.SELF;
        type = SKILLTYPE.PAS;
        image = "battle/Knight/Skill/KnightPassive3";
        name = "Skillful Guard";
        explain = "";

        this.level = level;
        coef = 1;
    }


    public override void initSkill()
    {
        if (set.user.DC.charEquipSet.subWeapon.cls == EquipClass.Knight) isInit = true;
        else isInit = false;
    }

    public override void resetSkill()
    {
        isInit = false;
    }

    public void pas3(BattleChar bc) {

        if(isInit)
            if (Random.value <= (0.2f + level * 0.01f))
            {
                if (bc.stateSturn == null) bc.stateSturn = new Sturn(1, set.user, bc);
                else bc.stateSturn.setSturn(1, set.user, bc);
            }
    }
}

[System.Serializable]
public class KnightPassive4 : KnightPassives
{
    [System.NonSerialized]
    Buff selfBuff;

    public KnightPassive4(int level, SkillKnight set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.SELF;
        type = SKILLTYPE.PAS;
        image = "battle/Knight/Skill/KnightPassive4";
        name = "Mad Warrior";
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
            selfBuff = new KnightPas4Buff(level);
            selfBuff.turn = 1;
            selfBuff.step = 1;
            selfBuff.buffMgcATK += 0.3f;
            selfBuff.buffPhyATK += 0.3f;

            set.user.buffStack.Add(selfBuff);
        }
        else
        {
            selfBuff.turn = 1;
        }
    }
}

[System.Serializable]
public class KnightPassive5 : KnightPassives
{
    [System.NonSerialized]
    Buff selfBuff;

    public KnightPassive5(int level, SkillKnight set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.SELF;
        type = SKILLTYPE.PAS;
        image = "battle/Knight/Skill/KnightPassive5";
        name = "Efficient defense";
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
            selfBuff = new KnightPas5Buff(level);
            selfBuff.turn = 1;
            selfBuff.step = 0;

            set.user.buffStack.Add(selfBuff);
        }
        else
        {
            selfBuff.turn = 100;
        }
    }
}
